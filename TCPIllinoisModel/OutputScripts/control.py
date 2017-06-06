import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab


filename = u"../out/control.txt"
t, u, rtt, rttmin, rttmax, dm, dh, dl,ss, thresh = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5,6,7,8,9), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(20, 6), dpi=150, facecolor='w', edgecolor='k')

plt.plot(t, u, '-', color = 'blue')
#plt.plot(t, dh, 'o', color = 'black')
#plt.plot(t, dl, 'x', color = 'red')
#plt.plot(t, ss, '--', color = 'green')
#plt.plot(t, thresh, ':', color = 'yellow')
ax1 = plt.subplot(111)
#ax1.set_ylim(0,20)
plt.show()

#plt.plot(t, rtt, '--', color = 'red')
#plt.plot(t, rttmin, '--', color = 'black')
#plt.plot(t, rttmax, '--', color = 'black')
#plt.plot(t, dm, '--', color = 'blue')
#plt.show()


#f.savefig("../output/alpha_beta.pdf")