import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab

f = plt.figure(num=None, figsize=(7,6), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)

ax0 = plt.subplot(311)
ax1 = plt.subplot(312)
ax2 = plt.subplot(313)
#ax4 = plt.subplot(414)

ax = [ax0, ax1, ax2]

for n in range(0,3):   
    filename = u"../Output/ForIFAC/MCTrajectory_" + str(n) + ".txt"
    t, X = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
    tplot = np.zeros(len(t)*2-1)
    Xplot = np.zeros(len(t)*2-1)
    for i in range(0, len(t)-1):
        tplot[i*2] = t[i]
        Xplot[i*2] = X[i]
        tplot[i*2+1] = t[i+1]
        Xplot[i*2+1] = X[i]
    tplot[len(t)*2-2] = t[len(t)-1]
    Xplot[len(t)*2-2] = X[len(t)-1]
    o = np.zeros(len(tplot))
    ones = np.ones(len(tplot))
    levelzero = np.ones(len(tplot))*0.0
    levelone = np.ones(len(tplot))*1.0

    filename = u"../Output/ForIFAC/Filter_" + str(n) + ".txt"
    t, p0, p1, p2, u = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4), unpack=True, dtype=float)

    Utplot = np.zeros(len(t)*2-1)
    Uplot = np.zeros(len(t)*2-1)
    for i in range(0, len(t)-1):
        Utplot[i*2] = t[i]
        Uplot[i*2] = u[i]
        Utplot[i*2+1] = t[i+1]
        Uplot[i*2+1] = u[i]
    Utplot[len(t)*2-2] = t[len(t)-1]
    Uplot[len(t)*2-2] = u[len(t)-1]
    Uo = np.zeros(len(Utplot))
    Uones = np.ones(len(Utplot))*0.8
    Ulevelzero = np.ones(len(Utplot))*0.0
    Ulevelone = np.ones(len(Utplot))*1.0





    #ax2.fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
    #ax1.fill_between(tplot, levelzero, levelone, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
    ax[n].fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);

    
    ax[n].plot(t, p0, color = 'black')
    #ax1.plot(t, p1, color = 'blue')
    #ax0.plot(t, p2, color = 'blue')
    
    
    #ax4.plot(t, u*0.3 + 0.03 + 0.33 *n, color='black')

    #ax4.fill_between(Utplot, Ulevelzero, Ulevelone, where=Uplot>Uones, color='black', alpha = 0.5, linewidth=0.0);
    #ax3.plot(t, u, color = 'red')

ax0.set_xlim(0,600)
ax1.set_xlim(0,600)
ax2.set_xlim(0,600)
ax0.axes.get_yaxis().set_visible(False)
ax1.axes.get_yaxis().set_visible(False)
ax2.axes.get_yaxis().set_visible(False)

plt.savefig(u"../Output/ForIFAC/fig_filter.pdf")
