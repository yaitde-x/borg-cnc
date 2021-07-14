import time
import json
import machineinterface
import readline
import logger

logWriter = logger.Logger(False)
machine = machineinterface.MachineInterface(
    logWriter, '/dev/tty.usbmodem14101', 115200, .1)

controlLoop = True

while controlLoop:
    line = raw_input("borg> ").strip()

    if (line == "x"):
        logWriter.debug("issuing shutdown")
        machine.shutdown()
        controlLoop = False
    elif (line == "?"):
        print("[id] = file, s = status, p = parameters, c = settings, g = state, x = exit, ? = this")
    elif (line.startswith("r ")):
        fileName = line[2:]
        logWriter.log("run " + fileName)
        machine.file(fileName)
    elif (line == "2"):
        logWriter.debug("run texas star gcode")
        machine.file(
            "/Users/sakamoto/.borg-cnc/library/texas-star-1/texas-star-1.062-em.gcode")
    elif (line == "s"):
        print(machine.getStatus())
    elif (line == "m"):
        print(machine.getMachineStatus())
    elif (line == "p"):
        print(machine.mdi("$#", True, True))
    elif (line == "c"):
        print(machine.mdi("$$", True, True))
    elif (line == "g"):
        print(machine.mdi("$G", True, True))
    else:
        logWriter.debug("mdi: " + line)
        machine.mdi(line, True)

