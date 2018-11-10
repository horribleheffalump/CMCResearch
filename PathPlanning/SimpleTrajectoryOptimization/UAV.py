import numpy as np

class UAV():
    #The UAV and the BS
    def __init__(self, V, X0, h):
        self.V = V                  # scalar UAV speed
        self.X0 = np.array(X0)      # initial position
        self.h = h                  # discritization step
        self.t = 0.0                # current time
        self.X = X0                 # current position
    def step(self, gamma):
        # UAV dynamics
        # gamma - current yaw angle
        self.t = self.t + self.h
        self.X = self.X + self.h * self.V * np.array([np.cos(gamma), np.sin(gamma)])
        return self.X
