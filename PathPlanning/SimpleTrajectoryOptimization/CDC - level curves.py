
import numpy as np
from SurveillanceTarget import *
from Channel import *
from UAV import *
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import matplotlib
matplotlib.rc('font',**{'family':'serif'})
matplotlib.rc('text', usetex = True)
from matplotlib.gridspec import GridSpec

folder = r'D:/Наука/_Статьи/__в работе/path planning/'

X0 = np.array([0,0])
XT = X0 # np.array([10,0])
Xtg0 = np.array([5.0,0.0])
XtgT = np.array([3.0,6.0])
Xbs = np.array([1.0, 5.0])
r0 =5.0
d0 = 2.5
kappa = 0.05
epsilon = 0.01
h = 0.1
V0 = 0.2
t0 = 0
T = 60
kT = 0.1




target = Target(Xtg0, XtgT, d0)
channel = Channel(Xbs, r0)
uav = UAV(V0, X0, h)
uav.setmission(target, channel, kappa, epsilon)


    
def F(t, X):
    return target.nu(t,X) * np.log(1 + channel.l(X) * uav.u(t,X)) - kappa * uav.u(t,X)

fig = plt.figure(num=None, figsize=(9, 4), dpi=150)
gs = GridSpec(1, 2)

ax1 = fig.add_subplot(gs[0,0])#, projection='3d')
ax2 = fig.add_subplot(gs[0,1])#, projection='3d')


delta = 0.05
x = np.arange(-5.0, 15.0, delta)
y = np.arange(-5.0, 15.0, delta)
X, Y = np.meshgrid(x, y)
Z0 = np.zeros_like(X)
ZT = np.zeros_like(X)
for i in range(0, X.shape[0]):
    for j in range(0, X.shape[1]):
        Z0[i,j] = F(0.0, [X[i,j], Y[i,j]])
        ZT[i,j] = F(1.0, [X[i,j], Y[i,j]])

minZ = np.min([Z0,ZT]) 
maxZ = np.max([Z0,ZT])
lev =  np.arange(minZ, maxZ, 0.1)
print(lev)
#ax1.plot_surface(X, Y, Z0)
#ax2.plot_surface(X, Y, ZT)
ax1.contour(X, Y, Z0, cmap='coolwarm', levels = lev)    
ax2.contour(X, Y, ZT, cmap='coolwarm', levels = lev)    
 
#ax1.plot([Xtg0[0], XtgT[0]], [Xtg0[1], XtgT[1]], color ="green", linestyle=":")
ax1.scatter([Xtg0[0]], [Xtg0[1]], color ="green", marker = 'x')
ax1.scatter([Xbs[0]], [Xbs[1]], color ="blue", marker = 'v')
ax1.set_xlim([-2, 8])
ax1.set_ylim([-2, 8])
ax1.set_aspect('equal')

#ax2.plot([Xtg0[0], XtgT[0]], [Xtg0[1], XtgT[1]], color ="green", linestyle=":")
ax2.scatter([XtgT[0]], [XtgT[1]], color ="green", marker = 'x')
ax2.scatter([Xbs[0]], [Xbs[1]], color ="blue", marker = 'v')
ax2.set_xlim([-2, 8])
ax2.set_ylim([-2, 8])
ax2.set_aspect('equal')

ax1.set_xlabel("$x_1^*(t)$ (km)")
ax1.set_ylabel("$x_2^*(t)$ (km)")

ax2.set_xlabel("$x_1^*(t)$ (km)")
ax2.set_ylabel("$x_2^*(t)$ (km)")

#ax2.plot(t, NU, color = 'blue', label = 'surv quality')
#ax2.plot(t, L, color = 'green', label = 'transfer quality')
#ax2.legend()

#ax3.plot(t, D, color = 'blue', label = 'target dist')
#ax3.plot(t, R, color = 'green', label = 'BS dist')
##ax3.plot(t, x_plot, color = 'blue', label = 'X')
##ax3.plot(t, y_plot, color = 'green', label = 'Y')
#ax3.legend()

#ax4.plot(t, U, color = 'red', label = 'optimal control')
#ax4.legend()

#print(j)
#plt.savefig(folder + 'levels' + str(j) + '.pdf')
plt.tight_layout()
plt.show()