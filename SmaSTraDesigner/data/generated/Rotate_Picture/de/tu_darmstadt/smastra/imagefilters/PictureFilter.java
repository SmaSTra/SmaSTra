package de.tu_darmstadt.smastra.imagefilters;

import android.graphics.Bitmap;

/**
 * This is an Interface for a Filter for a picture.
 * @author Tobias Welther
 */

public abstract class PictureFilter {

    /**
     * Applies the Filter on the Data.
     * @param  width to use
     * @param height to use
     * @param bitmap to apply on.
     */
    public void applyOnBitmap(int width , int height, Bitmap bitmap){}


    /**
     * Applies the Filter on the Data.
     * @param  width to use
     * @param height to use
     * @param bitmap to apply on.
     * @return  the new Bitmap. If null, result is ignored.
     */
    public Bitmap applyOnBitmapAndReplace(int width , int height, Bitmap bitmap){ return null; }


    /**
     * Returns a uniqueID for a filter.
     * This is to avoid using the same filter multiple times on an image!
     * @return the filter ID.
     */
    public abstract String getFilterId();

}
