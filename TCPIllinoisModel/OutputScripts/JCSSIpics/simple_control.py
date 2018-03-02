
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
#from Points import *
import pandas as pd       

filename = u"../out/simple_illinois_control.txt"
data_i = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_i"])

filename = u"../out/simple_newreno_control.txt"
data_nr = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_nr"])

#filename = u"D:/Наука/projects.git.vs2017/CMCResearch/TCPIllinoisModel/out/simple_statebased_control.txt"
filename = u"../out/simple_statebased_control.txt"
data_sb = pd.read_csv(filename, delimiter = " ", header=None, usecols=(0,1), dtype=float, names = ["t", "u_sb"])

data = pd.merge(data_i, data_nr, left_on = 't', right_on = 't')
data = pd.merge(data, data_sb, left_on = 't', right_on = 't')

f = plt.figure(num=None, figsize=(7, 3), dpi=150, facecolor='w', edgecolor='k')
#ax1 = plt.subplot(111)

levelzero = np.ones(data.t.size)*0.0
levelone = np.ones(data.t.size)*data.u_sb.max()



#plt.plot(data.t, data.u_i, '-', color = 'blue', label = 'illinois')

plt.plot(data.t, data.u_sb, '-', color = 'black') #, label = 'statebased')
plt.fill_between(data.t, levelzero, levelone, where=data.u_sb<1250, color='black', alpha = 0.2, linewidth=0.0);
plt.fill_between(data.t, levelzero, levelone, where=(data.u_sb>=1250)*(data.u_sb<1330), color='black', alpha = 0.4, linewidth=0.0);
plt.fill_between(data.t, levelzero, levelone, where=(data.u_sb>1330), color='black', alpha = 0.6, linewidth=0.0);

#plt.plot(data.t, data.u_nr, '-', color = 'black') #, label = 'newreno')
#plt.fill_between(data.t, levelzero, levelone, where=data.u_nr<1250, color='black', alpha = 0.2, linewidth=0.0);
#plt.fill_between(data.t, levelzero, levelone, where=(data.u_nr>=1250)*(data.u_nr<1330), color='black', alpha = 0.4, linewidth=0.0);
#plt.fill_between(data.t, levelzero, levelone, where=(data.u_nr>1330), color='black', alpha = 0.6, linewidth=0.0);

#ax2 = ax1.twinx()

#ax2.plot(data_i.t, data_i.d, ':', color = 'red', label = 'd')
#ax2.plot(data_i.t, data_i.d_1, ':', color = 'black', label = 'd_1')
#ax2.plot(data_i.t, data_i.d_m, ':', color = 'blue', label = 'd_m')


#ax1.legend()
#ax2.legend()

plt.xlim([0,200])
plt.show()

