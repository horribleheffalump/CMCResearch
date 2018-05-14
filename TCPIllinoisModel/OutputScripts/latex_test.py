import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *

f, ax = plt.subplots()
x = np.linspace(0,1,100)
ax.plot(x, np.sin(x), '-', color = 'black')


plt.show()
