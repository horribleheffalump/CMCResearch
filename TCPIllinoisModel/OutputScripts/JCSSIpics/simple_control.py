
import matplotlib
import matplotlib.pyplot as plt
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

def add_subplot_axes(ax,rect,axisbg='w'):
    fig = plt.gcf()
    box = ax.get_position()
    width = box.width
    height = box.height
    inax_position  = ax.transAxes.transform(rect[0:2])
    transFigure = fig.transFigure.inverted()
    infig_position = transFigure.transform(inax_position)    
    x = infig_position[0]
    y = infig_position[1]
    width *= rect[2]
    height *= rect[3]  # <= Typo was here
    subax = fig.add_axes([x,y,width,height],axisbg=axisbg)
    #x_labelsize = subax.get_xticklabels()[0].get_size()
    #y_labelsize = subax.get_yticklabels()[0].get_size()
    #x_labelsize *= rect[2]**0.5
    #y_labelsize *= rect[3]**0.5
    #subax.xaxis.set_tick_params(labelsize=x_labelsize)
    #subax.yaxis.set_tick_params(labelsize=y_labelsize)
    return subax

filename = u"../out/simple_illinois_control.txt"
data_i = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_i"], engine='python')

filename = u"../out/simple_newreno_control.txt"
data_nr = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_nr"], engine='python')

#filename = u"D:/Наука/projects.git.vs2017/CMCResearch/TCPIllinoisModel/out/simple_statebased_control.txt"
filename = u"../out/simple_statebased_control.txt"
data_sb = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_sb"], engine='python')

data = pd.merge(data_i, data_nr, left_on = 't', right_on = 't')
data = pd.merge(data, data_sb, left_on = 't', right_on = 't')

print(data.head())

f = plt.figure(num=None, figsize=(7, 3.5), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.1, bottom=0.12, right=0.98, top=0.95, wspace=0.1)

ax1 = plt.subplot(111)

levelzero = np.ones(data.t.size)*0.0
levelone = np.ones(data.t.size)*data.u_sb.max()



#plt.plot(data.t, data.u_i, '-', color = 'blue', label = 'illinois')

#plt.plot(data.t, data.u_sb, '-', color = 'black') #, label = 'statebased')
#plt.fill_between(data.t, levelzero, levelone, where=data.u_sb<1250, color='black', alpha = 0.1, linewidth=0.0);
#plt.fill_between(data.t, levelzero, levelone, where=(data.u_sb>=1250)*(data.u_sb<1330), color='black', alpha = 0.2, linewidth=0.0);
#plt.fill_between(data.t, levelzero, levelone, where=(data.u_sb>1330), color='black', alpha = 0.4, linewidth=0.0);

ax1.plot(data.t, data.u_nr, '-', color = 'black') #, label = 'newreno')
ax1.fill_between(data.t, levelzero, levelone, where=data.u_nr<1250, color='black', alpha = 0.1, linewidth=0.0);
ax1.fill_between(data.t, levelzero, levelone, where=(data.u_nr>=1250)*(data.u_nr<1330), color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(data.t, levelzero, levelone, where=(data.u_nr>1330), color='black', alpha = 0.4, linewidth=0.0);

#ax2 = ax1.twinx()

#ax2.plot(data_i.t, data_i.d, ':', color = 'red', label = 'd')
#ax2.plot(data_i.t, data_i.d_1, ':', color = 'black', label = 'd_1')
#ax2.plot(data_i.t, data_i.d_m, ':', color = 'blue', label = 'd_m')


#ax1.legend()
#ax2.legend()

ax1.set_xlim([0,200])
xticks = [0, 100, 200]
yticks = [0, 1250, 1350]
xlabels = ['0', '', '200']
ylabels = ['','$B$', "$B+W''$"]
ax1.set_xticks(xticks);
ax1.set_yticks(yticks);
ax1.set_xticklabels(xlabels);
ax1.set_yticklabels(ylabels);
ax1.set_xlabel('время, с')
ax1.set_ylabel('окно перегрузки, \# пакетов')
ax1.yaxis.set_label_coords(-0.02,0.45)
ax1.xaxis.set_label_coords(0.5,-0.02)

box_x1 = 0
box_x2 = 0.6
box_y1 = 0
box_y2 = 150

subax = add_subplot_axes(ax1, [0.05, 0.5, .1, .4])
subax.plot(data.t, data.u_nr, '-', color = 'black')
subax.set_xlim([0,0.5]);
subax.set_ylim([0,150]);
subax.set_xticks([0,0.5])
subax.set_yticks([])




plt.show()

