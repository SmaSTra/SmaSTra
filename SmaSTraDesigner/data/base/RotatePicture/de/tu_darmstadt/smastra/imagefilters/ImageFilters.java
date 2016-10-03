package de.tu_darmstadt.smastra.imagefilters;

import android.graphics.Bitmap;
import android.graphics.Matrix;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.transformation.Transformation;
import de.tu_darmstadt.smastra.sensors.Picture;

/**
 * A converter class for Image manipulation
 * @author Tobias Welther
 */
@NeedsOtherClass( value = {Picture.class, PictureFilter.class } )
public class ImageFilters implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    @Transformation(displayName = "Gray Scale Filter", description = "A Gray Scale filter for pictures.")
    public static Picture GrayScale(Picture picture){
        final String filterID = "GENERATED_GRAYS_CALE";

        picture.applyFilter(new PictureFilter() {
            @Override
            public void applyOnBitmap(int width, int height, Bitmap bitmap) {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        int rgb = bitmap.getPixel(x, y);
                        int blue = 0x0000ff & rgb;
                        int green = 0x0000ff & (rgb >> 8);
                        int red = 0x0000ff & (rgb >> 16);
                        int lum = (int) (red * 0.299 + green * 0.587 + blue * 0.114) + 0xff_00_00_00;
                        bitmap.setPixel(x, y, lum | (lum << 8) | (lum << 16));
                    }
                }
            }

            @Override
            public String getFilterId() {
                return filterID;
            }
        });

        return picture;
    }


    @Transformation(displayName = "ColorFilter", description = "Filters the Color passed (RGP).")
    public static Picture ColorFilter(Picture picture, int color){
        final String filterID = "COLOR_FILTER_" + color;

        final int filterBlue = 0x0000ff & color;
        final int filterGreen = 0x0000ff & (color >> 8);
        final int filterRed = 0x0000ff & (color >> 16);

        picture.applyFilter(new PictureFilter() {
            @Override
            public void applyOnBitmap(int width, int height, Bitmap bitmap) {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        int rgb = bitmap.getPixel(x, y);
                        int blue = Math.max(0, (0x0000ff & rgb) - filterBlue);
                        int green = Math.max(0, (0x0000ff & (rgb >> 8) - filterGreen));
                        int red = Math.max(0, (0x0000ff & (rgb >> 16) - filterRed));

                        bitmap.setPixel(x, y, ((red&0x0ff)<<16)|((green&0x0ff)<<8)|(blue&0x0ff));
                    }
                }
            }

            @Override
            public String getFilterId() {
                return filterID;
            }
        });

        return picture;
    }


    @Transformation(displayName = "Rotate Picture", description = "Rotates a picture by X degree.")
    public static Picture Rotate90(Picture picture, final double degree){
        final String filterID = "ROTATE_IMAGE_" + degree;

        picture.applyFilter(new PictureFilter() {
            @Override
            public Bitmap applyOnBitmapAndReplace(int width, int height, Bitmap bitmap) {
                Matrix matrix = new Matrix();
                matrix.postRotate((float) degree);
                return Bitmap.createBitmap(bitmap, 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, true);
            }

            @Override
            public String getFilterId() {
                return filterID;
            }
        });

        return picture;
    }

}
