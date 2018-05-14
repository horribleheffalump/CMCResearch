import matplotlib.pyplot as plt
import numpy as np

def arrowed_spines(fig, ax_x, ax_y):

    xmin, xmax = ax_x.get_xlim() 
    ymin, ymax = ax_y.get_ylim()
    ymin_x, ymax_x = ax_x.get_ylim()

    # removing the default axis on all sides:
    for side in ['bottom','right','top','left']:
        ax_x.spines[side].set_visible(False)
        ax_y.spines[side].set_visible(False)

    ## removing the axis ticks
    #plt.xticks([]) # labels 
    #plt.yticks([])
    #ax_x.xaxis.set_ticks_position('none') # tick markers
    #ax_y.yaxis.set_ticks_position('none')

    # get width and height of axes object to compute 
    # matching arrowhead length and width
    dps = fig.dpi_scale_trans.inverted()
    bbox_x = ax_x.get_window_extent().transformed(dps)
    bbox_y = ax_y.get_window_extent().transformed(dps)
    width, height = bbox_x.width, bbox_x.height + bbox_y.height


    # manual arrowhead width and length
    hw = 1./40.*(ymax-ymin) 
    hl = 1./40.*(xmax-xmin)
    lw = 1. # axis line width
    ohg = 0.0 # arrow overhang

    # compute matching arrowhead length and width
    yhw = hw/(ymax-ymin)*(xmax-xmin)* height/width 
    yhl = hl/(xmax-xmin)*(ymax-ymin)* width/height

    xhw = 1./40. * (ymax_x-ymin_x) *  (bbox_y.height) / bbox_x.height

    print(xhw, hl)
    print(hw, hl)
    print(yhw, yhl)

    # draw x and y axis
    ax_x.arrow(xmin, 0, xmax-xmin, 0., fc='k', ec='k', lw = lw, 
             head_width=xhw, head_length=hl, overhang = ohg, 
             length_includes_head= False, clip_on = False) 

    ax_y.arrow(0, ymin, 0., ymax-ymin, fc='k', ec='k', lw = lw, 
             head_width=yhw, head_length=yhl, overhang = ohg, 
             length_includes_head= False, clip_on = False)

    ax_x.arrow(0, ymin_x, 0., ymax_x-ymin_x, fc='k', ec='k', lw = lw, 
             head_width=0, head_length=0, overhang = 0, 
             length_includes_head= False, clip_on = False) 



