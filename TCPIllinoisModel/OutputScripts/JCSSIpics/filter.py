
import matplotlib
import matplotlib.pyplot as plt
from matplotlib import gridspec
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
import pandas as pd

from Points import * 


subfolder = 'STATEBASED/'
interval = [0,600]


#f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
f = plt.figure(num=None, figsize=(7,5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
gs = gridspec.GridSpec(5, 1, height_ratios=[10, 10, 10, 10, 2])     
gs.update(left=0.07, bottom=0.04, right=0.95, top=0.99, wspace=0.02, hspace=0.02)

filename = u"../out/" + subfolder + "channel_state.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "X"])
X = data.as_matrix()

Xpoints = Points(X[:,0], X[:,1])
Xpoints.multiply()

n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)*0.0
levelone = np.ones(n)*1.0

filename = u"../out/" + subfolder + "filter_Discrete.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_d = data.t.as_matrix()
p_d = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename = u"../out/" + subfolder + "filter_DiscreteContinuousGaussian.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
t_dcg = data.t.as_matrix()
p_dcg = data[["p0", "p1", "p2", "p3"]].as_matrix()

filename_dh = u"../out/" + subfolder + "CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/" + subfolder + "CP_obs_1.txt"
t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()

ax0 = plt.subplot(gs[0])
ax1 = plt.subplot(gs[1])
ax2 = plt.subplot(gs[2])
ax3 = plt.subplot(gs[3])
ax4 = plt.subplot(gs[4])

plots = [ax3, ax2, ax1, ax0]


ax3.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.2, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.6, linewidth=0.0);
ax0.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.7, linewidth=0.0);

    
#ax3.plot(t_dc, p0_dc, color = 'blue')
#ax2.plot(t_dc, p1_dc, color = 'blue')
#ax1.plot(t_dc, p2_dc, color = 'blue')
#ax0.plot(t_dc, p3_dc, color = 'blue')

for i in range(0, 4):
    #plots[i].set_xlim(0,max(X[:,0]))
    plots[i].set_xlim(interval[0], interval[1])
    plots[i].plot(t_d, p_d[:,i], '--', color = 'black', alpha=0.9, linewidth=1.0)
    #plots[i].plot(t_di, p_di[:,i], color = 'yellow')
    
    #plots[i].plot(t_dc, p_dc[:,i], color = 'cyan')
    #plots[i].plot(t_dmc, p_dmc[:,i], color = 'magenta')
    
    plots[i].plot(t_dcg, p_dcg[:,i], '-', color = 'black', linewidth = 1.5)

#ax4.set_xlim(0,max(X[:,0]))
ax4.set_xlim(interval[0], interval[1])

ax4.plot(dhpoints.x, dhpoints.y*2, '.', color = 'black')
ax4.plot(dlpoints.x, dlpoints.y, 'x', color = 'black')
ax4.set_ylim(0.5,2.5)
#ax4.set_axis_off()

#ax0.axes.get_yaxis().set_visible(False)
#ax1.axes.get_yaxis().set_visible(False)
#ax2.axes.get_yaxis().set_visible(False)
#ax3.axes.get_yaxis().set_visible(False)
ax4.axes.get_yaxis().set_visible(False)

plt.setp(ax0.get_xticklabels(), visible=False)
plt.setp(ax1.get_xticklabels(), visible=False)
plt.setp(ax2.get_xticklabels(), visible=False)
plt.setp(ax3.get_xticklabels(), visible=False)

##plt.savefig(u"../Output/ForIFAC.final/filter_" + str(n) + ".pdf")
plt.show()
