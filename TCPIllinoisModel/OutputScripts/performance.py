import pandas as pd
import sys
import glob
import os

folder = sys.argv[1]
protocols = sys.argv[2:]

outfilename = 'performance.txt'
pattern = 'crit_T_*.txt'
#print(sys.argv)
#print(folder)
#print(protocols)

dataframes_protocols = []
for protocolname in protocols:
    fullpattern = os.path.join(folder, protocolname, pattern)
    crits = glob.glob(fullpattern)
    #print(protocolname)
    #print(crits)
    dataframes_crits = []
    for crit in crits:
        cdf = pd.read_csv(crit, header=None, dtype=float, names = [protocolname], )
        cdf['Crit'] = crit.replace(fullpattern.replace('*.txt',''),'').replace('.txt','')
        dataframes_crits.append(cdf)
    dfp = pd.concat(dataframes_crits,ignore_index = True)
    dfp = dfp.set_index('Crit')
    dataframes_protocols.append(dfp)

df = pd.concat(dataframes_protocols, axis=1) 
print(df)
df.to_csv(os.path.join(folder, outfilename))