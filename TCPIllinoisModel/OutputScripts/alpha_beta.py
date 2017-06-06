import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab


filename = u"../out/alpha_beta.txt"
d, alpha, beta = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2), unpack=True, dtype=float)

f = plt.figure(num=None, figsize=(7, 6), dpi=150, facecolor='w', edgecolor='k')

plt.plot(d, alpha, '--', color = 'red')
plt.plot(d, beta, '--', color = 'blue')

plt.show()
#f.savefig("../output/alpha_beta.pdf")
