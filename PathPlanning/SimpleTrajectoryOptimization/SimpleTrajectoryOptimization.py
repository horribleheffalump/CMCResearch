
import numpy as np
from SurveillanceTarget import *
from Channel import *
from UAV import *
import matplotlib.pyplot as plt
from matplotlib.gridspec import GridSpec
import scipy.integrate as spi

folder = r'D:/Наука/_Статьи/__в работе/path planning/'

X0 = np.array([0,0])
XT = X0 # np.array([10,0])
Xtg0 = np.array([5.0,0.0])
XtgT = np.array([3.0,6.0])
Xbs = np.array([1.0, 5.0])
r0 = 5.0
d0 = 2.5
kappa = 0.05
epsilon = 0.01
h = 0.1
#V0 = 0.5
t0 = 0
T = 60
kT = 0.1





for j in range(0,1):
    V0 = 0.5 + 0.05 * j


    target = Target(Xtg0, XtgT, d0)
    channel = Channel(Xbs, r0)
    uav = UAV(V0, X0, h)
    uav.setmission(target, channel, kappa, epsilon)


    X = np.zeros((int((T - t0) / h) + 1,2))
    Psi = np.zeros((int((T - t0) / h) + 1,2))
    t = np.zeros(int((T - t0) / h) + 1)
    u = np.zeros(int((T - t0) / h) + 1)
    gamma = 0
    i = 0
    gamma = -np.pi / 2
    for s in np.arange(t0, T + h, h):
        #if s < T/2:
        #    gamma = gamma - 2.0 * np.pi * h / T
        #else:
        #    gamma = gamma + 2.0 * np.pi * h / T
        gamma = gamma + 2.0 * np.pi * h / T
        t[i] = s
        x = uav.step(gamma)
        #u[i] = uav.u(x)
        X[i] = x
        Psi[i] = [np.cos(gamma), np.sin(gamma)]
        #X[i] = (XT-X0) * h/T * i
        #Psi[i] = [np.cos(gamma), np.sin(gamma)]
        i = i + 1

#    XT = X[-1]
  
    
    def F(t, X):
        return target.nu(t,X) * np.log(1 + channel.l(X) * uav.u(t,X)) - kappa * uav.u(t,X)

    def dFdX(t, X,kappa):
        return target.dnu(t,X) * np.log(1 + channel.l(X) * uav.u(t,X)) + target.nu(t,X) * (channel.dl(X) * uav.u(t,X) + channel.l(X) * target.dnu(t,X)) / (1 + channel.l(X) * uav.u(t,X)) - kappa * uav.du(t,X)

    def dXdPsi(Psi,V):
        return T * V * Psi / np.linalg.norm(Psi)




    def fun(x, y):
        res = []
        for i in range(0, y.shape[1]):
            res.append(np.hstack((dXdPsi(y[2:4,i], V0), dFdX(x[i], y[0:2,i], kappa))))
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


    fig = plt.figure(num=None, figsize=(12, 6), dpi=300)
    gs = GridSpec(4, 3)

    ax1 = fig.add_subplot(gs[0:3, 0:3])
    ax2 = fig.add_subplot(gs[3, 0])
    ax3 = fig.add_subplot(gs[3, 1])
    ax4 = fig.add_subplot(gs[3, 2])
    
    #ax1 = plt.gca()
    x_plot = res.y[0]
    y_plot = res.y[1]

    NU = np.zeros(int((T - t0) / h) + 1)
    L = np.zeros(int((T - t0) / h) + 1)
    D = np.zeros(int((T - t0) / h) + 1)
    R = np.zeros(int((T - t0) / h) + 1)
    Xtg = np.zeros((int((T - t0) / h) + 1,2))
    U = np.zeros(int((T - t0) / h) + 1)

    i = 0
    for s in np.arange(t0, 1 + 1e-10,h / T):
        Xtg[i] = target.Xp(s)
        x = [x_plot[i], y_plot[i]]
        NU[i] = target.nu(s, x)
        D[i] = target.d(s, x)
        L[i] = channel.l(x)
        R[i] = channel.r(x)
        U[i] = uav.u(s, x)
        i = i + 1

 
    ax1.scatter(Xbs[0],Xbs[1], marker = 'v', color = 'blue')
    ax1.plot(Xtg[:,0],Xtg[:,1], marker = 'x', color = 'red')
    ax1.scatter(XT[0],XT[1], marker = 'x', color = 'green')
    ax1.plot(X[:,0],X[:,1], color = 'blue', linewidth = 2.0, alpha = 0.3)
    ax1.plot(x_plot, y_plot, color = 'red')
    ax1.set_aspect('equal')

    ax2.plot(t, NU, color = 'blue', label = 'surv quality')
    ax2.plot(t, L, color = 'green', label = 'transfer quality')
    ax2.legend()

    ax3.plot(t, D, color = 'blue', label = 'target dist')
    ax3.plot(t, R, color = 'green', label = 'BS dist')
    #ax3.plot(t, x_plot, color = 'blue', label = 'X')
    #ax3.plot(t, y_plot, color = 'green', label = 'Y')
    ax3.legend()

    ax4.plot(t, U, color = 'red', label = 'optimal control')
    ax4.legend()

    print(j)
    plt.savefig(folder + 'pic' + str(j) + '.jpg')