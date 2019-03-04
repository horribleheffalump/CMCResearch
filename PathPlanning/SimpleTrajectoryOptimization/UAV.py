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
    def setmission(self, target, channel, kappa, epsilon):
        self.target = target        # survaillance target
        self.channel = channel      # data transfer channel to the base station
        self.kappa = kappa          # data transfer cost
        self.epsilon = epsilon      # minimum control
    def step(self, gamma):
        # UAV dynamics
        # gamma - current yaw angle
        self.t = self.t + self.h
        self.X = self.X + self.h * self.V * np.array([np.cos(gamma), np.sin(gamma)])
        return self.X
    def u(self, t, X):
        # optimal control, depends on the state
        uu = self.target.nu(t,X) / self.kappa - 1.0 / self.channel.l(X)
        if uu > self.epsilon:
            return uu
        else:
            return self.epsilon
    def du(self, t, X):
        # optimal control derivative
        if self.u(t,X) > self.epsilon:
            return self.target.dnu(t,X) / self.kappa + self.channel.dl(X) / np.power(self.channel.l(X), 2) 
        else:
            return 0