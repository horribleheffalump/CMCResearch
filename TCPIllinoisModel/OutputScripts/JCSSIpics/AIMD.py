
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
from arrowed_spines import *

interval = [0,205]
bounds = [0,1450]



t_dh = np.array([100.0, 130.0]) 
dh = np.array([1.0, 1.0])

t_dl = np.array([160.0])
dl = np.array([1.0])

f = plt.figure(num=None, figsize=(6,4), dpi=200, facecolor='w', edgecolor='k')
gs = gridspec.GridSpec(2, 1, height_ratios=[20, 1])     
gs.update(left=0.13, bottom=0.08, right=0.92, top=0.95, wspace=0.0, hspace=0.0)

ax1 = plt.subplot(gs[0])
ax3 = plt.subplot(gs[1])


t_ss1 = np.arange(0.0, 20.0, 0.1) 
u_ss1 = 5.0 + t_ss1 * t_ss1 * t_ss1 / 40.0

t_ca1 = np.arange(20.0, 100.0, 0.1)
u_ca1 = 205.0 + (t_ca1-20.0) * 1150.0 / 80.0

t_ca2 = np.arange(100.0, 130.0, 0.1)
u_ca2 = 600.0 + (t_ca2-100.0) * 1150.0 / 80.0

t_ca3 = np.arange(130.0, 160.0, 0.1)
u_ca3 = 500.0 + (t_ca3-130.0) * 1150.0 / 80.0

t_ss2 = np.arange(160.0, 180.0, 0.1) 
u_ss2 = 5.0 + t_ss1 * t_ss1 * t_ss1 / 40.0

t_ca4 = np.arange(180.0, 200.0, 0.1)
u_ca4 = 205.0 + (t_ca4-180.0) * 1150.0 / 80.0

t = np.hstack((t_ss1, t_ca1, t_ca2, t_ca3, t_ss2, t_ca4))
u = np.hstack((u_ss1, u_ca1, u_ca2, u_ca3, u_ss2, u_ca4))




ax1.plot(t, u, '-', color = 'black', linewidth = 1.0)

ax3.plot(t_dh, dh*1.5, '.', color = 'black')
ax3.plot(t_dl, dl*0.7, 'x', color = 'black')


ax1.set_xlim(interval)
ax1.set_ylim(bounds)


ax3.set_xlim(interval)
ax3.set_ylim(0,2.0)





plt.setp(ax1.get_xticklabels(), visible=False)



xticks = [20.0, 95.0, 100.0, 105.0, 130.0, 160.0, 180.0]

x =      [0.0, 20.0, 20.0, 95.0, 95.0, 100.0, 100.0, 105.0, 105.0, 130.0, 130.0, 160.0, 160.0, 180.0, 180.0, 200.0]
vals =   [2,  2,  1,  1,  3,   3,   3,   3,   1,   1,   1,   1,   2,   2,   1,   1]

xlabels = ['','','','','','','','']





n = len(x)
o = np.zeros(n)
ones = np.ones(n)

levelzero = np.ones(n)* 0.0
levelone = np.ones(n) * 1450.0


ax3.set_xticks([]);
ax3.set_yticks([]);
ax3.set_xticklabels(xlabels);
ax3.set_xlabel('время, с')
ax3.xaxis.set_label_coords(0.5,-0.4)
ax3.text(208, -2.4, '$t$')

yticks = [5, 205, 1250, 1350]
ylabels = ['$\\underline{W}$', '$W^{th}$', '$B$', "$B+Q$"]

ax1.set_xticks(xticks);
ax1.set_yticks(yticks);
ax1.set_yticklabels(ylabels);
ax1.set_ylabel('окно перегрузки, \# пакетов')
ax1.yaxis.set_label_coords(-0.02, 0.50)
ax1.text(-9.0, 1450.0, '$U_t$')

ax1.fill_between(x, levelzero, levelone, where=vals==o, color='black', alpha = 0.1, linewidth=0.0);
ax1.fill_between(x, levelzero, levelone, where=vals==ones, color='black', alpha = 0.2, linewidth=0.0);
ax1.fill_between(x, levelzero, levelone, where=vals==ones*2, color='black', alpha = 0.4, linewidth=0.0);
ax1.fill_between(x, levelzero, levelone, where=vals==ones*3, color='black', alpha = 0.6, linewidth=0.0);

ax1.text(25, 80, '$U_t = \\underline{W} 2^t$')
ax1.text(30, 250, '$\\mathrm{tg}(\\alpha) = \\overline{RTT}^{-1}$')
#ax1.text(114, 400, '$U_t = \\frac{1}{2} U_{t-}$')
ax1.text(120, 400, '$U_t = \\frac{1}{2} U_{t-}$')

#ax1.text(1, 1405, 'Slow', fontsize=7)
#ax1.text(1, 1365, 'start', fontsize=7)

#ax1.text(21, 1405, 'Congestion', fontsize=7)
#ax1.text(21, 1365, 'avoidance', fontsize=7)

#ax1.text(96, 1505, 'Congestion', fontsize=7)
#ax1.text(96, 1465, 'fact', fontsize=7)

#ax1.text(106, 1405, 'Congestion', fontsize=7)
#ax1.text(106, 1365, 'avoidance', fontsize=7)

#ax1.text(161, 1405, 'Slow', fontsize=7)
#ax1.text(161, 1365, 'start', fontsize=7)

#ax1.text(181, 1405, 'Congestion', fontsize=7)
#ax1.text(181, 1365, 'avoidance', fontsize=7)

ax1.text(5.5, 1405, 'Slow', fontsize=7)
ax1.text(5.25, 1365, 'start', fontsize=7)

ax1.text(45, 1405, 'Congestion', fontsize=7)
ax1.text(46.5, 1365, 'avoidance', fontsize=7)

ax1.text(90, 1505, 'Congestion', fontsize=7)
ax1.text(96.75, 1465, 'fact', fontsize=7)

ax1.text(121, 1405, 'Congestion', fontsize=7)
ax1.text(122.5, 1365, 'avoidance', fontsize=7)

ax1.text(165.5, 1405, 'Slow', fontsize=7)
ax1.text(165.25, 1365, 'start', fontsize=7)

#ax1.text(181.5, 1405, 'Congestion', fontsize=7)
#ax1.text(183, 1365, 'avoidance', fontsize=7)

arrowed_spines(f, [ax1, ax3])
ax1.grid(linestyle=":", color='black')
plt.show()



