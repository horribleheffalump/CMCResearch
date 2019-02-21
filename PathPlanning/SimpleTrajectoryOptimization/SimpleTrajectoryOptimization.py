import numpy as np
from SurveillanceTarget import *
from Channel import *
from UAV import *
import matplotlib.pyplot as plt
from matplotlib.gridspec import GridSpec



X0 = np.array([0,0])
Xbs = np.array([2.0,3.0])
Xtg = np.array([10.0, -2.0])
r0 = 7.0
d0 = 3.5
kappa = 0.05
epsilon = 0.05
V0 = 0.5
h = 0.1

t0 = 0
T = 60

target = Target(Xtg, d0)
channel = Channel(Xbs, r0)
uav = UAV(V0, X0, h)
uav.setmission(target, channel, kappa)

print(uav.u([0,0]))


X = np.zeros((int((T-t0)/h)+1,2))
t = np.zeros(int((T-t0)/h)+1)
u = np.zeros(int((T-t0)/h)+1)

gamma = -np.pi/2
i = 0
for s in np.arange(t0, T+h, h):
    gamma = gamma + 0.01
    t[i] = s
    x = uav.step(gamma)
    u[i] = uav.u(x)
    X[i] = x
    i = i + 1
    print(s, x)


fig = plt.figure(num=None, figsize=(9, 6), dpi=300)
gs = GridSpec(2, 1)

ax1 = fig.add_subplot(gs[0, 0])
ax2 = fig.add_subplot(gs[1, 0])

#ax = plt.gca()

ax1.scatter(Xbs[0],Xbs[1], marker = 'v', color = 'blue')
ax1.scatter(Xtg[0],Xtg[1], marker = 'x', color = 'red')
ax1.plot(X[:,0],X[:,1])
ax1.set_aspect('equal')

ax2.plot(t,u)


plt.show()
