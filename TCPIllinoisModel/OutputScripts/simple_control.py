
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
import pandas as pd       

filename = u"../out/simple_illinois_control.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5,6,7,8,9,10,11,12), dtype=float, names = ["t", "u", "ss", "thresh", "m", "rtt", "alpha",  "d", "d_1", "d_m", "T_min", "T_max", "kappa_1"])
t_i = data.t.as_matrix()
u_i = data.u.as_matrix()
rtt_i = data.rtt.as_matrix()
alpha_i = data.alpha.as_matrix()
d_i = data.d.as_matrix()
d1_i = data.d_1.as_matrix()
dm_i = data.d_m.as_matrix()
kappa1_i = data.kappa_1.as_matrix()
#kappa2_i = data.kappa_2.as_matrix()

filename = u"../out/simple_newreno_control.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5), dtype=float, names = ["t", "u", "ss", "thresh", "m", "rtt"])
t_nr = data.t.as_matrix()
u_nr = data.u.as_matrix()



f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')
ax1 = plt.subplot(111)

plt.plot(t_i, u_i, '-', color = 'blue', label = 'illinois')
plt.plot(t_nr, u_nr, '-', color = 'red', label = 'newreno')

#ax1.plot(t_i, alpha_i, '-', color = 'green', label = 'alpha')
#ax1.plot(t_i, kappa1_i, '-', color = 'green', label = 'kappa1')
ax2 = ax1.twinx()

ax2.plot(t_i, d_i, ':', color = 'red', label = 'd')
ax2.plot(t_i, d1_i, ':', color = 'black', label = 'd_1')
ax2.plot(t_i, dm_i, ':', color = 'blue', label = 'd_m')
#ax2.plot(t_i, kappa2_i, '-', color = 'blue', label = 'kappa2')

#plt.plot(t, ss * max(u) / 2.0, '--', color = 'green')
#plt.plot(t, thresh, ':', color = 'yellow')

#ax1.set_xlim(150-10,450)
ax1.legend()
ax2.legend()
plt.show()

