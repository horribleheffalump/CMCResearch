import matplotlib.pyplot as plt
import numpy as np

def arrowed_spines(fig, axs, axs_twinx = []):
    # ax[0] - the upmost graph
    # ax[-1] - the botommost graph

    xlims = list(map(lambda foo: foo.get_xlim(), axs))
    ylims = list(map(lambda foo: foo.get_ylim(), axs))
    xlims_twinx = list(map(lambda foo: foo.get_xlim(), axs_twinx))
    ylims_twinx = list(map(lambda foo: foo.get_ylim(), axs_twinx))


    # removing the default axis on all sides:
    for side in ['bottom','right','top','left']:
        for ax in axs:
            ax.spines[side].set_visible(False)
        for ax in axs_twinx:
            ax.spines[side].set_visible(False)


    # get width and height of axes object to compute 
    dps = fig.dpi_scale_trans.inverted()
    sizes = [[e.width, e.height] for e in map(lambda foo: foo.get_window_extent().transformed(dps), axs)] 
    heights = [row[1] for row in sizes]
    total_height = sum(heights)
    total_width = sizes[0][0] 

    # arrowhead width and length
    # vertical arrowhead width is 1/80 of graphs mutual width.
    # horizontal arrowhead width is calculated in accordance with real graphs sizes
    aw_u = 1./80. * (xlims[0][1] - xlims[0][0])
    aw_b = 1./80. * (ylims[-1][1] - ylims[-1][0]) * total_height / heights[-1] * total_width / total_height

    # horizontal arrowhead length is 1/40 of graphs mutual width.
    # vertical arrowhead length is calculated in accordance with real graphs sizes
    ah_b = 1./40. * (xlims[-1][1] - xlims[-1][0])
    ah_u = 1./40. * (ylims[0][1] - ylims[0][0]) * total_height / heights[0]  * total_width / total_height


    lw = 1. # axis line width
    ohg = 0.0 # arrow overhang




    # draw x and y axis
    # bottom
    axs[-1].arrow(xlims[-1][0], 0, xlims[-1][1] - xlims[-1][0], 0., fc='k', ec='k', lw = lw, 
             head_width=aw_b, head_length=ah_b, overhang = ohg, 
             length_includes_head= False, clip_on = False) 

    #top
    axs[0].arrow(0, ylims[0][0], 0., ylims[0][1] - ylims[0][0], fc='k', ec='k', lw = lw, 
             head_width=aw_u, head_length=ah_u, overhang = ohg, 
             length_includes_head= False, clip_on = False)

    for i in range(1, len(axs)):
        axs[i].arrow(0, ylims[i][0], 0., ylims[i][1] - ylims[i][0], fc='k', ec='k', lw = lw, 
                 head_width=0.0, head_length=0.0, overhang = 0.0, 
                 length_includes_head= False, clip_on = False)

    # twinx
    #top
    if len(axs_twinx) > 0:
        axs_twinx[0].arrow(xlims_twinx[0][1], ylims_twinx[0][0], 0., ylims_twinx[0][1] - ylims_twinx[0][0], fc='k', ec='k', lw = 1.0, 
                head_width=aw_u, head_length= ah_u * (ylims_twinx[0][1] - ylims_twinx[0][0]) / (ylims[0][1] - ylims[0][0]) , overhang = 0, 
                length_includes_head= False, clip_on = False)

        for i in range(1, len(axs_twinx)):
            axs_twinx[i].arrow(xlims_twinx[i][1], ylims_twinx[i][0], 0., ylims_twinx[i][1] - ylims_twinx[i][0], fc='k', ec='k', lw = lw, 
                    head_width=0.0, head_length=0.0, overhang = 0.0, 
                    length_includes_head= False, clip_on = False)

