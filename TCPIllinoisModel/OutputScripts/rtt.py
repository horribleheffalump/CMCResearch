import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
matplotlib.rc('text', usetex = True)
import pylab
from Points import *



filename = u"../out/ILLINOIS/control.txt"
#t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5,6,7,8,9,10,11,12), 
                   dtype=float, 
                   names = ["t", "u", "ss", "thresh", "m", "rtt", "alpha",  "d", "d_1", "d_m", "T_min", "T_max", "kappa_1"])
data = data[data.t < 60]

#print(data.head())
#.set_index('t', inplace=True)
#t_i = data.t.as_matrix()
#u_i = data.u.as_matrix()
#rtt_i = data.rtt.as_matrix()
#alpha_i = data.alpha.as_matrix()
#d_i = data.d.as_matrix()
#d1_i = data.d_1.as_matrix()
#dm_i = data.d_m.as_matrix()
#kappa1_i = data.kappa_1.as_matrix()


filename_X = u"../out/ILLINOIS/channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

Xpoints = Points(t_X, X)
Xpoints.multiply()  

f = plt.figure(num=None, figsize=(10, 6), dpi=150, facecolor='w', edgecolor='k')


#plt.plot(Xpoints.x, Xpoints.y*0.1, '-', color = 'red')
#plt.plot(t, m, '-', color = 'black', linewidth = 5.0)
#plt.plot(t, rtt, '-', color = 'blue')


ax1 = plt.subplot(111)
#ax1.set_ylim(0,0.2)
#ax1.set_xlim(0,40)
#plt.show()

ax1.plot(data.t, data.rtt - 0.1, '-', color = 'green', label = 'rtt')
ax1.plot(data.t, data.d, ':', color = 'red', label = 'd')
ax1.plot(data.t, data.d_1, ':', color = 'black', label = 'd_1')
ax1.plot(data.t, data.d_m, ':', color = 'blue', label = 'd_m')
#plt.plot(t, rttmin, '--', color = 'black')
#plt.plot(t, rttmax, '--', color = 'black')
#plt.plot(t, dm, '--', color = 'blue')
plt.legend()
plt.show()


#f.savefig("../output/alpha_beta.pdf")
