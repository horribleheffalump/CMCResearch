include("target.jl")
include("basestation.jl")
include("UAVSurvey.jl")
using LinearAlgebra
using DifferentialEquations
using BoundaryValueDiffEq
#using Plots; gr()
using PyPlot

#Xtg0 = [5.0,0.0]
#XtgT = [3.0,6.0]
trajectory = [
[0.0,  3.0,  6.0],
[0.0,  5.0,  0.0]
]
totaldist = 0.0
for i = 2:size(trajectory)[1]
    global totaldist
    X₀ = trajectory[i-1][2:3]
    X₁ = trajectory[i][2:3]
    dist = norm(X₁ - X₀)
    trajectory[i][1] = dist
    totaldist = totaldist + dist
end
println(trajectory)
println(totaldist)
trajectory[1][1] = 0.0
for i = 2:size(trajectory)[1]
    trajectory[i][1] = trajectory[i-1][1] + trajectory[i][1]/totaldist
end
println(trajectory)


d₀ = 2.5
#target = Target(Xtg0, XtgT, d₀)
target = Target(trajectory, d₀)

Xbs = [1.0, 5.0]
#Xbs = [6.04, 1.86]
r₀ = 3.0
bs = BaseStation(Xbs, r₀)

X0 = [0,0]
#X0 = [3.27,-0.48]
XT = X0 # np.array([10,0])
kappa = 0.05
epsilon = 0.01
h = 0.1
V0 = 0.45
t0 = 0.0
T = 60.0
uav = UAVSurvey(target, bs, X0, X0, V0, t0, h, kappa, epsilon)
kT = 0.1

function F(t::Float64, X::Array{Float64})
    return nu(target, t, X) * log(1.0 + l(bs, X) * u(uav, t, X)) - kappa * u(uav, t, X)
end

function dFdX(t::Float64, X::Array{Float64}, kappa::Float64)
    return dnu(target, t, X) * log(1.0 + l(bs, X) * u(uav, t, X)) + nu(target, t, X) * (dl(bs, X) * u(uav, t, X) + l(bs, X) * du(uav, t, X)) / (1.0 + l(bs, X) * u(uav, t, X)) - kappa * du(uav, t, X)
end


function dXdPsi(Psi::Array{Float64}, V::Float64)
    return T * V * Psi / norm(Psi)
end



#println(dFdX(t0, X0, kappa))
#println(dXdPsi(Xtg0, V0))

function RHS!(du,u,p,t::Float64)
    X   = [u[1], u[2]]
    Psi = [u[3], u[4]]
    dfdx = dFdX(t, X, kappa)
    dxdpsi = dXdPsi(Psi,p)

    du[1] = dxdpsi[1]
    du[2] = dxdpsi[2]
    du[3] = -dfdx[1]
    du[4] = -dfdx[2]
    #println(du[:])
end

function bc2!(residual, u, p, t) # u[1] is the beginning of the time span, and u[end] is the ending
    residual[1] = u[1][1] - X0[1]
    residual[2] = u[1][2] - X0[2]
    residual[3] = u[end][3] + kT * (u[end][1] - XT[1])
    residual[4] = u[end][4] + kT * (u[end][2] - XT[2])
    #print(residual[:])
end

function plotall(V0::Float64)
    bvp2 = BVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan, V0)
    sol2 = solve(bvp2, Shooting(Euler()), dt=h)
    bc2!(res, sol2.u, [], 0.0)



    fig,axes = subplots(2,1)
    ax1 = axes[1]
    ax2 = axes[2]
    ax1.plot(map(x -> x[1], sol2.u), map(x -> x[2], sol2.u), color ="red")
    #ax1.plot([Xtg0[1], XtgT[1]], [Xtg0[2], XtgT[2]], color ="green")
    #ax1.scatter([XtgT[1]], [XtgT[2]], color ="green")
    ax1.plot(map(x -> x[2], trajectory), map(x -> x[3], trajectory), color ="green", linestyle=":")
    ax1.scatter([trajectory[1][2]], [trajectory[1][3]], color ="green", marker = "x")
    ax1.scatter([trajectory[end][2]], [trajectory[end][3]], color ="green", marker = "x")
    ax1.scatter([Xbs[1]], [Xbs[2]], color ="blue")
    ax1.set_xlim([-2, 8])
    ax1.set_ylim([-2, 8])
    ax1.set_title(string("V=", V0))


    crit=0.0
    intcontrol=0.0
    control = zeros(size(sol2.t)[1])
    for i = 1:size(sol2.t)[1]
        #global control, intcontrol, crit
        control[i] = u(uav, sol2.t[i], [sol2.u[i][1], sol2.u[i][2]])
        intcontrol = intcontrol + control[i] * h * T
        crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
    end
    ax2.plot(sol2.t * T, control, color ="red")

    savefig(string("D:\\Наука\\_Статьи\\__в работе\\path planning\\pic_V_", V0 , ".png"))
end

function plotpath(V0::Float64)
    bvp2 = BVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan, V0)
    sol2 = solve(bvp2, Shooting(Euler()), dt=h)
    bc2!(res, sol2.u, [], 0.0)

    matplotlib.rc("text", usetex = true)

    fig = plt.figure(figsize=(4, 4), dpi=300)
    plt.subplots_adjust(left=0.05, bottom=0.08, right=0.99, top=0.98)
    ax1 = fig.gca()
    img = plt.imread("D:\\map.png")
    ax1.imshow(img, extent=[-2, 8, -2, 8])

    ax1.plot(map(x -> x[1], sol2.u), map(x -> x[2], sol2.u), color ="blue")

    #ax1.plot([Xtg0[1], XtgT[1]], [Xtg0[2], XtgT[2]], color ="green", linestyle=":")
    #ax1.scatter([Xtg0[1]], [Xtg0[2]], color ="green", marker = "x")
    #ax1.scatter([XtgT[1]], [XtgT[2]], color ="green", marker = "x")
    ax1.plot(map(x -> x[2], trajectory), map(x -> x[3], trajectory), color ="white", linestyle=":")
    ax1.scatter([trajectory[1][2]], [trajectory[1][3]], color ="white", marker = "x")
    ax1.scatter([trajectory[end][2]], [trajectory[end][3]], color ="white", marker = "x")
    ax1.scatter([Xbs[1]], [Xbs[2]], color ="red", marker = "^")
    ax1.set_xlim([-2, 8])
    ax1.set_ylim([-2, 8])
    #ax1.set_xlabel("\$x_1^*(t)\$")
    #ax1.set_ylabel("\$x_2^*(t)\$")

    crit=0.0
    intcontrol=0.0
    control = zeros(size(sol2.t)[1])
    for i = 1:size(sol2.t)[1]
        #global control, intcontrol, crit
        control[i] = u(uav, sol2.t[i], [sol2.u[i][1], sol2.u[i][2]])
        intcontrol = intcontrol + control[i] * h * T
        crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
    end
    #ax2.plot(sol2.t * T, control, color ="red")

    savefig(string("D:\\pic_V_", V0 , ".pdf"))
    #savefig(string("D:\\Наука\\_Статьи\\__в работе\\path planning\\pic_V_", V0 , ".eps"))
end

function plotcontrol(V0::Float64)
    bvp2 = BVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan, V0)
    sol2 = solve(bvp2, Shooting(Euler()), dt=h)
    bc2!(res, sol2.u, [], 0.0)

    #matplotlib.rc('font',**{'family':'serif'})
    matplotlib.rc("text", usetex = true)

    fig = plt.figure(figsize=(4, 4), dpi=300)
    plt.subplots_adjust(left=0.10, bottom=0.07, right=0.99, top=0.98)

    ax1 = fig.gca()
    crit=0.0
    intcontrol=0.0
    control = zeros(size(sol2.t)[1])
    for i = 1:size(sol2.t)[1]
        #global control, intcontrol, crit
        control[i] = u(uav, sol2.t[i], [sol2.u[i][1], sol2.u[i][2]])
        intcontrol = intcontrol + control[i] * h * T
        crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
    end
    #ax1.set_xlabel("\$t \quad (min)\$")
    #ax1.set_ylabel("\$u^*(t)\$")
    ax1.plot(sol2.t * T, control, color ="blue")
    plt.xticks([0,30,60], ("\$0\\,min\$", "\$30\\,min\$", "\$60\\,min\$"))
    plt.yticks([0,10,20], ("\$0\$","\$10\$","\$u^*(t)\$"))
    #savefig(string("D:\\Наука\\_Статьи\\__в работе\\path planning\\pic_control_V_", V0 , ".pdf"))
    savefig(string("D:\\pic_control_V_", V0 , ".pdf"))
end


tspan = (0.0,1.0)

#bvp2 = TwoPointBVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan)
sol2 = []
res = [0.0,0.0,0.0,0.0]
maxcrit = 0.0
maxintcontrol = 0.0
bestV0 = 0.0
h = 0.01

#Vinterval = [0.1, 1.0]
Vstep = 0.001
Vinterval = [0.1, 0.55]
for j = 0:(Vinterval[2] - Vinterval[1])/Vstep
    global maxcrit, maxintcontrol, bestV0
    V0 = Vinterval[1] + j * Vstep
    println(V0)
    bvp2 = BVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan, V0)
    sol2 = solve(bvp2, Shooting(Euler()), dt=h)
    bc2!(res, sol2.u, [], 0.0)
    if norm(res) < 1e-1
        crit=0.0
        intcontrol=0.0
        control = zeros(size(sol2.t)[1])
        for i = 1:size(sol2.t)[1]
            control[i] = u(uav, sol2.t[i], [sol2.u[i][1], sol2.u[i][2]])
            intcontrol = intcontrol + control[i] * h * T
            crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
        end
        #println(crit, maxcrit)
        if crit > maxcrit
            maxcrit = crit
            maxintcontrol = intcontrol
            bestV0 = V0
        end
        println("V: ", V0, " crit: ", crit, " control integral: ", intcontrol, " maxcrit: ", maxcrit, " bestV: ", bestV0)
        plotall(V0)
    else
        #println("residual norm:", norm(res))
    end

end

V0 = bestV0
println(bestV0)
plotpath(bestV0)
plotcontrol(bestV0)
#plotall(bestV0)
println("best V: ", V0, "best crit: ", maxcrit, " control integral: ", maxintcontrol)
