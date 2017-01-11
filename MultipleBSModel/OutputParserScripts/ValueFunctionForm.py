import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab




from pylab import *

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')

plt.subplots_adjust(left=0.06, bottom=0.07, right=0.98, top=0.95, wspace=0.1)
ax1 = plt.subplot(111)

filename = u"../Output/VFForm.txt"
u, vf0,vf1,vf2 = np.loadtxt(filename, delimiter = ',', usecols=(0,1,3,5), unpack=True, dtype=float)
ax1.plot(u, vf0, '-', color = 'green')
ax1.plot(u, vf1, '-', color = 'blue')
ax1.plot(u, vf2, '-', color = 'red')

ax1.set_xlim(0.1,1.0)

f.savefig("../Output/vf_of_u.png")

#show()