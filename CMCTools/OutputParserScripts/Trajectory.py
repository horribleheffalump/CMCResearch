import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab


filename = u"../Output/MCTrajectory.txt"
t, X = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

tplot = np.zeros(len(t)*2-1)
Xplot = np.zeros(len(t)*2-1)
print(len(t))
print(t)
for i in range(0, len(t)-1):
    tplot[i*2] = t[i]
    Xplot[i*2] = X[i]
    tplot[i*2+1] = t[i+1]
    Xplot[i*2+1] = X[i]
tplot[len(t)*2-2] = t[len(t)-1]
Xplot[len(t)*2-2] = X[len(t)-1]

filename = u"../Output/CPTrajectory.txt"
tN, N = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)


#print(tplot)
#print(t[len(t)])

from pylab import *

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')

o = np.zeros(len(tplot))
ones = np.ones(len(tplot))
levelzero = np.ones(len(tplot))*0.0
levelone = np.ones(len(tplot))*1.0
leveltwo = np.ones(len(tplot))*2.0
levelthree = np.ones(len(tplot))*3.0


plt.subplots_adjust(left=0.06, bottom=0.07, right=0.98, top=0.95, wspace=0.1)
ax1 = plt.subplot(211)
ax2 = plt.subplot(212)

ax1.fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(tplot, levelone, leveltwo, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(tplot, leveltwo, levelthree, where=Xplot==ones*2, color='black', alpha = 0.8, linewidth=0.0);

ax1.plot(t, X, 'x', color = 'blue')
ax1.plot(tplot, Xplot, color = 'black')

ax2.plot(tN, N, color = 'black')

#show()
f.savefig("../Output/graph.pdf")