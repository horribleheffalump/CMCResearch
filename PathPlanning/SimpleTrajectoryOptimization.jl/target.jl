using LinearAlgebra

struct Target
           X₀::Array{Float64}    # target position at start
           X₁::Array{Float64}    # target position at finish
           d₀::Float64    # distanse where the survailance quality
                        # is twice as lower as above the target position
       end

function d(target::Target, t::Float64, X::Array{Float64})
   #return np.linalg.norm(self.Xp(t) - X) # distance
   return norm(X-pos(target,t))
end

function pos(target::Target, t::Float64)
   return target.X₀ + t * (target.X₁ - target.X₀)
end

function nu(target::Target, t::Float64, X::Array{Float64})
   # the survaillance quality
   # X - UAV position
   return 1.0 / (1.0 + (d(target, t, X) / target.d₀)^2)
end

function dnu(target::Target, t::Float64, X::Array{Float64})
   # the derivative
   return -2.0 * (X - pos(target, t)) * (nu(target, t, X) / target.d₀)^2
end
