import numpy as np

class Target():
    #The UAV survaillance target
    def __init__(self, X0, Xa, d0):
        self.Xp0 = np.array(X0)  # target position
        self.XpT = np.array(Xa)  # target position
        self.d0 = np.array(d0)  # distanse where the survailance quality is twice as lower as above the
                                # target position

    def d(self, t, X):
        #return np.linalg.norm(self.Xp(t) - X) # distance
        return np.sqrt((X[0]-self.Xp(t)[0])* (X[0]-self.Xp(t)[0]) + (X[1]-self.Xp(t)[1])* (X[1]-self.Xp(t)[1])) 
    def Xp(self, t):
        #return self.Xp0 + np.transpose((self.XpT-self.Xp0).reshape(2,1) * t)
        return self.Xp0 + t * (self.XpT-self.Xp0)
                                
    def nu(self, t, X):
        # the survaillance quality
        # X - UAV position
        return 1.0 / (1.0 + np.power(self.d(t,X) / self.d0, 2))
    def dnu(self, t, X):
        # the derivative
        return -2.0 * (X - self.Xp(t)) * np.power(self.nu(t,X),2) / np.power(self.d0, 2)
