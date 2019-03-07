using LinearAlgebra

struct Threat
           X₀::Array{Float64}    # threat position at start
           X₁::Array{Float64}    # threat position at finish
           d₀::Float64           # threat factor
       end

function d(threat::Threat, t::Float64, X::Array{Float64})
   #return np.linalg.norm(self.Xp(t) - X) # distance
   return norm(X-pos(threat,t))
end

function pos(threat::Threat, t::Float64) # moves in scaled time t = [0,1]
   return threat.X₀ + t * (threat.X₁ - threat.X₀)
end

function th(threat::Threat, t::Float64, X::Array{Float64})
   # the threat value
   # X - UAV position
   return threat.d₀ / (1.0 + (d(threat, t, X))^2)
end

function dth(threat::Threat, t::Float64, X::Array{Float64})
   # the derivative
   return -2.0 *  threat.d₀ * (X - pos(threat, t)) / d(threat, t, X) * (th(threat, t, X))^2
end
