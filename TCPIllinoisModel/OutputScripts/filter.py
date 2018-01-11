import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
import pandas as pd

from Points import *




f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
    
filename = u"../out/channel_state.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "X"])
X = data.as_matrix()

Xpoints = Points(X[:,0], X[:,1])
Xpoints.multiply()

n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)*0.0
levelone = np.ones(n)*1.0

filename = u"../out/filter_Discrete.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_d = data.t.as_matrix()
p_d = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename = u"../out/filter_DiscreteContinuous.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_dc = data.t.as_matrix()
p_dc = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename = u"../out/filter_DiscreteMeasureChange.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_dmc = data.t.as_matrix()
p_dmc = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename = u"../out/filter_DiscreteIndependent.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_di = data.t.as_matrix()
p_di = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename = u"../out/filter_DiscreteContinuousGaussian.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_dcg = data.t.as_matrix()
p_dcg = data[["p0", "p1", "p2", "p3"]].as_matrix()

#filename_dh = u"../out/CP_obs_0.txt"
#t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
#filename_dl = u"../out/CP_obs_1.txt"
#t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

#dhpoints = Points(t_dh, dh)
#dhpoints.toones()

#dlpoints = Points(t_dl, dl)
#dlpoints.toones()

ax0 = plt.subplot(411)
ax1 = plt.subplot(412)
ax2 = plt.subplot(413)
ax3 = plt.subplot(414)

plots = [ax3, ax2, ax1, ax0]


ax3.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax0.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.8, linewidth=0.0);

    
#ax3.plot(t_dc, p0_dc, color = 'blue')
#ax2.plot(t_dc, p1_dc, color = 'blue')
#ax1.plot(t_dc, p2_dc, color = 'blue')
#ax0.plot(t_dc, p3_dc, color = 'blue')

for i in range(0, 4):
    plots[i].set_xlim(0,max(t_d))
    plots[i].plot(t_d, p_d[:,i], color = 'red')
    plots[i].plot(t_dc, p_dc[:,i], color = 'cyan')
    plots[i].plot(t_dmc, p_dmc[:,i], color = 'magenta')
    plots[i].plot(t_dcg, p_dcg[:,i], color = 'blue')
    plots[i].plot(t_di, p_di[:,i], color = 'yellow')


#ax3.plot(dhpoints.x, dhpoints.y, 'o', color = 'black')
#ax3.plot(dlpoints.x, dlpoints.y, 'x', color = 'red')



##plt.savefig(u"../Output/ForIFAC.final/filter_" + str(n) + ".pdf")
plt.show()

