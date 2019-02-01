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
    return df

#folder1 = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_uniform"
#folder2 = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_gaussian"

#df = get_df([folder1,folder2])

#X = df.loc['Mean_Throughput']
#Y = df.loc['Loss'] / df.loc['TotalTime']
#note = df.loc['alpha_min'].map("{:.2f}".format) + ',' + df.loc['alpha_max'].map("{:.2f}".format) + ';' + df.loc['beta_min'].map("{:.2f}".format) + ',' + df.loc['beta_max'].map("{:.2f}".format)

#XY = np.transpose(np.vstack([-X.values,Y.values]))
#fr = is_pareto_efficient(XY)
#m = X>60
#fr = fr & m

#folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out"
#df = get_df(folder)
#X2 = df.loc['Mean_Throughput']
#Y2 = df.loc['Loss'] / df.loc['TotalTime']


folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_test\statebased_mln"
df = get_df([folder])
Xsb = df.loc['Mean_Throughput']
Ysb = df.loc['Loss'] / df.loc['TotalTime']

folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_test\statebased_100000"
df = get_df([folder])
Xsb2 = df.loc['Mean_Throughput']
Ysb2 = df.loc['Loss'] / df.loc['TotalTime']


folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_test\illinois_mln"
df = get_df([folder])
Xil = df.loc['Mean_Throughput']
Yil = df.loc['Loss'] / df.loc['TotalTime']

folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_test\illinois_100000"
df = get_df([folder])
Xil2 = df.loc['Mean_Throughput']
Yil2 = df.loc['Loss'] / df.loc['TotalTime']

folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_test\illinois_10mln"
df = get_df([folder])
Xil3 = df.loc['Mean_Throughput']
Yil3 = df.loc['Loss'] / df.loc['TotalTime']

#print(note)


f = plt.figure(num=None, figsize=(5,3), dpi=200, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.10, bottom=0.15, right=0.98, top=0.97)
f.tight_layout()
ax = f.add_subplot(111)
#ax.scatter(X,Y, c='black', s=1, label='TCP NewReno модифицированный')
#ax.scatter(X['ILLINOIS'],Y['ILLINOIS'],c='red', label='TCP Illinois')
#ax.scatter(X['NEWRENO'],Y['NEWRENO'],c='green', label='TCP NewReno')

#ax.scatter(X[fr],Y[fr], c='orange', s=3, label='Pareto')

ax.scatter(Xsb,Ysb,c='blue', s=1, label = 'Statebased')
ax.scatter(Xsb2,Ysb2, c='cyan', s=1, label='Statebased 100000')

ax.scatter(Xil,Yil, c='red', s=1, label='Illinois mln')
ax.scatter(Xil2,Yil2, c='orange', s=1, label='Illinois 100000')
ax.scatter(Xil3,Yil3, c='magenta', s=1, label='Illinois 10mln')



#ax.set_xlim(20,80)
#ax.set_ylim(0,4)

plt.xlabel('Пропускная способность соединения')
plt.ylabel('Потерь в секунду')
#for i, txt in enumerate(note):
#    ax.annotate(txt, (X[i], Y[i]))
plt.legend()
plt.show()
#print(df)
#df.to_csv(os.path.join(folder, outfilename))