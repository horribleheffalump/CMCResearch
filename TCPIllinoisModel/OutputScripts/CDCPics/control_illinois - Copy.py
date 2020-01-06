import matplotlib
import matplotlib.pyplot as plt
from matplotlib import gridspec
import numpy as np
from matplotlib import rc
rc('font',**{'family':'serif'})
rc('text', usetex=True)
rc('text.latex',unicode=True)
rc('text.latex',preamble=r'\usepackage[T2A]{fontenc}')
rc('text.latex',preamble=r'\usepackage[utf8]{inputenc}')
rc('text.latex',preamble=r'\usepackage[russian]{babel}')
rc('text.latex',preamble=r'\usepackage{amssymb}')
import pylab
import pandas as pd
from Points import *
#from arrowed_spines import *

#folder = "D:/projects.git/CMCResearch/TCPIllinoisModel/out_for_CDC/ILLINOIS_0,3_10_0,125_0,5_ff0c3fc1-b378-4449-8c02-983a531a88bb/"
#folder = "D:/projects.git/CMCResearch/TCPIllinoisModel/out_for_CDC/ILLINOIS_0,3_10_0,125_0,5_72938458-b6c2-4674-9c24-76f17e527979/"
folder = "D:/projects.git/CMCResearch/TCPIllinoisModel/out_for_CDC/ILLINOIS_STANDARD_SAMPLE/"
start = 1000 #2030
end = 1180 # 2210

interval = [start,end]
bounds = [0,1450]


filename = folder + "control.txt"
t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_X = folder + "channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = folder + "CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = folder + "CP_obs_1.txt"
try:
    t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
except:
    t_dl = [] 
    dl = []

#filename_f = folder + "filter_DiscreteContinuousGaussian.txt"
#data = pd.read_csv(filename_f, delimiter = " ", header=None, usecols=(0,1,2,3,4), dtype=float, names = ["t", "p0", "p1", "p2", "p3"])
#t_dcg = data.t.as_matrix()
#p_dcg = data[["p0", "p1", "p2", "p3"]].as_matrix()

f = plt.figure(num=None, figsize=(5,5), dpi=150, facecolor='w', edgecolor='k')
gs = gridspec.GridSpec(2, 2, width_ratios=[20,1], height_ratios=[1,1])     
gs.update(left=0.13, bottom=0.05, right=0.97, top=0.95, wspace=0.01, hspace=0.01)

ax1 = plt.subplot(gs[0])
ax2 = plt.subplot(gs[2])
ax1l = plt.subplot(gs[1])
#ax2l = plt.subplot(gs[3])

Xpoints = Points(t_X, X)
Xpoints.multiply()


n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.zeros(n)
levelone = np.ones(n)
levelevents = 0.110

dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()


ax1.plot(t, rtt, '-', color = 'red', alpha = 0.6, linewidth=1.5)
ax1.set_xticks([]);

ax1p = ax1.twinx()
ax1p.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='white', alpha = 0.3, linewidth=0.0);
ax1p.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='green', alpha = 0.3, linewidth=0.0);
ax1p.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='red', alpha = 0.3, linewidth=0.0);
ax1p.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.3, linewidth=0.0);
ax1p.set_ylim(0,1)
ax1p.set_xticks([]);

ax1l.fill_between([0,1], 0, 1/4, color='white', alpha = 0.3, linewidth=0.0);
ax1l.fill_between([0,1], 1/4, 1/2, color='green', alpha = 0.3, linewidth=0.0);
ax1l.fill_between([0,1], 1/2, 3/4, color='red', alpha = 0.3, linewidth=0.0);
ax1l.fill_between([0,1], 3/4, 1, color='black', alpha = 0.3, linewidth=0.0);
ax1l.text(0.05, 0.25 - 0.06, '$\mathbf{X}_t = e_1$', rotation=-90)
ax1l.text(0.05, 0.5  - 0.06, '$\mathbf{X}_t = e_2$', rotation=-90)
ax1l.text(0.05, 0.75 - 0.06, '$\mathbf{X}_t = e_3$', rotation=-90)
ax1l.text(0.05, 1.0  - 0.06, '$\mathbf{X}_t = e_4$', rotation=-90)

#ax2l.fill_between([0,1], 0, 1/4, color='white', alpha = 0.3, linewidth=0.0);
#ax2l.fill_between([0,1], 1/4, 1/2, color='green', alpha = 0.3, linewidth=0.0);
#ax2l.fill_between([0,1], 1/2, 3/4, color='red', alpha = 0.3, linewidth=0.0);
#ax2l.fill_between([0,1], 3/4, 1, color='black', alpha = 0.3, linewidth=0.0);
#ax2l.text(-0.01, 0.25 - 0.12, '$e_1^{\\top}\hat{\mathbf{X}_t}$', rotation=-90)
#ax2l.text(-0.01, 0.5  - 0.12, '$e_2^{\\top}\hat{\mathbf{X}_t}$', rotation=-90)
#ax2l.text(-0.01, 0.75 - 0.12, '$e_3^{\\top}\hat{\mathbf{X}_t}$', rotation=-90)
#ax2l.text(-0.01, 1.0  - 0.12, '$e_4^{\\top}\hat{\mathbf{X}_t}$', rotation=-90)

ax1l.set_xticks([]);
ax1l.set_yticks([]);
#ax2l.set_xticks([]);
#ax2l.set_yticks([]);
ax1l.set_ylim(0,1)
#ax2l.set_ylim(0,1)
ax1l.set_xlim(0,1)
#ax2l.set_xlim(0,1)

ax1.plot(dhpoints.x, dhpoints.y*levelevents, 'v', color = 'black', markersize=3, label = 'Losses')
ax1.plot(dlpoints.x, dlpoints.y*levelevents, 'x', color = 'red', label = 'Time-outs')


ax2.plot(t, u, '-', color = 'black', linewidth = 1.5)

ax2p = ax2.twinx()
#ax2p.stackplot(t_dcg,  p_dcg[:,0],  p_dcg[:,1],  p_dcg[:,2],  p_dcg[:,3], colors=['white', 'green', 'red', 'black'], alpha=0.3)
#ax2p.set_ylim(0,1)

ax1.set_xlim(interval)
ax2.set_xlim(interval)
ax1p.set_xlim(interval)
ax2p.set_xlim(interval)

ax1p.set_yticks([]);
ax2p.set_yticks([]);

#ax1.set_ylim(0.0995, 0.1105)
yticks1 = [0.1, 0.109]
ax1.set_yticks(yticks1);
ax1.text(start-10.0, 0.107, r'Smoothed RTT $r_t$', rotation=90)
#ax1.legend()
ax1.legend(bbox_to_anchor=(0.22,1.01,0.5,0.2), loc="lower left", borderaxespad=0, ncol=2, frameon=False)

xticks = [start+0, start+60, start+120, start+180]
xlabels = ['0', '1 min', '2 min', '3 min']

ax2.set_xticks(xticks);
#ax1.set_yticks([]);
ax2.set_xticklabels(xlabels);
#ax2.text(188, -20, '$t$')

yticks2 = [0, 1250, 1350]
ylabels2 = ['0','$B$', "$B+W''$"]

ax2.set_yticks(yticks2);
ax2.set_yticklabels(ylabels2);
ax2.yaxis.set_label_coords(-0.02, 0.45)
ax2.text(start-10.0, 1000.0, 'Illinoise control $U_t$', rotation=90)
plt.show()



