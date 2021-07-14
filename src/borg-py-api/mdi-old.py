#!/usr/bin/env python
# Much of this is a mashup of
# https://pymotw.com/2/readline/
# and
# https://github.com/gnea/grbl/blob/master/doc/script/simple_stream.py
import logging
import readline
import serial
import time
import termios
import fcntl
import os

LOG_FILENAME = '/tmp/borg-py-mdi.log'
logging.basicConfig(filename=LOG_FILENAME,
                    level=logging.DEBUG,
                    )

# Open grbl serial port
#se = serial.Serial('/dev/tty.usbmodem14101', 115200)
port = '/dev/tty.usbmodem14101'
# f = open(port)
# fd = f.fileno()
# flag = fcntl.fcntl(fd, fcntl.F_GETFL)
# fcntl.fcntl(fd, fcntl.F_SETFL, flag | os.O_NONBLOCK)

# attrs = termios.tcgetattr(f)
# attrs[2] = attrs[2] & ~termios.HUPCL
# termios.tcsetattr(f, termios.TCSAFLUSH, attrs)
# f.close()
# se = serial.Serial()
# se.baudrate = 115200
# se.port = port
# se.open()

se = serial.Serial()
se.port = port
se.baudrate = 115200
se.timeout = 1
se.setDTR(False)
se.open()

verbose = True

# Wake up grbl
se.write("\r\n\r\n")
time.sleep(2)   # Wait for grbl to initialize
se.flushInput()  # Flush startup text in serial input


class SimpleCompleter(object):
    
    def __init__(self, options):
        self.options = sorted(options)
        return

    def complete(self, text, state):
        response = None
        if state == 0:
            # This is the first time for this text, so build a match list.
            if text:
                self.matches = [s 
                                for s in self.options
                                if s and s.startswith(text)]
                logging.debug('%s matches: %s', repr(text), self.matches)
            else:
                self.matches = self.options[:]
                logging.debug('(empty input) matches: %s', self.matches)
        
        # Return the state'th item from the match list,
        # if we have that many.
        try:
            response = self.matches[state]
        except IndexError:
            response = None
        logging.debug('complete(%s, %s) => %s', 
                      repr(text), state, repr(response))
        return response

# Stream g-code to grbl
def input_loop():
    controlLoop = True

    while controlLoop:
        line = raw_input("borg> ")
        l = line.strip()  # Strip all EOL characters for consistency

        if (l != "x"):
            print 'Sending: ' + l,
            se.write(l + '\n')  # Send g-code block to grbl

            while 1:
                grbl_out = se.readline().strip()  # Wait for grbl response with carriage return
                if grbl_out.find('ok') >= 0:
                    if verbose:
                        print grbl_out
                    break
                elif grbl_out.find('error') >= 0:
                    if verbose:
                        print grbl_out
                    break
                else:
                    print "    MSG: \""+grbl_out+"\""

        else:
            controlLoop = False

# Register our completer function
readline.set_completer(SimpleCompleter(['start', 'stop', 'list', 'print']).complete)

# Use the tab key for completion
readline.parse_and_bind('tab: complete')

# Prompt the user for text
input_loop()

# Close file and serial port
se.close()
