import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
       

filename = u"../out/control.txt"
t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_X = u"../out/channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = u"../out/CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')

Xpoints = Points(t_X, X)
Xpoints.multiply()

dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()

#print(Xpoints.x)
#print(Xpoints.y)

plt.plot(t, u, '-', color = 'blue')
plt.plot(t, ss, '--', color = 'green')
plt.plot(t, thresh, ':', color = 'yellow')
plt.plot(Xpoints.x, Xpoints.y, '-', color = 'black')
plt.plot(dhpoints.x, dhpoints.y, 'o', color = 'black')
plt.plot(dlpoints.x, dlpoints.y, 'x', color = 'red')

#plt.plot(t, intf, '-', color = 'green')
#plt.plot(t, intm, '-', color = 'red')
#plt.plot(t, rtt, '-', color = 'blue')
ax1 = plt.subplot(111)
#ax1.set_ylim(0,0.2)
#ax1.set_xlim(0,40)
plt.show()

#plt.plot(t, rtt, '--', color = 'red')
#plt.plot(t, rttmin, '--', color = 'black')
#plt.plot(t, rttmax, '--', color = 'black')
#plt.plot(t, dm, '--', color = 'blue')
#plt.show()


#f.savefig("../output/alpha_beta.pdf")


