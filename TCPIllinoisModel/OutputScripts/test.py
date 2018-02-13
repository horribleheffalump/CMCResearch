
import numpy as np
import matplotlib.pyplot as plt
import mpl_toolkits.mplot3d.axes3d as axes3d

def sp2cart(r,phi,theta):
    X = r * np.sin(phi) * np.cos(theta)
    Y = r * np.sin(phi) * np.sin(theta)
    Z = r * np.cos(phi)
    return(X,Y,Z)
def new2old(r,phi,theta):
    new_r = r
    new_phi = phi + np.pi/4
    new_theta = theta 
    return(new_r, new_phi, new_theta)

fig = plt.figure()
ax = fig.add_subplot(1,1,1, projection='3d')

theta = np.linspace(0, 0, num = 100)
phi = np.linspace(0, 0, num = 100)
r = np.linspace(0,1,num =100)
(Xpole,Ypole,Zpole) = sp2cart(r,phi,theta)
(XpoleNew,YpoleNew,ZpoleNew) = sp2cart(new2old(r,phi,theta)[0],new2old(r,phi,theta)[1],new2old(r,phi,theta)[2])

theta = np.linspace(0, 0, num = 100)
phi = np.linspace(0, 2*np.pi, num = 100)
r = np.linspace(1,1,num =100)
(Xcirc_phi, Ycirc_phi, Zcirc_phi) = sp2cart(r,phi,theta)
(Xcirc_phiNew,Ycirc_phiNew,Zcirc_phiNew) = sp2cart(new2old(r,phi,theta)[0],new2old(r,phi,theta)[1],new2old(r,phi,theta)[2])

theta = np.linspace(-np.pi, np.pi, num = 100)
phi = np.linspace(np.pi/2, np.pi/2, num = 100)
r = np.linspace(1,1,num =100)
(Xcirc_th, Ycirc_th, Zcirc_th) = sp2cart(r,phi,theta)
(Xcirc_thNew,Ycirc_thNew,Zcirc_thNew) = sp2cart(new2old(r,phi,theta)[0],new2old(r,phi,theta)[1],new2old(r,phi,theta)[2])


ax.plot(Xpole,Ypole,Zpole, color = 'blue')
ax.plot(XpoleNew,YpoleNew,ZpoleNew, color = 'blue', linewidth = '3')

ax.plot(Xcirc_phi,Ycirc_phi,Zcirc_phi, color = 'red')
ax.plot(Xcirc_phiNew,Ycirc_phiNew,Zcirc_phiNew, color = 'red', linewidth = '3')
ax.plot(Xcirc_th,Ycirc_th,Zcirc_th, color = 'yellow')
ax.plot(Xcirc_thNew,Ycirc_thNew,Zcirc_thNew, color = 'yellow', linewidth = '3')




plt.show()