import numpy as np

class Channel():
    #Channel between the UAV and the BS
    def __init__(self, X0, r0):
        #self.l = np.array(l) # vector of base state exit intensities
        self.X0 = np.array(X0)  # BS position
        self.r0 = np.array(r0)  # distanse where the noise is twice as higher as above the BS
    #def L(self, X):
    #    # state exit intensities
                                  #    # X - UAV position
                                  #    r = np.linalg.norm(self.X0-X) # squared distance
                                  #    l0 = self.l[0] * r/(self.r0)**2 / (1 + r/(self.r0)**2)
                                  #    l1 = self.l[1] / (1 + r/(self.r0)**2)
                                  #    return np.array([[-l0, l0],[l1,-l1]])
    def r(self, X):
        return np.sqrt((X[0]-self.X0[0])* (X[0]-self.X0[0]) + (X[1]-self.X0[1])* (X[1]-self.X0[1])) 
        #np.linalg.norm(self.X0 - X) # distance
    def l(self, X):
        # SIR
        # X - UAV position
        return 1.0 / (1 + np.power(self.r(X) / self.r0, 2))
    def dl(self, X):
        # the derivative
        return -2.0 * (X - self.X0) * np.power(self.l(X) / self.r0, 2)
