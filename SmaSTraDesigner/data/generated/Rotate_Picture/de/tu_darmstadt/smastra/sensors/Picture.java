package de.tu_darmstadt.smastra.sensors;

import android.graphics.Bitmap;

import java.util.HashSet;
import java.util.Set;

import de.tu_darmstadt.smastra.imagefilters.PictureFilter;

/**
 * @author Tobias Welther
 */

public class Picture {

    /**
     * The Width of the image
     */
    private final int width;

    /**
     * The height of the image
     */
    private final int height;

    /**
     * The Texture id of the image.
     */
    private Bitmap bitmap;

    /**
     * The already applied filters.
     */
    private final Set<String> appliedFilters = new HashSet<>();


    public Picture(int width, int height, Bitmap bitmap) {
        this.width = width;
        this.height = height;
        this.bitmap = bitmap;
    }


    public Picture(Bitmap bmp) {
        this.width = bmp.getWidth();
        this.height = bmp.getHeight();
        this.bitmap = bmp;
    }


    public int getHeight() {
        return height;
    }

    public int getWidth() {
        return width;
    }


    /**
     * Gets the current Picture data.
     * @return the picture data.
     */
    public Bitmap getBitmap() {
        return bitmap;
    }

    /**
     * Applies the filter passed.
     * @param filter to apply.
     */
    public void applyFilter(PictureFilter filter){
        String filterID = filter.getFilterId();
        if(filterID == null || appliedFilters.contains(filterID)) return;

        //Call the filters by step:
        filter.applyOnBitmap(width, height, bitmap);

        //Last stept, if we need to replace the image:
        Bitmap newMap = filter.applyOnBitmapAndReplace(width, height, bitmap);
        if(newMap != null) {
            this.bitmap.recycle();
            this.bitmap = newMap;
        }

        appliedFilters.add(filterID);
    }

}
