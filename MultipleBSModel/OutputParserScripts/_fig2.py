import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab
import matplotlib.gridspec as gridspec

filename = u"../Output/ForIFAC/TrTrajectory.txt"
t, x, y, d1, d2, d3 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(7, 5), dpi=150, facecolor='w', edgecolor='k')
#plt.subplots_adjust(left=0.06, bottom=0.07, right=0.95, top=0.95, wspace=0.1)


gs = gridspec.GridSpec(3, 2,
                       width_ratios=[20,1],
                       height_ratios=[1,1,1]
                       )

gs.update(left=0.07, bottom=0.04, right=0.95, top=0.99, wspace=0.02, hspace=0.02)

ax2 = plt.subplot(gs[0])
ax21 = plt.subplot(gs[1])
ax3 = plt.subplot(gs[2])
ax31 = plt.subplot(gs[3])
ax4 = plt.subplot(gs[4])
ax41 = plt.subplot(gs[5])


#ax2 = plt.subplot(611)
#ax21 = plt.subplot(621)
#ax3 = plt.subplot(612)
#ax31 = plt.subplot(622)
#ax4 = plt.subplot(613)
#ax41 = plt.subplot(623)

ax = [ax2,ax3,ax4]
ax1 = [ax21,ax31,ax41]
#ax_N = [ax2_N,ax3_N,ax4_N]

ax2.plot(t, d1, '--', color = 'red')
ax3.plot(t, d2, '--', color = 'red')
ax4.plot(t, d3, '--', color = 'red')

ax2.set_xlim(0,600)
ax3.set_xlim(0,600)
ax4.set_xlim(0,600)

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
    leveltwo = np.ones(len(tplot))*2.0
    levelthree = np.ones(len(tplot))*3.0
    #ax[n].plot(tplot, Xplot, color = 'black')
    ax[n].fill_between(tplot, levelzero, levelone, where=Xplot==o, color='black', alpha = 0.2, linewidth=0.0);
    ax[n].fill_between(tplot, levelone, leveltwo, where=Xplot==ones, color='black', alpha = 0.4, linewidth=0.0);
    ax[n].fill_between(tplot, leveltwo, levelthree,  where=Xplot==ones*2, color='black', alpha = 0.8, linewidth=0.0);
    for tl in ax[n].get_yticklabels():
        tl.set_color('r')

    ax[n].axes.get_yaxis().set_visible(False)
    
    ax1[n].fill_between([0,1], 0, 1, color='black', alpha = 0.2, linewidth=0.0);
    ax1[n].fill_between([0,1], 1, 2, color='black', alpha = 0.4, linewidth=0.0);
    ax1[n].fill_between([0,1], 2, 3, color='black', alpha = 0.8, linewidth=0.0);
    ax1[n].text(0.2, 0.6, 'good', rotation=-90)
    ax1[n].text(0.2, 1.6, 'norm', rotation=-90)
    ax1[n].text(0.2, 2.55, 'bad', color='white', rotation=-90)
    ax1[n].set_axis_off()
    #ax1[n].axes.get_xaxis().set_visible(False)
    #ax1[n].axes.get_yaxis().set_visible(False)


    #filename = u"../Output/CPTrajectory_" + str(n) + ".txt"
    #tN, N = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)
    #ax_N[n].plot(tN, N, color = 'blue')
    #for tl in ax_N[n].get_yticklabels():
    #    tl.set_color('b')
ax1[2].text(1.3, 7.3, "States of transmission channels MCs", rotation=-90);

#ax1[2].yaxis.set_label_coords(1.05, 0.5)
#ax[2].set_xlabel("Time, $t$")

ax[0].text(-20, 1.6, '$r_1(t)$', rotation=90)
ax[1].text(-20, 1.6, '$r_2(t)$', rotation=90)
ax[2].text(-20, 1.6, '$r_3(t)$', rotation=90)


plt.setp(ax[0].get_xticklabels(), visible=False)
plt.setp(ax[1].get_xticklabels(), visible=False)



ax[2].text(-43, 8.4, 'Distance between the UAV and the base stations $r_i(t)$', rotation=90)


#plt.show()
#f.savefig("../Output/graph.pdf")
plt.savefig("../Output/ForIFAC.final/fig_MC.pdf")
