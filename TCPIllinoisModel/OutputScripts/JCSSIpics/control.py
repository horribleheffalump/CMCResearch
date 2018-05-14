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
import pylab
from Points import *

#subfolder = ''
subfolder = 'STATEBASED/'
interval = [0,200]
#bounds = [0,0.11]


filename = u"../out/" + subfolder + "control.txt"
t, u, ss, thresh, m, rtt = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

filename_X = u"../out/" + subfolder + "channel_state.txt"
t_X, X = np.loadtxt(filename_X, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename_dh = u"../out/" + subfolder + "CP_obs_0.txt"
t_dh, dh = np.loadtxt(filename_dh, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
filename_dl = u"../out/" + subfolder + "CP_obs_1.txt"
try:
    t_dl, dl = np.loadtxt(filename_dl, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
except:
    t_dl = [] 
    dl = []

f = plt.figure(num=None, figsize=(7,5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
gs = gridspec.GridSpec(2, 1, height_ratios=[20, 1])     
gs.update(left=0.1, bottom=0.04, right=0.95, top=0.99, wspace=0.0, hspace=0.0)

ax1 = plt.subplot(gs[0])
#ax2 = plt.subplot(gs[1])
ax3 = plt.subplot(gs[1])
#ax4 = plt.subplot(gs[2])
ax1.set_xlim(interval[0],interval[1])
#ax2.set_xlim(interval[0],interval[1])
ax3.set_xlim(interval[0],interval[1])
#ax4.set_xlim(interval[0],interval[1])


Xpoints = Points(t_X, X)
Xpoints.multiply()


n = len(Xpoints.x)
o = np.zeros(n)
ones = np.ones(n)
levelzero = np.ones(n)* -0.05*u.max()
levelone = np.ones(n)*1.05*u.max()
rttmax = np.array([0.2, rtt.max()]).min()
rttmin = 0.1
diff = rttmax - rttmin

print(rttmax, rttmin)

levelzerortt = np.ones(n)*(rttmin-0.05 * diff)
levelonertt = np.ones(n)*(rttmax+0.05*diff)


dhpoints = Points(t_dh, dh)
dhpoints.toones()

dlpoints = Points(t_dl, dl)
dlpoints.toones()

#print(Xpoints.x)
#print(Xpoints.y)

ax1.plot(t, u, '-', color = 'black', linewidth = 1.0)
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==o, color='black', alpha = 0.1, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones, color='black', alpha = 0.2, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*2, color='black', alpha = 0.4, linewidth=0.0);
#ax1.fill_between(Xpoints.x, levelzero, levelone, where=Xpoints.y==ones*3, color='black', alpha = 0.6, linewidth=0.0);

ax2=ax1.twinx()
ax2.plot(t, rtt, '--', color = 'black', alpha=0.8, linewidth=1.5)
#ax2.set_ylim(0.098, 0.22)
#ax2.plot(t, rtt, '-', color = 'black')
ax2.fill_between(Xpoints.x, levelzerortt, levelonertt, where=Xpoints.y==o, color='black', alpha = 0.1, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzerortt, levelonertt, where=Xpoints.y==ones, color='black', alpha = 0.2, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzerortt, levelonertt, where=Xpoints.y==ones*2, color='black', alpha = 0.4, linewidth=0.0);
ax2.fill_between(Xpoints.x, levelzerortt, levelonertt, where=Xpoints.y==ones*3, color='black', alpha = 0.6, linewidth=0.0);


ax2.set_ylim(rttmin,rttmax)
ax2.set_xlim(interval[0], interval[1])

ax3.plot(dhpoints.x, dhpoints.y*2.0, '.', color = 'black')
ax3.plot(dlpoints.x, dlpoints.y, 'x', color = 'black')
#ax4.plot(dlpoints.x, dlpoints.y, 'x', color = 'black')
#ax3.set_axis_off()
ax3.set_ylim(0.5,2.5)

#ax4.set_axis_off()

plt.setp(ax1.get_xticklabels(), visible=False)
plt.setp(ax2.get_xticklabels(), visible=False)
ax1.spines['bottom'].set_visible(False)
ax2.spines['bottom'].set_visible(False)
ax1.axes.get_xaxis().set_visible(False)
ax2.axes.get_xaxis().set_visible(False)

ax3.spines['top'].set_visible(False)
ax3.axes.get_yaxis().set_visible(False)


yticks = [0, 1250, 1350]
ylabels = ['0','$B$', "$B+W''$"]
ax1.set_yticks(yticks);
ax1.set_yticklabels(ylabels);
#ax1.set_ylabel('окно перегрузки, \# пакетов')
ax1.yaxis.set_label_coords(-0.06,0.45)

yticks2 = [0.1, np.round(rttmax * 100)/100]
ax2.set_yticks(yticks2);
#ax2.text(205, rttmin + diff*0.73, 'Время кругового обращения, с', rotation= -90)


ax3.set_xlim([0,200])
xticks = [0, 100, 200]
xlabels = ['0', '', '200']
ax3.set_xticks(xticks);
ax3.set_xticklabels(xlabels);
#ax3.set_xlabel('время, с')
ax3.xaxis.set_label_coords(0.5,-0.4)


plt.show()



