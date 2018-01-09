import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *




f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
    
filename = u"../out/channel_state.txt"
t_X, X = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

Xpoints = Points(t_X, X)
Xpoints.multiply()

n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)*0.0
levelone = np.ones(n)*1.0

filename = u"../out/filter_Discrete.txt"
t_d, p0_d, p1_d, p2_d, p3_d, u_d = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename = u"../out/filter_DiscreteContinuous.txt"
t_dc, p0_dc, p1_dc, p2_dc, p3_dc, u_dc = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_dh = u"../out/CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()

ax0 = plt.subplot(411)
ax1 = plt.subplot(412)
ax2 = plt.subplot(413)
ax3 = plt.subplot(414)


ax3.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax0.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);

    
ax3.plot(t_dc, p0_dc, color = 'blue')
ax2.plot(t_dc, p1_dc, color = 'blue')
ax1.plot(t_dc, p2_dc, color = 'blue')
ax0.plot(t_dc, p3_dc, color = 'blue')

ax3.plot(t_d, p0_d, color = 'red')
ax2.plot(t_d, p1_d, color = 'red')
ax1.plot(t_d, p2_d, color = 'red')
ax0.plot(t_d, p3_d, color = 'red')


ax3.plot(dhpoints.x, dhpoints.y, 'o', color = 'black')
ax3.plot(dlpoints.x, dlpoints.y, 'x', color = 'red')

#ax3.plot(t, u, color = 'red')


#ax0.set_xlim(0,50)
#ax1.set_xlim(0,50)
#ax2.set_xlim(0,50)
#ax3.set_xlim(0,50)
#plt.savefig(u"../Output/ForIFAC.final/filter_" + str(n) + ".pdf")
plt.show()

