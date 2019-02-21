import numpy as np

class Target():
    #The UAV survaillance target
    def __init__(self, X0, d0):
        self.X0 = np.array(X0)  # target position
        self.d0 = np.array(d0)  # distanse where the survailance quality is twice as lower then above the target position
    def nu(self, X):
        # the survaillance quality
        # X - UAV position
        d = np.linalg.norm(self.X0-X) # squared distance
        return 1.0 / (1.0 + np.power(d / self.d0, 2))
    def dnu(self, X):
        # the derivative
        return 2 * (X - self.X0) * np.power(self.nu(X),3) / np.power(self.d0, 2)
