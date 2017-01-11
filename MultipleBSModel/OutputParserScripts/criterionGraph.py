from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np

# This example demonstrates mplot3d's offset text display.
# As one rotates the 3D figure, the offsets should remain oriented
# same way as the axis label, and should also be located "away"
# from the center of the plot.
#
# This demo triggers the display of the offset text for the x and
# y axis by adding 1e5 to X and Y. Anything less would not
# automatically trigger it.

fig = plt.figure()
ax = fig.gca(projection='3d')
X, Y = np.mgrid[0.2:2:0.05, 0.2:2:0.05]
Z = -1/(X) - 1/(Y) - (0.05*X + Y)
surf = ax.plot_surface(X, Y, Z, cstride=2, rstride=2,  cmap='autumn')

print(X)
print(Y)
print(Z)

#surf = ax.plot_surface(X + 1e5, Y + 1e5, Z, cmap='autumn', cstride=2, rstride=2)
ax.set_xlabel("X")
ax.set_ylabel("Y")
ax.set_zlabel("- 1/(X+Y) - (0.5*X+Y)")
#ax.set_zlim(0, 2)

plt.show()