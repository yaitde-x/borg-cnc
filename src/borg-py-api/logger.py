from abc import ABCMeta, abstractmethod

class ILogger:
    __metaclass__ = ABCMeta

    @classmethod
    def version(self): return "1.0"
    @abstractmethod
    def log(self, msg): raise NotImplementedError
    @abstractmethod
    def debug(self, msg): raise NotImplementedError

class Logger(ILogger):

    def __init__(self, logDebug=True):
        self.logDebug = logDebug

    def log(self, msg):
        print(msg)

    def debug(self, msg):
        if (self.logDebug):
            print(msg)

class NullLogger(ILogger):

    def log(self, msg):
        x = 1

    def debug(self, msg):
        x = 1
