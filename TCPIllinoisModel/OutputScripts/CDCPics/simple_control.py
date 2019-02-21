
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



filename = u"../out_for_JCSS/simple_illinois_control.txt"
data_i = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_i"], engine='python')

filename = u"../out_for_JCSS/simple_newreno_control.txt"
data_nr = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_nr"], engine='python')

#filename = u"D:/Наука/projects.git.vs2017/CMCResearch/TCPIllinoisModel/out/simple_statebased_control.txt"
filename = u"../out_for_JCSS/simple_statebased_control.txt"
data_sb = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_sb"], engine='python')

data = pd.merge(data_i, data_nr, left_on = 't', right_on = 't')
data = pd.merge(data, data_sb, left_on = 't', right_on = 't')

t_dh_nr = [125.95, 193.90]
dh_nr = [0.5, 0.5]

t_dh_sb = [116.05]
dh_sb = [0.5]

print(data.head())

#f = plt.figure(num=None, figsize=(7, 3.5), dpi=150, facecolor='w', edgecolor='k')
f = plt.figure(num=None, figsize=(5,2.5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.1, bottom=0.12, right=0.96, top=0.95, wspace=0.1)

ax1 = plt.gca()

#ax1 = plt.subplot(111)

levelzero = np.ones(data.t.size)*0.0
levelone = np.ones(data.t.size)*1.0


ax1p = ax1.twinx()
ax1p.set_yticks([]);
ax1p.set_yticks([]);
ax1p.set_xlim([0,180])
ax1p.set_ylim([0,1])

ax1p.fill_between(data.t, levelzero, levelone, where=data.u_i<1250, color='white', alpha = 0.3, linewidth=0.0);
ax1p.fill_between(data.t, levelzero, levelone, where=(data.u_i>=1250)*(data.u_i<1330), color='green', alpha = 0.3, linewidth=0.0);
ax1p.fill_between(data.t, levelzero, levelone, where=(data.u_i>1330), color='red', alpha = 0.3, linewidth=0.0);

ax1.plot(data.t, data.u_i, '-', color = 'black') #, label = 'illinois')

ax1.set_xlim([0,180])
ax1.set_ylim([0,1450])


xticks = [0, 60, 120, 180]
xlabels = ['0', '1 min', '2 min', '3 min']

ax1.set_xticks(xticks);
ax1.set_xticklabels(xlabels);

yticks = [0, 1250, 1350]
ylabels = ['0','$B$', "$B+W''$"]

ax1.set_yticks(yticks);
ax1.set_yticklabels(ylabels);
ax1.set_ylabel('Illinoise control $U_t$')
ax1.yaxis.set_label_coords(-0.02,0.45)





plt.show()

