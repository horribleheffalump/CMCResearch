
import numpy as np
from SurveillanceTarget import *
from Channel import *
from UAV import *
import matplotlib.pyplot as plt
from matplotlib.gridspec import GridSpec
import scipy.integrate as spi

folder = r'D:/Наука/_Статьи/__в работе/path planning/'

X0 = np.array([0,0])
XT = np.array([10,0])
Xtg = np.array([2.0,3.0])
Xbs = np.array([7.0, -3.0])
r0 = 5.0
d0 = 2.5
kappa = 0.4
epsilon = 0.05
h = 0.1
#V0 = 0.5
t0 = 0
T = 60
kT = 1





for j in range(0,20):
    V0 = 0.5 + 0.01 * j


    target = Target(Xtg, d0)
    channel = Channel(Xbs, r0)
    uav = UAV(V0, X0, h)
    uav.setmission(target, channel, kappa)


    X = np.zeros((int((T - t0) / h) + 1,2))
    Psi = np.zeros((int((T - t0) / h) + 1,2))
    t = np.zeros(int((T - t0) / h) + 1)
    u = np.zeros(int((T - t0) / h) + 1)
    gamma = 0
    i = 0
    gamma = np.pi / 2
    for s in np.arange(t0, T + h, h):
        if s < T/2:
            gamma = gamma - 2.0 * np.pi * h / T
        else:
            gamma = gamma + 2.0 * np.pi * h / T
        #gamma = gamma - 2.0 * np.pi * h / T
        t[i] = s
        x = uav.step(gamma)
        u[i] = uav.u(x)
        X[i] = x
        Psi[i] = [np.cos(gamma), np.sin(gamma)]
        #X[i] = (XT-X0) * h/T * i
        #Psi[i] = [np.cos(gamma), np.sin(gamma)]
        i = i + 1

#    XT = X[-1]
  
    

    def dFdX(X,kappa):
        return target.dnu(X) * np.log(1 + channel.l(X) * uav.u(X)) + target.nu(X) * (channel.dl(X) * uav.u(X) + channel.l(X) * target.dnu(X)) / (1 + channel.l(X) * uav.u(X)) - kappa * uav.du(X)

    def dXdPsi(Psi,V):
        return T * V * Psi / np.linalg.norm(Psi)




    def fun(x, y):
        res = []
        for i in range(0, y.shape[1]):
            res.append(np.hstack((dXdPsi(y[2:4,i], V0), dFdX(y[0:2,i], kappa))))
        return np.transpose(np.vstack(res))

    def bc(ya, yb):
        return np.array([ya[0] - X0[0], ya[1] - X0[1], yb[2] + kT * (yb[0] - XT[0]), yb[3] + kT * (yb[1] - XT[1])])

    y = np.vstack((np.transpose(X), np.transpose(Psi))) #np.zeros((2, t.size)))) #np.ones((4, t.size))

    t = np.arange(0,1 + 1e-10,h / T)

    res = spi.solve_bvp(fun, bc, t, y, verbose=1)
    print(res.status, res.message, res.success)

    #print(res.sol(t))
    #print(t)
    #print(res.x)


    fig = plt.figure(num=None, figsize=(9, 6), dpi=300)
    ax1 = plt.gca()
    x_plot = res.y[0]
    y_plot = res.y[1]
    ax1.scatter(Xbs[0],Xbs[1], marker = 'v', color = 'blue')
    ax1.scatter(Xtg[0],Xtg[1], marker = 'x', color = 'red')
    ax1.scatter(XT[0],XT[1], marker = 'x', color = 'green')
    ax1.plot(X[:,0],X[:,1], color = 'blue', linewidth = 2.0, alpha = 0.3)
    ax1.plot(x_plot, y_plot, color = 'red')
    #ax1.set_aspect('equal')

    print(j)
    plt.savefig(folder + 'pic' + str(j) + '.jpg')