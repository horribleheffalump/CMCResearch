
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
import pandas as pd       

filename = u"../out/simple_illinois_control.txt"
data_i = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5,6,7,8,9,10,11,12), dtype=float, names = ["t", "u", "ss", "thresh", "m", "rtt", "alpha",  "d", "d_1", "d_m", "T_min", "T_max", "kappa_1"])

filename = u"../out/simple_newreno_control.txt"
data_nr = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5), dtype=float, names = ["t", "u", "ss", "thresh", "m", "rtt"])



f = plt.figure(num=None, figsize=(10, 6), dpi=150, facecolor='w', edgecolor='k')
ax1 = plt.subplot(111)

plt.plot(data_i.t, data_i.u, '-', color = 'blue', label = 'illinois')
plt.plot(data_nr.t, data_nr.u, '-', color = 'red', label = 'newreno')

ax2 = ax1.twinx()

#ax2.plot(data_i.t, data_i.d, ':', color = 'red', label = 'd')
#ax2.plot(data_i.t, data_i.d_1, ':', color = 'black', label = 'd_1')
#ax2.plot(data_i.t, data_i.d_m, ':', color = 'blue', label = 'd_m')


ax1.legend()
ax2.legend()
plt.show()

