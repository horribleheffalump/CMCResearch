import numpy as np

class Points():
    def __init__(self, x , y):
        self.x = x
        self.y = y
    def multiply(self):
        xplot = np.zeros(len(self.x) * 2 - 1)
        yplot = np.zeros(len(self.x) * 2 - 1)
        for i in range(0, len(self.x) - 1):
            xplot[i * 2] = self.x[i]
            yplot[i * 2] = self.y[i]
            xplot[i * 2 + 1] = self.x[i + 1]
            yplot[i * 2 + 1] = self.y[i]
        xplot[len(self.x) * 2 - 2] = self.x[len(self.x) - 1]
        yplot[len(self.x) * 2 - 2] = self.y[len(self.x) - 1]
        self.x = xplot
        self.y = yplot
    def toones(self):
        self.y = np.ones(len(self.y))

