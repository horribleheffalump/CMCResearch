include("target.jl")
include("basestation.jl")
include("UAVSurvey.jl")
using LinearAlgebra
using DifferentialEquations
using BoundaryValueDiffEq
#using Plots; gr()
using PyPlot

Xtg0 = [5.0,0.0]
XtgT = [3.0,6.0]
d₀ = 2.5
target = Target(Xtg0, XtgT, d₀)

Xbs = [1.0, 5.0]
r₀ = 5.0
bs = BaseStation(Xbs, r₀)


X0 = [0.0,0.0]
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
    return dnu(target, t, X) * log(1.0 + l(bs, X) * u(uav, t, X)) + nu(target, t, X) * (dl(bs, X) * u(uav, t, X) + l(bs, X) * dnu(target, t, X)) / (1.0 + l(bs, X) * u(uav, t, X)) - kappa * du(uav, t, X)
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
    du[3] = dfdx[1]
    du[4] = dfdx[2]
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
    ax1.plot([Xtg0[1], XtgT[1]], [Xtg0[2], XtgT[2]], color ="green")
    ax1.scatter([XtgT[1]], [XtgT[2]], color ="green")
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

tspan = (0.0,1.0)

#bvp2 = TwoPointBVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan)
sol2 = []
res = [0.0,0.0,0.0,0.0]
maxcrit = 0.0
maxintcontrol = 0.0
bestV0 = 0.0
h = 0.01

Vinterval = [0.1, 0.5]
Vstep = 0.001
for j = 0:(Vinterval[2] - Vinterval[1])/Vstep
    global maxcrit, maxintcontrol, bestV0
    V0 = Vinterval[1] + j * Vstep
    #println(V0)
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


plotall(bestV0)
println("best V: ", V0, "best crit: ", crit, " control integral: ", intcontrol)
