import numpy as np

class Channel():
    #Channel between the UAV and the BS
    def __init__(self, X0, r0):
        #self.l = np.array(l)    # vector of base state exit intensities
        self.X0 = np.array(X0)  # BS position
        self.r0 = np.array(r0)  # distanse where the noise is twice as higher as above the BS
    #def L(self, X):
    #    # state exit intensities
    #    # X - UAV position
    #    r = np.linalg.norm(self.X0-X) # squared distance
    #    l0 = self.l[0] * r/(self.r0)**2 / (1 + r/(self.r0)**2)
    #    l1 = self.l[1] / (1 + r/(self.r0)**2)
    #    return np.array([[-l0, l0],[l1,-l1]])
    def l(self, X):
        # SIR
        # X - UAV position
        r = np.linalg.norm(self.X0-X) # squared distance
        return 1.0 / (1 + np.power(r/self.r0, 2))
    def dl(self, X):
        # the derivative
        return 2 * (X - self.X0) * np.power(self.l(X),3) / np.power(self.r0, 2)
