# -*- coding: utf-8 -*-
import matplotlib as mpl
mpl.rcParams['backend'] = 'pdf'
mpl.rc('font',**{'family':'serif'})
mpl.rc('text', usetex=True)
mpl.rc('text.latex',unicode=True)

import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.ticker import FuncFormatter

millionFormatter        = FuncFormatter(lambda x, pos:'\$%1.0fM' % (x*1e-6))
percentFormatter        = FuncFormatter(lambda x, pos:'{:.2%}'.format(x))

errorDF = pd.DataFrame({'% Diff':[ -6.12256893e-13,   1.27849915e-12,   6.29839396e-06,
                              3.38728472e-05,   6.23072435e-06,   5.03582306e-06,
                              -1.09295890e-05,   2.04080118e-04],
                    'Difference': [ -2.43408203e-01,   4.77478027e-01,   2.31911964e+06,
                                   1.26799125e+07,   2.25939726e+06,   1.55594653e+06,
                                   -3.10751878e+06,   5.58644987e+07]}
                   ,index = np.arange(2008,2016))

#sns.set_style('ticks')
fig = plt.figure(figsize=(5,2))
ax = fig.add_subplot(111)
ax2 = ax.twinx()
errorDF['% Diff'].plot(kind='bar', position=1, ax=ax, color = 'r', legend=True, label = 'Percent Error',ylim=(0,0.0005), **{'width':0.3})
errorDF.Difference.plot(kind='bar', position=0, ax=ax2,ylim=(0,80000000), legend=True, label = 'Absolute Error [secondary y-axis]', **{'width':0.3})
ax2.legend(loc= 'upper left')
ax.set_xlabel('')
ax2.set_xlabel('')
ax.legend(bbox_to_anchor= (0.286,0.85))
ax.yaxis.set_major_formatter(percentFormatter)
ax2.yaxis.set_major_formatter(millionFormatter)
ax.yaxis.set_ticks([0,0.0001,0.0002,0.0003, 0.0004])
ax2.yaxis.set_ticks([0,20000000,40000000,60000000])
fig.savefig(r'd:\dataerrors.pdf', bbox_inches='tight')