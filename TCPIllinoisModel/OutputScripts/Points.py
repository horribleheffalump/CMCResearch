import numpy as np

class Points():
    def __init__(self, x , y):
        self.x = np.array(x);
        self.y = np.array(y);
    def multiply(self):
        xplot = np.zeros(self.x.size * 2 - 1)
        yplot = np.zeros(self.x.size * 2 - 1)
        for i in range(0, self.x.size - 1):
            xplot[i * 2] = self.x[i]
            yplot[i * 2] = self.y[i]
            xplot[i * 2 + 1] = self.x[i + 1]
            yplot[i * 2 + 1] = self.y[i]
        xplot[self.x.size * 2 - 2] = self.x[self.x.size - 1]
        yplot[self.x.size * 2 - 2] = self.y[self.x.size - 1]
        self.x = xplot
        self.y = yplot
    def toones(self):
        self.y = np.ones(self.y.size)
    def val(self,x):
        for i in range(0, self.x.size):
            if (self.x[i] >= x):
                return self.y[i]
        return (self.y[self.x.size-1])

        

