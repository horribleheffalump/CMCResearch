import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab




for n in range(0,3):
    f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')
    plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
    
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



    ax0 = plt.subplot(411)
    ax1 = plt.subplot(412)
    ax2 = plt.subplot(413)
    ax3 = plt.subplot(414)


    ax2.fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
    ax1.fill_between(tplot, levelzero, levelone, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
    ax0.fill_between(tplot, levelzero, levelone, where=Xplot==ones*2, color='black', alpha = 0.8, linewidth=0.0);

    
    ax2.plot(t, p0, color = 'blue')
    ax1.plot(t, p1, color = 'blue')
    ax0.plot(t, p2, color = 'blue')
    ax3.fill_between(Utplot, Ulevelzero, Ulevelone, where=Uplot>Uones, color='red', alpha = 0.5, linewidth=0.0);
    #ax3.plot(t, u, color = 'red')


    ax0.set_xlim(0,600)
    ax1.set_xlim(0,600)
    ax2.set_xlim(0,600)
    ax3.set_xlim(0,600)
    #plt.savefig(u"../Output/ForIFAC.final/filter_" + str(n) + ".pdf")
    plt.show()
