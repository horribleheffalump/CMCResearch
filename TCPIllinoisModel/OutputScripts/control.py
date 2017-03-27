import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab


filename = u"../out/control.txt"
t, u, rtt, rttmin, rttmax, dm = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3,4,5), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(7, 6), dpi=150, facecolor='w', edgecolor='k')

plt.plot(t, u, '--', color = 'blue')
plt.show()

#plt.plot(t, rtt, '--', color = 'red')
#plt.plot(t, rttmin, '--', color = 'black')
#plt.plot(t, rttmax, '--', color = 'black')
#plt.plot(t, dm, '--', color = 'blue')
#plt.show()


#f.savefig("../output/alpha_beta.pdf")