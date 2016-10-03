package de.tu_darmstadt.smastra.sensors;

import android.graphics.Bitmap;

import java.util.HashSet;
import java.util.Set;

import de.tu_darmstadt.smastra.imagefilters.PictureFilter;
import de.tu_darmstadt.smastra.markers.elements.datatype.DataType;

/**
 * @author Tobias Welther
 */

public class Picture {

    /**
     * The Width of the image
     */
    private int width;

    /**
     * The height of the image
     */
    private int height;

    /**
     * The Texture id of the image.
     */
    private Bitmap bitmap;

    /**
     * The already applied filters.
     */
    private final Set<String> appliedFilters = new HashSet<>();


    @DataType(creatable = false)
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
        if(bitmap.isMutable()){
            //We need to be sure that we get a mutable bitmap. Otherwise the pixel-based approach will fail.
            filter.applyOnBitmap(width, height, bitmap);
        }

        //Last stept, if we need to replace the image:
        Bitmap newMap = filter.applyOnBitmapAndReplace(width, height, bitmap);
        if(newMap != null) {
            this.bitmap.recycle();
            this.bitmap = newMap;
            this.width = bitmap.getWidth();
            this.height = bitmap.getHeight();
        }

        appliedFilters.add(filterID);
    }

}
