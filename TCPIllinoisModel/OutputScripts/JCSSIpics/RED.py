
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


f = plt.figure(num=None, figsize=(6,4), dpi=200, facecolor='w', edgecolor='k')
gs = gridspec.GridSpec(3, 1, height_ratios=[10, 10, 10])     
gs.update(left=0.16, bottom=0.08, right=0.92, top=0.95, wspace=0.0, hspace=0.3)

ax0 = plt.subplot(gs[0])
ax1 = plt.subplot(gs[1])
ax2 = plt.subplot(gs[2])

plots = [ax2, ax1, ax0]

x1 = [10, 30, 60, 90, 90, 110]
y1 = [10, 10, 10, 80, 100, 100]

x2 = [10, 30, 110]
y2 = [10, 10, 100]

xticks = [10, 30, 60, 90, 110]
xlabels = ['$\\underline{W}$', '$B$', "$W'$", "$W''$", "$B+Q$"]

yticks0 = [10, 80, 100]
ylabels0 = ['$P_0$', '$P_1$', "$1$"]

yticks1 = [10, 100]
ylabels1 = ['$\delta_0 + \\xi_i$', '$\\delta_0 + \\xi_i + Q \\nu_i$']

yticks2 = [10, 100]
ylabels2 = ['$\\phi_i$', '$\phi_i + Q \\psi_i$']


ax0.set_yticks(yticks0);
ax0.set_yticklabels(ylabels0);

ax1.set_yticks(yticks1);
ax1.set_yticklabels(ylabels1);

ax2.set_yticks(yticks2);
ax2.set_yticklabels(ylabels2);


ax2.set_xticks(xticks);
ax2.set_xticklabels(xlabels);

ax0.plot(x1, y1, '-', color = 'black', linewidth = 1.0)
ax1.plot(x2, y2, '-', color = 'black', linewidth = 1.0)
ax2.plot(x2, y2, '-', color = 'black', linewidth = 1.0)

ax0.text(-11, 120, '$P_l(u)$')#, rotation = '90')
ax1.text(-22, 125, '$\\mathbf{E}\\{\\mathrm{RTT}\\}(u)$')#, rotation = '90')
ax2.text(-22, 125, '$\\mathbf{D}\\{\\mathrm{RTT}\\}(u)$')#, rotation = '90')

ax0.text(120, -20, '$u$')
ax1.text(120, -20, '$u$')
ax2.text(120, -20, '$u$')

xf = [10, 30, 30, 90, 90, 110]
vals = [1,   1,  2,  2,  3,   3]

n = len(xf)
o = np.zeros(n)
ones = np.ones(n)

levelzero = np.ones(n) * 0.0
levelone = np.ones(n)  * 110.0

ax0.fill_between(xf, levelzero, levelone, where=vals==o, color='black', alpha = 0.1, linewidth=0.0);
ax0.fill_between(xf, levelzero, levelone, where=vals==ones, color='black', alpha = 0.2, linewidth=0.0);
ax0.fill_between(xf, levelzero, levelone, where=vals==ones*2, color='black', alpha = 0.4, linewidth=0.0);
ax0.fill_between(xf, levelzero, levelone, where=vals==ones*3, color='black', alpha = 0.6, linewidth=0.0);

for ax in plots:
    ax.set_xlim([0,120])
    ax.set_ylim([0,120])   
    plt.setp(ax.get_xticklabels(), visible=False)
    ax.set_xticks(xticks);
    ax.grid(linestyle=":", color='black')
plt.setp(ax2.get_xticklabels(), visible=True)


arrowed_spines(f, [ax0])
arrowed_spines(f, [ax1])
arrowed_spines(f, [ax2])

plt.show()


