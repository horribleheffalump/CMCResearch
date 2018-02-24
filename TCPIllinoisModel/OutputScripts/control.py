import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *

#subfolder = ''
subfolder = 'ILLINOIS/'
interval = [0,500]
#bounds = [0,0.11]


filename = u"../out/" + subfolder + "control.txt"
t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_X = u"../out/" + subfolder + "channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = u"../out/" + subfolder + "CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/" + subfolder + "CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(10, 6), dpi=150, facecolor='w', edgecolor='k')

Xpoints = Points(t_X, X)
Xpoints.multiply()


n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)*0.0
levelone = np.ones(n)*u.max()


dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()

#print(Xpoints.x)
#print(Xpoints.y)

ax1 = plt.subplot(111)

ax1.plot(t, ss * u, '-', color = 'blue')
ax1.plot(t, (1-ss) * u, '-', color = 'green')
#plt.plot(t, ss * max(u) / 2.0, '--', color = 'green')
#ax1.plot(t, thresh, ':', color = 'yellow')
#plt.plot(Xpoints.x, Xpoints.y, '-', color = 'black')
ax1.plot(dhpoints.x, dhpoints.y, '.', color = 'black')
ax1.plot(dlpoints.x, dlpoints.y, 'x', color = 'red')

ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);

ax2 = ax1.twinx()
ax2.plot(t, rtt, '-', color = 'red')

#plt.plot(t, intf, '-', color = 'green')
#plt.plot(t, intm, '-', color = 'red')
#ax1.set_ylim(bounds[0],bounds[1])
ax1.set_xlim(interval[0],interval[1])
ax2.set_xlim(interval[0],interval[1])
ax2.set_ylim(0.1,0.12)
plt.show()

#plt.plot(t, rtt, '--', color = 'red')
#plt.plot(t, rttmin, '--', color = 'black')
#plt.plot(t, rttmax, '--', color = 'black')
#plt.plot(t, dm, '--', color = 'blue')
#plt.show()


#f.savefig("../output/alpha_beta.pdf")


