import pandas as pd
import sys
import glob
import os
import matplotlib.pyplot as plt

from matplotlib import rc
rc('font',**{'family':'serif'})
rc('text', usetex=True)
rc('text.latex',unicode=True)
rc('text.latex',preamble=r'\usepackage[T2A]{fontenc}')
rc('text.latex',preamble=r'\usepackage[utf8]{inputenc}')
rc('text.latex',preamble=r'\usepackage[russian]{babel}')

#from matplotlib import gridspec

#folder = sys.argv[1]
#protocols = sys.argv[2:]
folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_uniform"

outfilename = 'performance.txt'
pattern = 'crit_T_*.txt'
#print(sys.argv)
#print(folder)
#print(protocols)

#protocols = sys.argv[2:]

dirspattern = os.path.join(folder, '*')
dirs = glob.glob(dirspattern)
dataframes_protocols = []
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


folder = "D:\projects.git\CMCResearch\TCPIllinoisModel\out_gaussian"
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

X = df.loc['Mean_Throughput']
Y = df.loc['Loss'] / df.loc['TotalTime']
note = df.loc['alpha_min'].map("{:.2f}".format) + ',' + df.loc['alpha_max'].map("{:.2f}".format) + ';' + df.loc['beta_min'].map("{:.2f}".format) + ',' + df.loc['beta_max'].map("{:.2f}".format)

#print(note)


f = plt.figure(num=None, figsize=(5,3), dpi=200, facecolor='w', edgecolor='k')
plt.subplots_adjust(left=0.10, bottom=0.15, right=0.98, top=0.97)
f.tight_layout()
ax = f.add_subplot(111)
ax.scatter(X,Y, c='black', s=1, label='TCP NewReno модифицированный')
ax.scatter(X['ILLINOIS'],Y['ILLINOIS'],c='red', label='TCP Illinois')
ax.scatter(X['NEWRENO'],Y['NEWRENO'],c='green', label='TCP NewReno')

ax.set_xlim(20,80)
ax.set_ylim(0,4)

plt.xlabel('Пропускная способность соединения')
plt.ylabel('Потерь в секунду')
#for i, txt in enumerate(note):
#    ax.annotate(txt, (X[i], Y[i]))
plt.legend()
plt.show()
#print(df)
#df.to_csv(os.path.join(folder, outfilename))