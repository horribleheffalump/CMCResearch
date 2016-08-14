import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab

filename = u"../Output/TrTrajectory.txt"
x, y = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename = u"../Output/BaseStations.txt"
bx, by = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

from pylab import *

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')


filename = u"../Output/MCTrajectory_0.txt"
t0, X0 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

t0plot = np.zeros(len(t0)*2-1)
X0plot = np.zeros(len(t0)*2-1)
for i in range(0, len(t0)-1):
    t0plot[i*2] = t0[i]
    X0plot[i*2] = X0[i]
    t0plot[i*2+1] = t0[i+1]
    X0plot[i*2+1] = X0[i]
t0plot[len(t0)*2-2] = t0[len(t0)-1]
X0plot[len(t0)*2-2] = X0[len(t0)-1]
o0 = np.zeros(len(t0plot))
ones0 = np.ones(len(t0plot))
levelzero0 = np.ones(len(t0plot))*0.0
levelone0 = np.ones(len(t0plot))*1.0

filename = u"../Output/MCTrajectory_1.txt"
t1, X1 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

t1plot = np.zeros(len(t1)*2-1)
X1plot = np.zeros(len(t1)*2-1)
for i in range(0, len(t1)-1):
    t1plot[i*2] = t1[i]
    X1plot[i*2] = X1[i]
    t1plot[i*2+1] = t1[i+1]
    X1plot[i*2+1] = X1[i]
t1plot[len(t1)*2-2] = t1[len(t1)-1]
X1plot[len(t1)*2-2] = X1[len(t1)-1]
o1 = np.zeros(len(t1plot))
ones1 = np.ones(len(t1plot))
levelzero1 = np.ones(len(t1plot))*0.0
levelone1 = np.ones(len(t1plot))*1.0

filename = u"../Output/MCTrajectory_2.txt"
t2, X2 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

t2plot = np.zeros(len(t2)*2-1)
X2plot = np.zeros(len(t2)*2-1)
for i in range(0, len(t2)-1):
    t2plot[i*2] = t2[i]
    X2plot[i*2] = X2[i]
    t2plot[i*2+1] = t2[i+1]
    X2plot[i*2+1] = X2[i]
t2plot[len(t2)*2-2] = t2[len(t2)-1]
X2plot[len(t2)*2-2] = X2[len(t2)-1]
o2 = np.zeros(len(t2plot))
ones2 = np.ones(len(t2plot))
levelzero2 = np.ones(len(t2plot))*0.0
levelone2 = np.ones(len(t2plot))*1.0

#filename = u"../Output/CPTrajectory.txt"
#tN, N = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

plt.subplots_adjust(left=0.06, bottom=0.07, right=0.98, top=0.95, wspace=0.1)
ax1 = plt.subplot(411)
ax1.plot(x, y, '-', color = 'black')
ax1.plot(bx, by, 'x', color = 'blue')


ax2 = plt.subplot(412)
ax3 = plt.subplot(413)
ax4 = plt.subplot(414)

ax2.plot(t0plot, X0plot, color = 'black')
ax2.fill_between(t0plot, levelzero0, levelone0, where=X0plot==o0, color='black', alpha = 0.2, linewidth=0.0);
ax2.fill_between(t0plot, levelzero0, levelone0, where=X0plot==ones0, color='black', alpha = 0.4, linewidth=0.0);
ax2.fill_between(t0plot, levelzero0, levelone0,  where=X0plot==ones0*2, color='black', alpha = 0.8, linewidth=0.0);

ax3.plot(t1plot, X1plot, color = 'black')
ax3.fill_between(t1plot, levelzero1, levelone1, where=X1plot==o1, color='black', alpha = 0.2, linewidth=0.0);
ax3.fill_between(t1plot, levelzero1, levelone1, where=X1plot==ones1, color='black', alpha = 0.4, linewidth=0.0);
ax3.fill_between(t1plot, levelzero1, levelone1,  where=X1plot==ones1*2, color='black', alpha = 0.8, linewidth=0.0);

ax4.plot(t2plot, X2plot, color = 'black')
ax4.fill_between(t2plot, levelzero2, levelone2, where=X2plot==o2, color='black', alpha = 0.2, linewidth=0.0);
ax4.fill_between(t2plot, levelzero2, levelone2, where=X2plot==ones2, color='black', alpha = 0.4, linewidth=0.0);
ax4.fill_between(t2plot, levelzero2, levelone2,  where=X2plot==ones2*2, color='black', alpha = 0.8, linewidth=0.0);

#ax2.plot(tN, N, color = 'black')















show()
#f.savefig("../Output/graph.pdf")

