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

#arr = np.linspace(0, np.sqrt(2))
#print(arr)

def maxval(x):
    for i in range(0, len(x)):
        if(x[i] > 1): x[i] = 1
    return x
def minval(x):
    for i in range(0, len(x)):
        if(x[i] < 0): x[i] = 0
    return x

def lims(x):
    if(x > 0.95): return 0.95
    else:
        if(x < 0.05): return 0.05
        else: return x


#def max(x,y):
#    if(x > y): return x
#    else: return y

#def min(x,y):
#    if(x < y): return x
#    else: return y

#print (min(1,2))
#print (max(1,2))

#x = np.linspace(0, np.sqrt(10**2 + 25**2), 1000)
x = np.linspace(0, 5.0, 1000)
#plt.plot(x, exp(-10*x/2), color = 'black')
#plt.plot(x, maxval(exp(-5+10*x/2)), color = 'blue')
#plt.plot(x, minval(1.0 - exp(-10*x/2) - maxval(exp(-5+10*x/2))), color = 'red')
#print (maxval(exp(-10+x)))

def p(t):
    return np.matrix([lims(exp(-t/2)), 1.0 - lims(exp(-t/2)) - lims(exp(-5+t/2)), lims(exp(-5+t/2))]);
def P(t):
    return np.vstack([p(t), p(t), p(t)]);
def la(t):
    return P(t) - np.identity(3);

print(la(0.0))
print(la(2.0))
print(la(4.0))
print(la(6.0))
print(la(8.0))
print(la(10.0))

show()
#f.savefig("../Output/graph.pdf")

