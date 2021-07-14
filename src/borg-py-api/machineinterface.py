import threading
import time
import serial
import queue as Queue
import re
import logger
import datetime as dt

RX_BUFFER_SIZE = 128
BAUD_RATE = 115200
ENABLE_STATUS_REPORTS = True
REPORT_INTERVAL = 1.0  # seconds

verbose = True

class MachineInterface(object):

    def __init__(self, logger, port, baud, pollingInterval=.1):

        self.pollingInterval = pollingInterval
        self.logger = logger
        self.currentJob = None
        self.lastJob = None;

        # self.serialPort = serial.Serial(port, baud)
        self.serialPort = serial.Serial()
        self.serialPort.port = port
        self.serialPort.baudrate = 115200
        self.serialPort.timeout = 1
        self.serialPort.setDTR(False)
        self.serialPort.open()
        self.timer = dt.datetime.utcnow()

        # Wake up grbl
        self.serialPort.write("\r\n\r\n".encode())
        time.sleep(2)   # Wait for grbl to initialize
        self.serialPort.flushInput()  # Flush startup text in serial input

        self.status = ""
        self.syncLock = threading.RLock()
        self.allowMdi = True
        self.runQueue = Queue.Queue()
        self.historyQueue = dict()
        self.jobId = 0
        self.machineStatus = dict(
            id=None, status="Idle", type="", error="", errorCount=0, data=None)
        self.isShuttingDown = False

        self.thread = threading.Thread(target=self.run, args=[pollingInterval])
        self.thread.daemon = True                            # Daemonize thread
        self.thread.start()                                  # Start the execution

    def shouldUpdateStatus(self):
        return (dt.datetime.utcnow() - self.timer).microseconds > 1000

    def shutdown(self):
        self.logger.debug("shutdown")
        self.isShuttingDown = True
        self.logger.debug("drain queue")
        self.runQueue.join()
        self.logger.debug("term bg thread")
        self.thread.join()
        self.logger.debug("shutdown: done")

    def setMachineStatus(self, status):
        self.logger.debug("set-machine-status")
        self.syncLock.acquire()

        if ("abort" in status):
            self.machineStatus["abort"] = status["abort"]

        if ("id" in status):
            self.machineStatus["id"] = status["id"]

        if ("type" in status):
            self.machineStatus["type"] = status["type"]

        if ("status" in  status):
            self.machineStatus["status"] = status["status"]

        if ("error" in status):
            self.machineStatus["error"] = status["error"]
        
        if ("errorCount" in status):
            self.machineStatus["errorCount"] = status["errorCount"]
        
        if ("data" in status):
            self.machineStatus["data"] = status["data"]

        self.syncLock.release()
        self.logger.debug("set-machine-status: done")

    def getMachineStatus(self):
        return self.machineStatus

    def getStatus(self):
        self.syncLock.acquire()
        statusBuf = self.status
        self.syncLock.release()
        return statusBuf

    def getJob(self, jobId):
        if jobId in self.historyQueue:
            return self.historyQueue[jobId]
        return None

    def getEngineStatus(self):
        ljob = self.cloneJob(self.lastJob)
        cjob = self.cloneJob(self.currentJob)

        return dict(lastJob = ljob, currentJob = cjob)

    def cloneJob(self, job):

        if job is None:
            return dict()

        newJob = dict(id=job["id"], type=job["type"], eof=job["eof"], status=job["status"], result=job["result"],
                        errorCount=job["errorCount"])

        if newJob["type"] == "mdi":
            newJob["block"] = job["block"]
        else:
            newJob["fileName"] = job["fileName"]
            newJob["lineCount"] = job["lineCount"]
            newJob["charCounts"] = job["charCounts"]
            newJob["grblCount"] = job["grblCount"]

        return newJob

    def setStatus(self, status):
        self.logger.debug("set-status")
        self.syncLock.acquire()
        self.status = status
        self.timer = dt.datetime.utcnow()
        self.syncLock.release()
        self.logger.debug("set-status: done")

    def mdi(self, cmd, newLine, block=False):
        self.logger.debug("queue mdi command: " + cmd)
        self.jobId = self.jobId + 1

        job = dict(id=self.jobId, type="mdi", eof=0, status=0, result="",
                   block=cmd, errorCount=0, newLine=newLine)
        self.runQueue.put(job)

        if (block):
            while (job["status"] == 0):
                time.sleep(0.05)
            return job["result"]

        return job;

    def file(self, fileName):
        self.logger.debug("queue file job: " + fileName)
        self.jobId = self.jobId + 1
        job = dict(id=self.jobId, type="file", eof=0, status=0,
                               fileName=fileName, lineCount=0, errorCount=0, fd=None, 
                               result="", charCounts = 0, grblCount = 0 )
        self.runQueue.put(job)
        return self.cloneJob(job)

    def run(self, p):

        state = 0  # idle
        self.currentJob = None

        while state != 3:

            currentStatus = self.getMachineStatus()
            if ("abort" in currentStatus and currentStatus["abort"]):
                # drain the queue
                draining = True
                self.logger.debug('abort detected, draining')
                while draining:
                    try:
                        jobIWontRun = self.runQueue.get(True, .01)

                    except Queue.Empty:
                        draining = False
                state = 0
                self.lastJob = self.currentJob
                self.lastJob["result"] = "aborted"

            if state == 0:
                while (self.currentJob is None and not self.isShuttingDown):
                    try:
                        self.logger.debug('next job or sleep')
                        self.currentJob = self.runQueue.get(True, 1)

                    except Queue.Empty:
                        self.logger.debug(
                            "timed waiting for job. housekeeping now")

                        if (self.shouldUpdateStatus()):
                            self.exec_status()

                if (self.currentJob):
                    self.logger.debug('run job : ' + str(self.currentJob["id"]))
                state = 1

            if (self.isShuttingDown and self.currentJob is None):
                state = 3
                continue

            if (self.shouldUpdateStatus()):
                self.exec_status()

            # Doing real work
            if (self.currentJob["type"] == "mdi"):
                self.setMachineStatus(
                    dict(id=self.currentJob["id"], status="Running", type="mdi", data=self.currentJob["block"]))

                self.logger.debug('running mdi job  ' + self.currentJob["block"])
                self.currentJob["result"] = self.exec_cmd(self.currentJob["block"], self.currentJob["newLine"])
                self.runQueue.task_done()
                self.currentJob["status"] = 1
                state = 0
                self.lastJob = self.currentJob
                self.historyQueue[self.currentJob["id"]] = self.currentJob
                self.currentJob = None
                self.setMachineStatus(dict(id=None, status="Idle", type=""))

            elif (self.currentJob["type"] == "file" and self.currentJob["fd"] == None):
                self.setMachineStatus(
                    dict(id=self.currentJob["id"], status="Running", type="mdi", data=self.currentJob["fileName"]))

                self.logger.debug('starting file ' + self.currentJob["fileName"])
                self.prep_file_job(self.currentJob)
            elif (self.currentJob["eof"] >= 2):
                self.logger.debug('cleanup file ' + self.currentJob["fileName"])
                self.runQueue.task_done()
                self.cleanup_file_job(self.currentJob)
                self.currentJob["status"] = 1
                self.currentJob["result"] = "Ok"
                self.lastJob = self.currentJob
                self.historyQueue[self.currentJob["id"]] = self.currentJob
                self.currentJob = None
                state = 0
                self.setMachineStatus(dict(id=None, status="Idle", type=""))
            else:
                self.logger.debug('continue file job ' + self.currentJob["fileName"])
                self.exec_file(self.currentJob)

    def prep_file_job(self, job):
        # need to setup the file for reading
        job["grblCount"] = 0
        self.logger.debug("prep file " + job["fileName"])
        job["fd"] = open(job["fileName"], 'r')
        job["charCounts"] = []
        job["lineCount"] = 0

    def cleanup_file_job(self, job):
        # need to setup the file for reading
        job["fd"].close()
        job["fd"] = None

    def exec_status(self):
        self.logger.debug("exec-status")
        self.serialPort.write("?".encode())
        result = self.serialPort.readline().strip()
        self.setStatus(result)
        self.logger.debug("exec-status:done")

    def exec_file(self, job):

        grbl_out = ''

        # line = job["fd"].readline(job["lineCount"])
        line = job["fd"].readline()

        l_block = re.sub('\s|\(.*?\)', '', line).upper()

        if (l_block == ""):
            job["eof"] += 1

        job["charCounts"].append(len(l_block)+1)

        while sum(job["charCounts"]) >= RX_BUFFER_SIZE-1 | self.serialPort.inWaiting():
            out_temp = self.serialPort.readline().strip()  # Wait for grbl response
            if out_temp.find('ok') < 0 and out_temp.find('error') < 0:
                self.logger.debug("    MSG: \""+out_temp +
                                  "\"")  # Debug response
            else:
                if out_temp.find('error') >= 0:
                    self.setMachineStatus(dict(error=out_temp, errorCount=job["errorCount"]))
                    job["errorCount"] += 1

                job["grblCount"] += 1  # Iterate g-code counter
                self.logger.debug(
                    "REC < "+str(job["grblCount"])+": \""+out_temp+"\"")
                # Delete the block character count corresponding to the last 'ok'
                del job["charCounts"][0]

        if (len(l_block) > 0):
            job["lineCount"] += 1  # Iterate line counter
            self.setMachineStatus(dict(data=l_block));
            self.serialPort.write(str.encode(l_block + '\n'))  # Send g-code block to grbl
            self.logger.debug(
                "SND > "+str(job["lineCount"])+": \"" + l_block + "\"")

        # Wait until all responses have been received.
        while job["lineCount"] > job["grblCount"]:
            out_temp = self.serialPort.readline().decode().strip()  # Wait for grbl response
            if out_temp.find('ok') < 0 and out_temp.find('error') < 0:
                self.logger.debug("-> MSG: \""+out_temp+"\"")  # Debug response
            else:
                if out_temp.find('error') >= 0:
                    self.setMachineStatus(dict(error=out_temp, errorCount=job["errorCount"]))
                    job["errorCount"] += 1
                job["grblCount"] += 1  # Iterate g-code counter

                # Delete the block character count corresponding to the last 'ok'
                del job["charCounts"][0]
                if verbose:
                    self.logger.debug(
                        "REC < "+str(job["grblCount"])+": \""+out_temp + "\"")

    def exec_cmd(self, cmd, newline=True):

        if (not self.allowMdi):
            return ""

        self.logger.debug("SND > "+cmd)
        self.setMachineStatus(dict(data=cmd));
        self.serialPort.write(cmd.encode())

        if newline:
            self.serialPort.write("\n".encode())

        resp = ''

        while 1:
            # Wait for grbl response with carriage return
            grbl_out = self.serialPort.readline().decode('utf8').strip()
            self.logger.debug("REC < "+grbl_out)
            if grbl_out.find("Grbl") >= 0:
                resp = ''
            elif grbl_out.find("ok") >= 0:
                break
            elif grbl_out.find("error") >= 0:
                self.setMachineStatus(dict(error=grbl_out, errorCount=1))
                break
            else:
                resp += grbl_out

        return resp
