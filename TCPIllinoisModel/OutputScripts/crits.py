import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
from Points import *
       
import pandas as pd


filename = u"../out/ILLINOIS/control.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,2,3,4,5), dtype=float, names = ["t", "u", "ss", "thresh", "m", "rtt"])
t = data.t.as_matrix()
rtt = data.rtt.as_matrix()
u = data.u.as_matrix()

filename = u"../out/ILLINOIS/crit_Throughput.txt"
data = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1,3,4), dtype=float, names = ["t", "throughput", "u", "rtt"])
tc = data.t.as_matrix()
throughput = data.throughput.as_matrix()
rttc = data.rtt.as_matrix()
uc = data.u.as_matrix()


ax1 = plt.subplot(111)
ax1.plot(t, rtt, '-', color = 'yellow', alpha = 0.4, linewidth = 3.0, label = "rtt")


ax2 = ax1.twinx()
#ax2.plot(t, u, '-', color = 'green', alpha = 0.4, linewidth = 3.0, label = "u")
ax2.plot(t, throughput, '-', color = 'red', alpha = 0.4, linewidth = 3.0, label = "throughput")

#ax1.plot(t, I0[:,1], '-', color = 'green', alpha = 0.6, linewidth = 2.0, label = "sigma: 1-3")
#ax1.plot(t, I0[:,2], '-', color = 'cyan', alpha = 1, linewidth = 1.0, label = "sigma: 1-2")
#ax1.plot(t, A[:,0], ':', color = 'yellow', alpha = 1.0, linewidth = 3.0, label = "A[0-3]")
#ax1.plot(t, A[:,1], ':', color = 'green', alpha = 1.0, linewidth = 2.0, label = "A[1-3]")
#ax1.plot(t, A[:,2], ':', color = 'cyan', alpha = 1.0, linewidth = 1.0, label = "A[1-2]")


ax1.legend()
ax2.legend()


plt.show()
