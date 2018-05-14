
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
#from Points import *
import pandas as pd    
from subplot import *
from arrowed_spines import *


filename = u"../out/simple_illinois_control.txt"
data_i = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_i"], engine='python')

filename = u"../out/simple_newreno_control.txt"
data_nr = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_nr"], engine='python')

#filename = u"D:/Наука/projects.git.vs2017/CMCResearch/TCPIllinoisModel/out/simple_statebased_control.txt"
filename = u"../out/simple_statebased_control.txt"
data_sb = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_sb"], engine='python')

data = pd.merge(data_i, data_nr, left_on = 't', right_on = 't')
data = pd.merge(data, data_sb, left_on = 't', right_on = 't')

t_dh_nr = [125.95, 193.90]
dh_nr = [0.5, 0.5]

t_dh_sb = [116.05]
dh_sb = [0.5]

print(data.head())

f = plt.figure(num=None, figsize=(7, 3.5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.1, bottom=0.12, right=0.96, top=0.95, wspace=0.1)

gs = gridspec.GridSpec(2, 1, height_ratios=[20, 1])     
gs.update(left=0.1, bottom=0.08, right=0.96, top=0.95, wspace=0.0, hspace=0.0)
ax1 = plt.subplot(gs[0])
ax3 = plt.subplot(gs[1])

#ax1 = plt.subplot(111)

levelzero = np.ones(data.t.size)*0.0
levelone = np.ones(data.t.size)*data.u_sb.max()



#plt.plot(data.t, data.u_i, '-', color = 'blue', label = 'illinois')

ax1.plot(data.t, data.u_sb, '-', color = 'black') #, label = 'statebased')
ax1.fill_between(data.t, levelzero, levelone, where=data.u_sb<1250, color='black', alpha = 0.1, linewidth=0.0);
ax1.fill_between(data.t, levelzero, levelone, where=(data.u_sb>=1250)*(data.u_sb<1330), color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(data.t, levelzero, levelone, where=(data.u_sb>1330), color='black', alpha = 0.4, linewidth=0.0);
ax3.plot(t_dh_sb, dh_sb, '.', color = 'black')

#ax1.plot(data.t, data.u_nr, '-', color = 'black') #, label = 'newreno')
#ax1.fill_between(data.t, levelzero, levelone, where=data.u_nr<1250, color='black', alpha = 0.1, linewidth=0.0);
#ax1.fill_between(data.t, levelzero, levelone, where=(data.u_nr>=1250)*(data.u_nr<1330), color='black', alpha = 0.2, linewidth=0.0);
#ax1.fill_between(data.t, levelzero, levelone, where=(data.u_nr>1330), color='black', alpha = 0.4, linewidth=0.0);
#ax3.plot(t_dh_nr, dh_nr, '.', color = 'black')


#ax2 = ax1.twinx()

#ax2.plot(data_i.t, data_i.d, ':', color = 'red', label = 'd')
#ax2.plot(data_i.t, data_i.d_1, ':', color = 'black', label = 'd_1')
#ax2.plot(data_i.t, data_i.d_m, ':', color = 'blue', label = 'd_m')


#ax1.legend()
#ax2.legend()

ax1.set_xlim([0,205])
ax1.set_ylim([0,1450])
ax3.set_xlim([0,205])
ax3.set_ylim([0.0,1.0])

xticks = [0, 100, 200]
xlabels = ['0', '', '200']

ax3.set_xticks(xticks);
ax3.set_yticks([]);
ax3.set_xticklabels(xlabels);
ax3.set_xlabel('время, с')
ax3.xaxis.set_label_coords(0.5,-0.6)
ax3.text(208, -1.4, '$t$')

yticks = [0, 1250, 1350]
ylabels = ['','$B$', "$B+W''$"]

ax1.set_xticks([]);
ax1.set_yticks(yticks);
ax1.set_yticklabels(ylabels);
ax1.set_ylabel('окно перегрузки, \# пакетов')
ax1.yaxis.set_label_coords(-0.02,0.45)
ax1.text(-8.0, 1450.0, '$U_t$')

#plt.setp(ax1.get_xticklabels(), visible=False)
#ax1.spines['bottom'].set_visible(False)
#ax1.axes.get_xaxis().set_visible(False)



box_x1 = 0
box_x2 = 0.6
box_y1 = 0
box_y2 = 150

#subax = add_subplot_axes(ax1, [0.05, 0.5, .1, .4])
#subax.plot(data.t, data.u_nr, '-', color = 'black')
#subax.set_xlim([0,0.5]);
#subax.set_ylim([0,150]);
#subax.set_xticks([0,0.5])
#subax.set_yticks([])

arrowed_spines(f, ax3, ax1)


plt.show()

