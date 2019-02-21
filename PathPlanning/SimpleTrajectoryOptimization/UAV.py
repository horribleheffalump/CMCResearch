import numpy as np
from SurveillanceTarget import *
from Channel import *

class UAV():
    #The UAV and the BS
    def __init__(self, V, X0, h):
        self.V = V                  # scalar UAV speed
        self.X0 = np.array(X0)      # initial position
        self.h = h                  # discritization step
        self.t = 0.0                # current time
        self.X = X0                 # current position
    def setmission(self, target, channel, kappa):
        self.target = target        # survaillance target
        self.channel = channel      # data transfer channel to the base station
        self.kappa = kappa          # data transfer cost
    def step(self, gamma):
        # UAV dynamics
        # gamma - current yaw angle
        self.t = self.t + self.h
        self.X = self.X + self.h * self.V * np.array([np.cos(gamma), np.sin(gamma)])
        return self.X
    def u(self, X):
        # optimal control, depends on the state
        return self.target.nu(X) / self.kappa - 1.0 / self.channel.l(X) 
    def du(self, X):
        # optimal control derivative
        return self.target.dnu(X) / self.kappa + self.channel.dl(X) / np.power(self.channel.l(X), 2) 