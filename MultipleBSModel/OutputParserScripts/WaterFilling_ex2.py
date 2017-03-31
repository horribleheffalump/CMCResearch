import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
matplotlib.rcParams['text.latex.preamble'] = [
    r'\usepackage{amsfonts}',
    r'\usepackage{amssymb}'
]
import pylab



I = list(range(0,17));
Val = [3,5,10,8,2,6,11,10,11,9,7,1,4,5,6,7,7];
addVal = [0.3,0.5,1.0,0.8,0.2,0.6,1.1,1.0,1.1,0.9,0.7,0.1,0.4,0.5,0.6,0.9,0.9];

#print(I);
#print(Val);

IPlot = np.zeros(len(I)*2-1)
ValPlot = np.zeros(len(Val)*2-1)
addPlot = np.zeros(len(Val)*2-1)
for i in range(0, len(I)-1):
    IPlot[i*2] = I[i]
    ValPlot[i*2] = Val[i]
    addPlot[i*2] = addVal[i]
    IPlot[i*2+1] = I[i+1]
    ValPlot[i*2+1] = Val[i]
    addPlot[i*2+1] = addVal[i]
IPlot[len(I)*2-2] = I[len(I)-1]
ValPlot[len(I)*2-2] = ValPlot[len(I)-1]
addPlot[len(I)*2-2] = addPlot[len(I)-1]

Thresh = 13 * np.ones(len(Val)*2-1) + addPlot


print(IPlot);
print(ValPlot);


f = plt.figure(num=None, figsize=(7, 5), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.08, bottom=0.07, right=0.95, top=0.95, wspace=0.1)
ax = plt.subplot(111)

ax.plot(IPlot, ValPlot, color = 'black')
ax.plot([0,16], [13,13], color = 'black', linestyle='--')
ax.fill_between(IPlot, ValPlot, Thresh, color='black', alpha = 0.2, linewidth=0.0);
ax.arrow(6.5, 0, 0, 11-0.5, head_width=0.2, head_length=0.5, fc='k', ec='k')
ax.arrow(6.5, 11, 0, -11+0.5, head_width=0.2, head_length=0.5, fc='k', ec='k')
plt.text(6.8, 5, r'$\overline{1/l^i}$', fontdict = {'size' : 20})

ax.arrow(6.5, 11, 0, 2+1.1-0.55, head_width=0.2, head_length=0.5, fc='k', ec='k')
ax.arrow(6.5, 13+1.1-0.05, 0, -2-1.1+0.55, head_width=0.2, head_length=0.5, fc='k', ec='k')
plt.text(6.8, 14.3, r'$\tilde{u}^i_t$', fontdict = {'size' : 20})

plt.text(0.4, -0.9, r'$1$', fontdict = {'size' : 20})
plt.text(6.4, -0.9, r'$i$', fontdict = {'size' : 20})
plt.text(15.1, -0.9, r'$M$', fontdict = {'size' : 20})

plt.text(-1.3, 12.6, r'$1/\varkappa$', fontdict = {'size' : 20})

plt.setp(ax.get_yticklabels(), visible=False)
plt.xticks(I, [])



#ax.annotate('wdwedw', (6.5, 0),
#                (6.5, 11),
#                ha="right", va="center",
#                size=12,
#                arrowprops=dict(arrowstyle='<|-|>',
#                                fc="k", ec="k",
#                                ))


ax.set_ylim(0,16)
plt.savefig(u"../Output/ex2.pdf")
#plt.savefig(u"D:/?????/_??????/__? ??????/AiT - mmc/ex1.pdf")
plt.show()
