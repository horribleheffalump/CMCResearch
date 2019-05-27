using LinearAlgebra

struct Target
           trajectory::Array{Array{Float64,1},1}    # target position at finish
           d₀::Float64    # distanse where the survailance quality
                        # is twice as lower as above the target position
                        #X₀::Array{Float64}    # target position at start
                        #X₁::Array{Float64}    # target position at finish
       end

function d(target::Target, t::Float64, X::Array{Float64})
   #return np.linalg.norm(self.Xp(t) - X) # distance
   return norm(X-pos(target,t))
end

function pos(target::Target, t::Float64) # moves in scaled time t = [0,1]
   for i = 1:size(target.trajectory)[1]
      if t >= target.trajectory[i][1]
         t₀ = target.trajectory[i][1]
         X₀ = [target.trajectory[i][2], target.trajectory[i][3]]
         t₁ = target.trajectory[i+1][1]
         X₁ = [target.trajectory[i+1][2], target.trajectory[i+1][3]]
         return X₀ + (t-t₀) / (t₁ - t₀) * (X₁ - X₀)
      end
   end
   #return target.X₀ + t * (target.X₁ - target.X₀)
end

function nu(target::Target, t::Float64, X::Array{Float64})
   # the survaillance quality
   # X - UAV position
   return 1.0 / (1.0 + (d(target, t, X) / target.d₀)^2)
end

function dnu(target::Target, t::Float64, X::Array{Float64})
   # the derivative
   return -2.0 * (X - pos(target, t)) / d(target, t, X) * (nu(target, t, X) / target.d₀)^2
end
