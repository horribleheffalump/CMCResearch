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

filename_co = u"../out/cont_obs.txt"
t_co, co = np.loadtxt(filename_co, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)


f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')

Xpoints = Points(t_X, X)
Xpoints.multiply()

delta_p = 0.01
D = [0.001, 0.01, 0.05, 0.1]
K = [0.0005, 0.005, 0.025, 0.05]

R = np.zeros(len(t))
G = np.zeros(len(t))

for i in range(0, len(t)):
    x = int(Xpoints.val(t[i]))
    R[i] = 1.0 / (delta_p + D[x] + u[i] * K[x])
    G[i] = 1.0 / np.sqrt(D[x] + u[i] * K[x])


dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()


#plt.plot(t, u, '-', color = 'blue')
#plt.plot(t, ss, '--', color = 'green')
#plt.plot(t, thresh, ':', color = 'yellow')
#plt.plot(Xpoints.x, Xpoints.y, '-', color = 'black')
#plt.plot(dhpoints.x, dhpoints.y, 'o', color = 'black')
#plt.plot(dlpoints.x, dlpoints.y, 'x', color = 'red')


n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)*0.0
levelone = np.ones(n)*100.0




ax1 = plt.subplot(111)
ax1.plot(t, R, '-', color = 'blue')
ax1.plot(t, G, '-', color = 'green')

ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);



ax2 = ax1.twinx()
ax2.plot(t_co, co, '-', color = 'black')

plt.show()


#f.savefig("../output/alpha_beta.pdf")



