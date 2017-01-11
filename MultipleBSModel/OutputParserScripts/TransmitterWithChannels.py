import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab

filename = u"../Output/TrTrajectory.txt"
t, x, y, d1, d2, d3 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename = u"../Output/BaseStations.txt"
bx, by = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

from pylab import *

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.07, right=0.98, top=0.95, wspace=0.1)
#ax1 = plt.subplot(411)
#plt.axis('equal')
ax1 = plt.subplot(111)
ax1.plot(x, y, '-', color = 'black')
ax1.plot(bx, by, 'x', color = 'blue')
ax1.set_xlim(0,1)

plt.savefig("../Output/trajectory.pdf")
#plt.show()

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)

ax2 = plt.subplot(311)
ax2_N = ax2.twinx()
ax3 = plt.subplot(312)
ax3_N = ax3.twinx()
ax4 = plt.subplot(313)
ax4_N = ax4.twinx()

ax = [ax2,ax3,ax4]
ax_N = [ax2_N,ax3_N,ax4_N]

ax2.plot(t, d1, '-', color = 'red')
ax3.plot(t, d2, '-', color = 'red')
ax4.plot(t, d3, '-', color = 'red')

ax2.set_xlim(0,600)
ax3.set_xlim(0,600)
ax4.set_xlim(0,600)

for n in range(0,3):
    filename = u"../Output/MCTrajectory_" + str(n) + ".txt"
    t, X = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
    tplot = np.zeros(len(t)*2-1)
    Xplot = np.zeros(len(t)*2-1)
    for i in range(0, len(t)-1):
        tplot[i*2] = t[i]
        Xplot[i*2] = X[i]
        tplot[i*2+1] = t[i+1]
        Xplot[i*2+1] = X[i]
    tplot[len(t)*2-2] = t[len(t)-1]
    Xplot[len(t)*2-2] = X[len(t)-1]
    o = np.zeros(len(tplot))
    ones = np.ones(len(tplot))
    levelzero = np.ones(len(tplot))*0.0
    levelone = np.ones(len(tplot))*1.0
    leveltwo = np.ones(len(tplot))*2.0
    levelthree = np.ones(len(tplot))*3.0
    #ax[n].plot(tplot, Xplot, color = 'black')
    ax[n].fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
    ax[n].fill_between(tplot, levelone, leveltwo, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
    ax[n].fill_between(tplot, leveltwo, levelthree,  where=Xplot==ones*2, color='black', alpha = 0.8, linewidth=0.0);
    for tl in ax[n].get_yticklabels():
        tl.set_color('r')

    filename = u"../Output/CPTrajectory_" + str(n) + ".txt"
    tN, N = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
    ax_N[n].plot(tN, N, color = 'blue')
    for tl in ax_N[n].get_yticklabels():
        tl.set_color('b')

plt.show()
#f.savefig("../Output/graph.pdf")
#plt.savefig("../Output/observations.pdf")

