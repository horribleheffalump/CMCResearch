import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
       
import pandas as pd

filename = u"../out/STATEBASED/control.txt"
t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_X = u"../out/STATEBASED/channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = u"../out/STATEBASED/CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/STATEBASED/CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_co = u"../out/STATEBASED/cont_obs.txt"
t_co, co = np.loadtxt(filename_co, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)


f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')

Xpoints = Points(t_X, X)
Xpoints.multiply()

filename = u"../out/STATEBASED/filter_Dummy.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0, 6, 7, 8, 9, 10, 11, 12, 13), dtype=float, names = ["t", "G0", "G1", "G2", "G3", "R0", "R1", "R2", "R3"])
t_RG = data.t.as_matrix()
R = data[["R0", "R1", "R2", "R3"]].as_matrix()
G = data[["G0", "G1", "G2", "G3"]].as_matrix()

Rreal = np.zeros(t_RG.size)
Greal = np.zeros(t_RG.size)

for i in range(0, t_RG.size):
    x = int(Xpoints.val(t_RG[i]))
    Rreal[i] = R[i,x]
    Greal[i] = G[i,x]


#delta_p = 0.01
#D = [0.001, 0.01, 0.05, 0.1]
#K = [0.0005, 0.005, 0.025, 0.05]

#R = np.zeros(t.size)
#G = np.zeros(t.size)

#for i in range(0, t.size):
#    x = int(Xpoints.val(t[i]))
#    R[i] = 1.0 / (delta_p + D[x] + u[i] * K[x])
#    G[i] = 1.0 / np.sqrt(D[x] + u[i] * K[x])


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
levelone = np.ones(n)*R.max().max()




ax1 = plt.subplot(111)
ax1.plot(t_RG, R[:,0], '-', color = 'red', alpha = 0.4, linewidth = 8.0)
ax1.plot(t_RG, R[:,1], '-', color = 'green', alpha = 0.4, linewidth = 3.0)
ax1.plot(t_RG, R[:,2], '-', color = 'blue', alpha = 0.6, linewidth = 3.0)
ax1.plot(t_RG, R[:,3], '-', color = 'black', alpha = 0.8, linewidth = 3.0)
#ax1.plot(t_RG, Rreal, '-', color = 'red', alpha = 1, linewidth = 1.0)

#ax1.plot(t_RG, G[:,0], '-', color = 'green', alpha = 0.2, linewidth = 2.0)
#ax1.plot(t_RG, G[:,1], '-', color = 'green', alpha = 0.4, linewidth = 2.0)
#ax1.plot(t_RG, G[:,2], '-', color = 'green', alpha = 0.6, linewidth = 2.0)
#ax1.plot(t_RG, G[:,3], '-', color = 'green', alpha = 0.8, linewidth = 2.0)
#ax1.plot(t_RG, Greal, '-', color = 'green', alpha = 1, linewidth = 1.0)
#print(R)
#print(G)
#print(data)

ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);



#ax2 = ax1.twinx()
#ax2.plot(t_co, co, '-', color = 'black')

plt.show()


#f.savefig("../output/alpha_beta.pdf")




