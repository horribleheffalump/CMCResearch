using LinearAlgebra

struct BaseStation
           X₀::Array{Float64}    # BS position
           r₀::Float64           # distanse where the noise is twice as higher as above the BS
       end

function r(bs::BaseStation, X::Array{Float64})
   return norm(X-bs.X₀)
end

function l(bs::BaseStation, X::Array{Float64})
   # SIR
   # X - UAV position
   return 1.0 / (1.0 + (r(bs, X) / bs.r₀)^2)
end

function dl(bs::BaseStation, X::Array{Float64})
   # the derivative
   return -2.0 * (X-bs.X₀) * (l(bs, X) / bs.r₀)^2
end
