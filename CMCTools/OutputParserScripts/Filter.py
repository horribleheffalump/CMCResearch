import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab

for n in range(0,3):
    f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
    plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
    
    filename = u"../Output/MCTrajectory_" + str(n) + ".txt"
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

    ax0 = plt.subplot(311)
    ax1 = plt.subplot(312)
    ax2 = plt.subplot(313)


    ax2.fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
    ax1.fill_between(tplot, levelzero, levelone, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
    ax0.fill_between(tplot, levelzero, levelone, where=Xplot==ones*2, color='black', alpha = 0.8, linewidth=0.0);

    filename = u"../Output/Filter_" + str(n) + ".txt"
    t, p0, p1, p2 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3), unpack=True, dtype=float)
    
    ax2.plot(t, p0, color = 'blue')
    ax1.plot(t, p1, color = 'blue')
    ax0.plot(t, p2, color = 'blue')


    ax0.set_xlim(0,600)
    ax1.set_xlim(0,600)
    ax2.set_xlim(0,600)
    plt.savefig(u"../Output/filter_" + str(n) + ".pdf")
    #plt.show()
