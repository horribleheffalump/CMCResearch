
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

import pandas as pd

from Points import * 
from arrowed_spines import *


f = plt.figure(num=None, figsize=(6,3), dpi=200, facecolor='w', edgecolor='k')
gs = gridspec.GridSpec(1, 1)     
gs.update(left=0.16, bottom=0.08, right=0.92, top=0.95, wspace=0.0, hspace=0.3)

ax0 = plt.subplot(gs[0])

x1 = [10, 30, 60, 90, 90, 110]
y1 = [10, 10, 10, 80, 100, 100]

xticks = [10, 30, 60, 90, 110]
xlabels = ['$\\underline{W}$', '$B$', "$W'$", "$W''$", "$B+Q$"]

yticks0 = [10, 80, 100]
ylabels0 = ['$P_0$', '$P_1$', "$1$"]


ax0.set_yticks(yticks0);
ax0.set_yticklabels(ylabels0);

ax0.set_xticks(xticks);
ax0.set_xticklabels(xlabels);

ax0.plot(x1, y1, '-', color = 'black', linewidth = 1.0)

ax0.text(-11, 115, '$P_l(u)$')#, rotation = '90')

ax0.text(115, -20, '$u$')

xf = [10, 30, 30, 90, 90, 110]
vals = [1, 1, 2, 2, 3, 3]

n = len(xf)
o = np.zeros(n)
ones = np.ones(n)

levelzero = np.ones(n) * 0.0
levelone = np.ones(n)  * 150.0

ax0.fill_between(xf, levelzero, levelone, where=vals==ones, color='white', alpha = 0.3, linewidth=0.0);
ax0.fill_between(xf, levelzero, levelone, where=vals==ones*2, color='green', alpha = 0.3, linewidth=0.0);
ax0.fill_between(xf, levelzero, levelone, where=vals==ones*3, color='red', alpha = 0.3, linewidth=0.0);

ax0.set_xlim([0,120])
ax0.set_ylim([0,115])   
ax0.set_xticks(xticks);
ax0.grid(linestyle=":", color='black')
#plt.setp(ax2.get_xticklabels(), visible=True)


arrowed_spines(f, [ax0])

plt.show()


