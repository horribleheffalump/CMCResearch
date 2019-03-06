include("target.jl")
include("basestation.jl")
struct UAV
           target::Target          # survaillance target
           bs::BaseStation         # data transfer channel to the base station
           X₀::Array{Float64}      # UAV position at start
           X::Array{Float64}       # current position
           V::Float64              # scalar UAV speed
           t::Float64              # current time
           h::Float64              # discritization step
           kappa::Float64          # data transfer cost
           epsilon::Float64        # minimum control
       end

function step!(uav::UAV, gamma::Float64)
    uav.t = uav.t + uav.h
    uav.X = uav.X + uav.h * uav.V * [cos(gamma), sin(gamma)]
    return uav.X
end

function u(uav::UAV, t::Float64, X::Array{Float64})
    # optimal control, depends on the state
    uu = nu(uav.target, t, X) / uav.kappa - 1.0 / l(bs, X)
    if uu > uav.epsilon
        return uu
    else
        return uav.epsilon
    end
end

function du(uav::UAV, t::Float64, X::Array{Float64})
    # optimal control derivative
    if u(uav,t,X) > uav.epsilon
        return dnu(uav.target,t,X) / uav.kappa + dl(bs, X) / (l(bs,X))^2
    else
        return [0.0, 0.0]
    end
end
