import pandas as pd
import sys
import glob
import os
import matplotlib.pyplot as plt
import numpy as np

from matplotlib import rc
rc('font',**{'family':'serif'})
rc('text', usetex=True)
rc('text.latex',unicode=True)
rc('text.latex',preamble=r'\usepackage[T2A]{fontenc}')
rc('text.latex',preamble=r'\usepackage[utf8]{inputenc}')
rc('text.latex',preamble=r'\usepackage[russian]{babel}')


def is_pareto_efficient(costs, return_mask = True):
    """
    Find the pareto-efficient points
    :param costs: An (n_points, n_costs) array
    :param return_mask: True to return a mask
    :return: An array of indices of pareto-efficient points.
        If return_mask is True, this will be an (n_points, ) boolean array
        Otherwise it will be a (n_efficient_points, ) integer array of indices.
    """
    is_efficient = np.arange(costs.shape[0])
    n_points = costs.shape[0]
    next_point_index = 0  # Next index in the is_efficient array to search for
    while next_point_index<len(costs):
        nondominated_point_mask = np.any(costs<costs[next_point_index], axis=1)
        nondominated_point_mask[next_point_index] = True
        is_efficient = is_efficient[nondominated_point_mask]  # Remove dominated points
        costs = costs[nondominated_point_mask]
        next_point_index = np.sum(nondominated_point_mask[:next_point_index])+1
    if return_mask:
        is_efficient_mask = np.zeros(n_points, dtype = bool)
        is_efficient_mask[is_efficient] = True
        return is_efficient_mask
    else:
        return is_efficient

def get_df(folders):
    pattern = 'crit_T_*.txt'
    dataframes_protocols = []
    for folder in folders:
        dirspattern = os.path.join(folder, '*')
        dirs = glob.glob(dirspattern)
        for dir in dirs:
            protocolname = os.path.basename(dir)
            print(protocolname)
            #fullpattern = os.path.join(folder, protocolname, pattern)
            fullpattern = os.path.join(dir, pattern)
            crits = glob.glob(fullpattern)
            #print(crits)
            dataframes_crits = []
            for crit in crits:
        #        #print(crit)
                cdf = pd.read_csv(crit, header=None, dtype=float, names = [protocolname], engine='python') ## TODO: problem with unicode strings
                cdf['Crit'] = crit.replace(fullpattern.replace('*.txt',''),'').replace('.txt','')
                dataframes_crits.append(cdf)

            if 'STATEBASED_' in protocolname:
                pars = protocolname.replace('STATEBASED_RAND_','').replace(',','.').split('_')
                cdf = pd.DataFrame(data={'Crit':'alpha_min', protocolname: float(pars[0])}, index=[0])
                dataframes_crits.append(cdf)
                cdf = pd.DataFrame(data={'Crit':'alpha_max', protocolname: float(pars[1])}, index=[0])
                dataframes_crits.append(cdf)
                cdf = pd.DataFrame(data={'Crit':'beta_min', protocolname: float(pars[2])}, index=[0])
                dataframes_crits.append(cdf)
                cdf = pd.DataFrame(data={'Crit':'beta_max', protocolname: float(pars[3])}, index=[0])
                dataframes_crits.append(cdf)

            dfp = pd.concat(dataframes_crits,ignore_index = True)
            dfp = dfp.set_index('Crit')
            dataframes_protocols.append(dfp)
    df = pd.concat(dataframes_protocols, axis=1)
    return df.transpose()

#folder1 = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_uniform"
#folder2 = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_gaussian"

#df = get_df([folder1,folder2])


#X = df.loc['Mean_Throughput']
#Y = df.loc['Loss'] / df.loc['TotalTime']
#note = df.loc['alpha_min'].map("{:.2f}".format) + ',' + df.loc['alpha_max'].map("{:.2f}".format) + ';' + df.loc['beta_min'].map("{:.2f}".format) + ',' + df.loc['beta_max'].map("{:.2f}".format)



#folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out"
#df = get_df(folder)
#X2 = df.loc['Mean_Throughput']
#Y2 = df.loc['Loss'] / df.loc['TotalTime']

folder = r"D:\\projects.git\\CMCResearch\\TCPIllinoisModel\\out_for_CDC\\"
files = glob.glob(os.path.join(folder, '*.txt'))
dfs = []
for f in files:
    d = pd.read_csv(os.path.join(folder, f), header=0, delimiter = " ", dtype=str, engine='python') ## TODO: problem with unicode strings
    dfs.append(d)

df = pd.concat(dfs, axis=0)

for c in df.columns:
    if c == 'protocol':
        df[[c]] = df[[c]].astype(str)
    else:
        df[[c]] = df[[c]].astype(float)

X = df['Mean_Throughput']
Y = df['Loss'] / df['TotalTime']

XY = np.transpose(np.vstack([-X.values,Y.values]))
fr = is_pareto_efficient(XY)

#g = Y<0.7
#fr = fr & g

Xbest = df[fr]['Mean_Throughput']
Ybest = df[fr]['Loss'] / df[fr]['TotalTime']



#best = df[df['Mean_Throughput'] > 66][['Mean_Throughput', 'Loss', 'alpha_min', 'alpha_max',  'beta_min', 'beta_max']]
#best['Loss'] = best['Loss'] / best['TotalTime']
#best = best[best['Loss'] < 2.0]
#best = best.set_index('Mean_Throughput')
#best = best.sort_values('Mean_Throughput')

folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_for_CDC\ILLINOIS_STANDARD_STATS"
df = get_df([folder])
Xil = df['Mean_Throughput'].mean()
Yil = (df['Loss'] / df['TotalTime']).mean()

folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_for_CDC\STATEBASED_STANDARD_STATS"
df = get_df([folder])
Xsb = df['Mean_Throughput'].mean()
Ysb = (df['Loss'] / df['TotalTime']).mean()


f = plt.figure(num=None, figsize=(5,2.5), dpi=150, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.10, bottom=0.17, right=0.98, top=0.97)
f.tight_layout()
ax = f.add_subplot(111)
#ax.scatter(X,Y, c='black', s=1, label='TCP NewReno модифицированный')
#ax.scatter(X['ILLINOIS'],Y['ILLINOIS'],c='red', label='TCP Illinois')
#ax.scatter(X['NEWRENO'],Y['NEWRENO'],c='green', label='TCP NewReno')

#ax.scatter(X[fr],Y[fr], c='orange', s=3, label='Pareto')

ax.scatter(X,Y, c='black', s=1, label='Statebased - random params')
ax.scatter(Xbest,Ybest, c='blue', s=40, marker='.', label='Statebased - Pareto frontier')
ax.scatter(Xil,Yil, c='red', s=40, marker= 'h', label='Illinois - original params')
#ax.scatter(Xsb,Ysb, c='green', s=40, marker= 'h', label='Statebased original params')




plt.xlabel('Average throughput')
plt.ylabel('Average loss intensity')
#for i, txt in enumerate(note):
#    ax.annotate(txt, (X[i], Y[i]))
ax.set_ylim(0.3,1)
ax.set_xlim(60,76)

plt.legend()
plt.show()
#print(df)
#df.to_csv(os.path.join(folder, outfilename))

