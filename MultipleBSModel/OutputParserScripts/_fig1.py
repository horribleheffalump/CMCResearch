import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab

#filename = u"../Output/_fig1_TrTrajectory.txt"
#t, x, y, d1, d2, d3 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

#filename = u"../Output/_fig1_BaseStations.txt"
#bx, by = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
bx = [0.2, 0.7, 0.8]
by = [1.1, 1.6, 0.2]

t = np.linspace(0, 600) 
x =  1 / 600.0 * t
y = 10.0 * t / 600.0 - t * t / 36000.0


t1 = np.array([25, 90, 175, 425, 510, 575])
#np.linspace(0, 600, 5) 
x1 =  1 / 600.0 * t1
y1 = 10.0 * t1 / 600.0 - t1 * t1 / 36000.0

t2 = np.array([300])
#np.linspace(0, 600, 5) 
x2 =  1 / 600.0 * t2
y2 = 10.0 * t2 / 600.0 - t2 * t2 / 36000.0


from pylab import *

f = plt.figure(num=None, figsize=(3, 1), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.01, right=0.98, top=0.99, wspace=0.1)
#ax1 = plt.subplot(411)
ax1 = plt.subplot(111)
ax1.plot(x, y, '-', color = 'black', linewidth = 1.0)
ax1.plot(x1, y1, '3', color = 'black', linewidth = 1.5)
ax1.plot(x2, y2, '4', color = 'black', linewidth = 1.5)
ax1.plot(bx, by, '^', color = 'blue')
ax1.set_xlim(-0.05,1.05)
ax1.set_ylim(-0.05,2.55)
ax1.set_axis_off()

plt.text(0.22, 1.15, 'BS1', fontdict = {'size' : 10})
plt.text(0.57, 1.65, 'BS2', fontdict = {'size' : 10})
plt.text(0.82, 0.25, 'BS3', fontdict = {'size' : 10})

plt.savefig("../Output/ForIFAC.final/fig_trajectory.pdf")
