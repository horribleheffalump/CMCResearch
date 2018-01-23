


import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
       
import pandas as pd

filename_X = u"../out/channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = u"../out/CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')

Xpoints = Points(t_X, X)
Xpoints.multiply()

filename = u"../out/filter_Dummy.txt"
# 0 t
# 1-4 pi (p0)
# 5 X
# 6-9 R
# 10-13 G
# 14-17 C0
# 18-21 C1
# 22 C0[X] - SimultJumpsRatesSum
# 23 C1[X] - SimultJumpsRatesSum
# 24-26 I0 SimultJumpsRates 0->3, 1->3, 1->2
# 27-29 I1 SimultJumpsRates 0->3, 1->3, 1->2
# 30-32 A[0,3], A[1,3], A[1,2]
# 33-35 A[3,0], A[3,1], A[2,1]
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0, 5, 14, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32), dtype=float, names = ["t", "X", "C00", "C01", "C02", "C03", "C10", "C11", "C12", "C13", "CI0", "CI1", "I0_03", "I0_13", "I0_12", "I1_03", "I1_13", "I1_12", "A03", "A13", "A12"])
t = data.t.as_matrix()
Xreal = data.X.as_matrix()
C0 = data[["C00", "C01", "C02", "C03"]].as_matrix()
C1 = data[["C10", "C11", "C12", "C13"]].as_matrix()
CI0 = data.CI0.as_matrix()
CI1 = data.CI1.as_matrix()
I0 = data[["I0_03", "I0_13", "I0_12"]].as_matrix()
I1 = data[["I1_03", "I1_13", "I1_12"]].as_matrix()
A = data[["A03", "A13", "A12"]].as_matrix()
C0real = np.zeros(t.size)
C1real = np.zeros(t.size)

C0err = np.zeros(t.size)
C1err = np.zeros(t.size)


for i in range(0, t.size):
    C0real[i] = C0[i,int(Xreal[i])]
    C1real[i] = C1[i,int(Xreal[i])]
    C0err[i] = - CI0[i] + C0[i,int(Xreal[i])]
    C1err[i] = - CI1[i] + C1[i,int(Xreal[i])]
    if (int(Xreal[i]) == 0):
        C0err[i] = C0err[i] - I0[i,0]
        C1err[i] = C1err[i] - I1[i,0]
    if (int(Xreal[i]) == 1):
        C0err[i] = C0err[i] - I0[i,2] - I0[i,1]
        C1err[i] = C1err[i] - I1[i,2] - I1[i,1]



np.savetxt('c0err.txt', C0err, delimiter=' ')
np.savetxt('c1err.txt', C1err, delimiter=' ')

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
levelzero = np.ones(n)* C0err.min()
levelone = np.ones(n) * C0err.max()




ax1 = plt.subplot(111)
#ax1.plot(t, C0err, '-', color = 'black')
#ax1.plot(t, C1err, '-', color = 'blue', alpha = 0.6,  linewidth = 2.0)

#ax1.plot(t, C0[:,0], '-', color = 'blue', alpha = 0.2, linewidth = 3.0)
#ax1.plot(t, C0[:,1], '-', color = 'blue', alpha = 0.4, linewidth = 3.0)
#ax1.plot(t, C0[:,2], '-', color = 'blue', alpha = 0.6, linewidth = 3.0)
#ax1.plot(t, C0[:,3], '-', color = 'blue', alpha = 0.8, linewidth = 3.0)
#ax1.plot(t, C0real, '-', color = 'red', alpha = 1, linewidth = 1.0)
#ax1.plot(t, CI0, '-', color = 'magenta', alpha = 1, linewidth = 1.0)
ax1.plot(t, I0[:,0], '-', color = 'yellow', alpha = 0.4, linewidth = 3.0, label = "sigma: 0-3")
ax1.plot(t, I0[:,1], '-', color = 'green', alpha = 0.6, linewidth = 2.0, label = "sigma: 1-3")
ax1.plot(t, I0[:,2], '-', color = 'cyan', alpha = 1, linewidth = 1.0, label = "sigma: 1-2")
ax1.plot(t, A[:,0], ':', color = 'yellow', alpha = 1.0, linewidth = 3.0, label = "A[0-3]")
ax1.plot(t, A[:,1], ':', color = 'green', alpha = 1.0, linewidth = 2.0, label = "A[1-3]")
ax1.plot(t, A[:,2], ':', color = 'cyan', alpha = 1.0, linewidth = 1.0, label = "A[1-2]")

#ax1.plot(t, C1[:,0], '-', color = 'green', alpha = 0.2, linewidth = 2.0)
#ax1.plot(t, C1[:,1], '-', color = 'green', alpha = 0.4, linewidth = 2.0)
#ax1.plot(t, C1[:,2], '-', color = 'green', alpha = 0.6, linewidth = 2.0)
#ax1.plot(t, C1[:,3], '-', color = 'green', alpha = 0.8, linewidth = 2.0)
#ax1.plot(t, C1real, '-', color = 'green', alpha = 1, linewidth = 1.0)
#ax1.plot(t, I1[:,0], '-', color = 'magenta', alpha = 0.4, linewidth = 3.0, label = "nu: 0-3")
#ax1.plot(t, I1[:,1], '-', color = 'blue', alpha = 0.6, linewidth = 2.0, label = "nu: 1-3")
#ax1.plot(t, I1[:,2], '-', color = 'black', alpha = 1, linewidth = 1.0, label = "nu: 1-2")
#print(R)
#print(G)
#print(data)

#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);

#ax1.set_xlim(53, 55)

ax1.legend()


plt.show()

#f.savefig("../output/alpha_beta.pdf")



