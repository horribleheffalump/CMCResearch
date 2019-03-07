include("threat.jl")
using LinearAlgebra
using DifferentialEquations
using BoundaryValueDiffEq
#using Plots; gr()
using PyPlot

Xth1_0 = [5.0,0.0]
Xth1_T = [3.0,6.0]
#Xth1_0 = [5.0,5.0]
#Xth1_T = [5.0,5.0]
d1₀ = 2.5
threat1 = Threat(Xth1_0, Xth1_T, d1₀)

Xth2_0 = [0.0,1.0]
Xth2_T = [4.0,5.0]
#Xth2_0 = [1.0,4.0]
#Xth2_T = [1.0,4.0]
d2₀ = 7.5
threat2 = Threat(Xth2_0, Xth2_T, d2₀)

threats = [threat1, threat2]

X0 = [0.0,0.0]
XT = [4.0, 7.0] # np.array([10,0])

t0 = 0.0
T = 60.0

kT = 10

function F(t::Float64, X::Array{Float64})
    return sum(map(a -> th(a, t, X), threats))
end

function dFdX(t::Float64, X::Array{Float64})
    return sum(map(a -> dth(a, t, X), threats))
end

function dXdPsi(Psi::Array{Float64}, V::Float64)
    return T * V * Psi / norm(Psi)
end



#println(dFdX(t0, X0, kappa))
#println(dXdPsi(Xtg0, V0))

function RHS!(du,u,p,t::Float64)
    X   = [u[1], u[2]]
    Psi = [u[3], u[4]]
    dfdx = dFdX(t, X)
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



    fig = figure()
    ax1 = fig.gca()
    #ax2 = axes[2]
    ax1.plot(map(x -> x[1], sol2.u), map(x -> x[2], sol2.u), color ="red")
    for i = 1:size(threats)[1]
        ax1.plot([threats[i].X₀[1], threats[i].X₁[1]], [threats[i].X₀[2], threats[i].X₁[2]], color ="green")
        ax1.scatter([threats[i].X₁[1]], [threats[i].X₁[2]], color ="green")
    end
    ax1.scatter([XT[1]], [XT[2]], color ="red")

    ax1.set_xlim([-2, 8])
    ax1.set_ylim([-2, 8])
    ax1.set_title(string("V=", V0))


    crit=0.0
    for i = 1:size(sol2.t)[1]
        #global control, intcontrol, crit
        crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
    end
    #ax2.plot(sol2.t * T, control, color ="red")

    savefig(string("D:\\Наука\\_Статьи\\__в работе\\path planning\\pic_V_", V0 , ".png"))
end

tspan = (0.0,1.0)

#bvp2 = TwoPointBVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan)
sol2 = []
res = [0.0,0.0,0.0,0.0]
mincrit = 1e10
bestV0 = 0.0
h = 0.01

Vinterval = [0.1, 0.2]
Vstep = 0.002
for j = 0:(Vinterval[2] - Vinterval[1])/Vstep
    global mincrit, maxintcontrol, bestV0
    V0 = Vinterval[1] + j * Vstep
    #println(V0)
    bvp2 = BVProblem(RHS!, bc2!, [X0[1],X0[2], 1.0, 1.0], tspan, V0)
    sol2 = solve(bvp2, Shooting(Euler()), dt=h)
    bc2!(res, sol2.u, [], 0.0)
    if norm(res) < 1e-2
        crit=0.0
        for i = 1:size(sol2.t)[1]
            crit = crit + F(sol2.t[i], [sol2.u[i][1], sol2.u[i][2]]) * h * T
        end
        #println(crit, mincrit)
        if crit < mincrit
            mincrit = crit
            bestV0 = V0
        end
        println("V: ", V0, " crit: ", crit, " mincrit: ", mincrit, " bestV: ", bestV0, " residuals ", norm(res))
        plotall(V0)
    else
        #println("residual norm:", norm(res))
    end

end

V0 = bestV0


plotall(bestV0)
println("best V: ", V0, "best crit: ", mincrit)
