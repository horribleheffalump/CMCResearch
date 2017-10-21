# Controllable Markov chains tools

A set of tools for finite-state Markov processes (MP) state estimation and control with applications to data transmission (TCP) performance evaluation and control.

## CMCTools
Algorithms for MP optimal (suboptimal) state estimation and control, all theory may be found in [1-2].

### CMCClasses
Classes for the controllable Markov chain, counting process with controllable intensity, controllable continuous process.

### SystemCPObs
Model with state dynamics given by a controllable Markov process and indirect observations given by a counting process with intensity, 
which depends on the state of the observable MP. Suboptimal filter for the MP state estimation as described in [1]].

### SystemJointObs
1. Model with additional continuous observations given by Wiener process with drift and variance dependant on the state of the MP.
2. Model with additional continuous observations and possibility of simultaneous jumps of the controlled MP and the counting process observations.
Optimal filter from [3]

## MultipleBSModel
A model for an Unmanned aerial vehicle (UAV) transmitting data through the set of base stations (BS) from [2]. 

The transmitter chooses how to allocate the traffic, 
by choosing the intensity of the data flow to each transmission channel. The transmission channel to each BS is described by a controllable
MP, the transitions intensity depends on the distance between the transmitter and the BS (the farther, the worse states are more possible). The transmitter estimates 
the channel states having only the information about the packets lost during the transmission (TCP-like protocol). These observations are given by the controllable 
counting process with intensity, which depends on the state of the channel (the worse the channel state is, the more packets are being lost). Different control policies are compared with
the one based on the suboptimal channel state estimation.

## TCPIllinoisModel
TCP Illonoise channel model with states given by a controllable MP, ack flow approximated by continuous Wiener process and losses/timeouts represented by two counting processes [3]. 
The key feature is that the losses/timeouts may occur at the same time as the MP transitions.

--under construction--

## References
[[1]](https://link.springer.com/article/10.1134/S0005117916090071) 
Miller B., Miller G., Siemenikhin K. Optimal control problem regularization for the Markov process with finite number of states and constraints // Automation and Remote Control, 2016, vol. 77, pp. 1589-1611. DOI: 10.1134/S0005117916090071.
[Researchgate](https://www.researchgate.net/publication/307946969_Optimal_control_problem_regularization_for_the_Markov_process_with_finite_number_of_states_and_constraints)

[[2]](http://www.sciencedirect.com/science/article/pii/S2405896317314544)
Miller B., Miller G., Siemenikhin K. Optimization of the Data Transmission Flow from Moving Object to Nonhomogeneous Network of Base Stations // In Preprints of the 20th World Congress The International Federation of Automatic Control, July 2017, Toulouse, France, IFAC-PapersOnLine, vol 50(1), pp. 6160-6165. 
DOI: 10.1016/j.ifacol.2017.08.981. [Researchgate](https://www.researchgate.net/publication/320465943_Optimization_of_the_Data_Transmission_Flow_from_Moving_Object_to_Nonhomogeneous_Network_of_Base_Stations)

[3] yet unpublished
