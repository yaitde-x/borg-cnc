import time
import serial
import json
import machineinterface
from flask import Flask
from flask import request
from flask import Response
import sys
import logger

logWriter = logger.Logger(False)

def getComPort():

    # Opening JSON file 
    f = open('cfg.json',) 
  
    cfg = json.load(f) 
    
    f.close()

    return cfg['comPort']

app = Flask(__name__)
#machine = machineinterface.MachineInterface(logWriter,'/dev/ttyACM0', 115200, .1)
machine = machineinterface.MachineInterface(logWriter, getComPort(), 115200, .1)

@app.route('/api/machine/settings')
def get_settings():
    return machine.mdi('$$', True, True)

@app.route('/api/machine/engine')
def get_engineStatus():
    return Response(json.dumps(machine.getEngineStatus()), mimetype='application/json')

@app.route('/api/machine/job/<jobId>')
def get_jobStat(jobId):
    print("job" + jobId)
    return Response(json.dumps(machine.getJob(int(jobId))), mimetype='application/json')

@app.route('/api/machine/parameters')
def get_parameters():
    return machine.mdi('$#', True, True)

@app.route('/api/machine/state')
def get_state():
    return machine.getStatus()

@app.route('/api/machine/cstate')
def get_cstate():
    return machine.mdi('$G', True, True)

@app.route('/api/machine/home', methods=['POST'])
def post_home():
    return machine.mdi('$H', True, True)

@app.route('/api/machine/stop', methods=['POST'])
def post_abort():
    return machine.setMachineStatus(dict(abort=True))

@app.route('/api/machine/runblock', methods=['POST'])
def post_run():
    gCodeBlock = request.json
    job = machine.mdi(gCodeBlock['block'], True)
    return Response(json.dumps(job), mimetype='application/json')

@app.route('/api/machine/runfile', methods=['POST'])
def post_file():
    fileMetaPayload = request.json
    
    job = machine.file(fileMetaPayload['fileName'])
    return Response(json.dumps(job), mimetype='application/json')

#if __name__ == "__main__":
#    app.run(host='0.0.0.0', port=6040, debug=False)
