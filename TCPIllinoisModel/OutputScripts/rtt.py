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

Xpoints = Points(t_X, X)
Xpoints.multiply()

f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')


plt.plot(Xpoints.x, Xpoints.y*0.1, '-', color = 'red')
plt.plot(t, m, '-', color = 'black')
plt.plot(t, rtt, '-', color = 'blue')


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
