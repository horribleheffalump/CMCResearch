
import numpy as np
from SurveillanceTarget import *
from Channel import *
from UAV import *
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

from matplotlib.gridspec import GridSpec
import matplotlib
matplotlib.rc('font',**{'family':'serif'})
matplotlib.rc('text', usetex = True)

folder = r'D:/Наука/_Статьи/__в работе/path planning/'


fig = plt.figure(num=None, figsize=(9, 3), dpi=150)
gs = GridSpec(1, 2)

ax1 = fig.add_subplot(gs[0,0])#, projection='3d')
ax2 = fig.add_subplot(gs[0,1])#, projection='3d')

x = np.arange(0.01,1,0.01)
y = -np.log(x) - 1 + x

ax1.plot(x,y, color = "black")

eps = 1.9
r = 1/(1+eps)

x = np.arange(0.01,r,0.01)
y = -np.log(x) - 1 + x
ax2.plot(x,y, color = "black")
x = np.arange(r,1,0.01)
y = (-np.log(r) - 1 + r) * np.ones_like(x)
ax2.plot(x,y, color = "black")
x = np.arange(r,1,0.01)
y = -np.log(x) - 1 + x
ax2.plot(x,y, color = "black", linestyle=":")

ax2.plot([r,r],[0,(-np.log(r) - 1 + r)], color = "black", linestyle=":")

ax1.set_xticks([])
ax2.set_xticks([r])
ax2.set_xticklabels(['$\\rho_{\\varepsilon}$'])
ax1.set_yticks([])
ax2.set_yticks([])

ax1.set_ylabel('$g_0(\\rho)$')
ax2.set_ylabel('$g_{\\varepsilon}(\\rho)$')

ax1.set_ylim(0, 3)
ax2.set_ylim(0, 3)

plt.tight_layout()
plt.show()