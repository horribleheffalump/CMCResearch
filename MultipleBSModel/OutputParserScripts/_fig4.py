import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
import matplotlib.gridspec as gridspec

f = plt.figure(num=None, figsize=(7,5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)

#ax0 = plt.subplot(311)
#ax1 = plt.subplot(312)
#ax2 = plt.subplot(313)


gs = gridspec.GridSpec(3, 2,
                       width_ratios=[20,1],
                       height_ratios=[1,1,1]
                       )

#gs.update(left=0.07, bottom=0.07, right=0.95, top=0.99, wspace=0.03, hspace=0.13)
gs.update(left=0.07, bottom=0.04, right=0.95, top=0.99, wspace=0.02, hspace=0.02)

ax0 = plt.subplot(gs[0])
ax01 = plt.subplot(gs[1])
ax1 = plt.subplot(gs[2])
ax11 = plt.subplot(gs[3])
ax2 = plt.subplot(gs[4])
ax21 = plt.subplot(gs[5])



ax = [ax0, ax1, ax2]
axleft = [ax01,ax11,ax21]
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

    
    #ax[n].plot(t, p0, color = 'blue')
    #ax1.plot(t, p1, color = 'blue')
    #ax0.plot(t, p2, color = 'blue')
    
    
    ax[n].plot(t, u, color='black')

    axleft[n].fill_between([0,1], 0, 1, color='black', alpha = 0.2, linewidth=0.0);
    axleft[n].text(0.2, 0.6, 'good', rotation=-90)
    axleft[n].set_axis_off()

    #ax4.fill_between(Utplot, Ulevelzero, Ulevelone, where=Uplot>Uones, color='black', alpha = 0.5, linewidth=0.0);
    #ax3.plot(t, u, color = 'red')

ax0.set_xlim(0,600)
ax1.set_xlim(0,600)
ax2.set_xlim(0,600)
ax0.axes.get_yaxis().set_visible(False)
ax1.axes.get_yaxis().set_visible(False)
ax2.axes.get_yaxis().set_visible(False)



axleft[2].text(1.3, 2.4, "States of transmission channels MCs", rotation=-90);

#ax1[2].yaxis.set_label_coords(1.05, 0.5)
#ax[2].set_xlabel("Time, $t$")

ax0.text(-20, 0.5, r'$\hat{u}^{1}_t$', rotation=90)
ax1.text(-20, 0.5, r'$\hat{u}^{2}_t$', rotation=90)
ax2.text(-20, 0.5, r'$\hat{u}^{3}_t$', rotation=90)


plt.setp(ax[0].get_xticklabels(), visible=False)
plt.setp(ax[1].get_xticklabels(), visible=False)


ax2.text(-43, 2.3, r'Suboptimal control for the $i$th channel', rotation=90)



#plt.show()
plt.savefig(u"../Output/ForIFAC.final/fig_control.pdf")
