import matplotlib
import matplotlib.pyplot as plt
import numpy as np
matplotlib.rc('text', usetex = True)
import pylab


filename = u"../Output/TrTrajectory.txt"
t, X = np.loadtxt(filename, delimiter = ' ', usecols=(0,1), unpack=True, dtype=float)

filename = u"../Output/temp.txt"
tt, p1,p2,p3 = np.loadtxt(filename, delimiter = ' ', usecols=(0,1,2,3), unpack=True, dtype=float)

from pylab import *

f = plt.figure(num=None, figsize=(10, 10), dpi=150, facecolor='w', edgecolor='k')


plt.subplots_adjust(left=0.06, bottom=0.07, right=0.98, top=0.95, wspace=0.1)
#plt.plot(t, X, 'x', color = 'blue')
plt.plot(tt, p1, '-', color = 'black')
plt.plot(tt, p2, '-', color = 'blue')
plt.plot(tt, p3, '-', color = 'red')



#def maxval(x):
#    for i in range(0, len(x)):
#        if(x[i] > 1): x[i] = 1
#    return x
#def minval(x):
#    for i in range(0, len(x)):
#        if(x[i] < 0): x[i] = 0
#    return x

#def lims(x):
#    if(x > 0.95): return 0.95
#    else:
#        if(x < 0.05): return 0.05
#        else: return x


#x = np.linspace(0, 5.0, 1000)


#def p(t):
#    return np.matrix([lims(exp(-t/2)), 1.0 - lims(exp(-t/2)) - lims(exp(-5+t/2)), lims(exp(-5+t/2))]);
#def P(t):
#    return np.vstack([p(t), p(t), p(t)]);
#def la(t):
#    return P(t) - np.identity(3);



#print(la(0.0))
#print(la(2.0))
#print(la(4.0))
#print(la(6.0))
#print(la(8.0))
#print(la(10.0))

f.savefig("../Output/graph.pdf")
#show()

