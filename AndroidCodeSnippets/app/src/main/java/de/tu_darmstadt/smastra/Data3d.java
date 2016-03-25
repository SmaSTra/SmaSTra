package de.tu_darmstadt.smastra;

/**
 * This class represents a 3-D data entry.
 * <br>It can be anything from Vectoring data, to Acceleration data, etc...
 *
 * @author Tobias Welther
 */
public class Data3d {

    /**
     * The x value of the 3d data.
     */
    private final float x;

    /**
     * The y value of the 3d data.
     */
    private final float y;

    /**
     * The z value of the 3d data.
     */
    private final float z;


    public Data3d(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }


    /**
     * Generates an Data3d by an float array.
     * @param array to use.
     * @throws IllegalArgumentException if the array is null or the array does not have 3 entries.
     */
    public Data3d(float[] array){
        if(array == null || array.length != 3) {
            throw new IllegalArgumentException("3D data need to be created by an Array with 3 elements! " +
                    "Found " + (array == null ? "0" : array.length) + " elements.");
        }

        this.x = array[0];
        this.y = array[1];
        this.z = array[2];
    }


    /**
     * Gets the X-Component of the 3D data.
     * @return the x value.
     */
    public float getX() {
        return x;
    }

    /**
     * Gets the Y-Component of the 3D data.
     * @return the x value.
     */
    public float getY() {
        return y;
    }

    /**
     * Gets the Z-Component of the 3D data.
     * @return the z value.
     */
    public float getZ() {
        return z;
    }


    /**
     * Calculates the length of the 3d data.
     * <br>The length is defined by the Pythagoras law.
     * @return the length
     */
    public float length(){
        return (float) Math.sqrt(x*x + y*y + z*z);
    }


    /**
     * The Distance to the other data squared.
     * @param other to compare against.
     * @return the distance to the other data as square.
     *
     * @throws IllegalStateException if other is null.
     */
    public double distanceSquare(Data3d other){
        if(other == null) throw new IllegalArgumentException("Can not compare Data3d to null.");

        double dist = Math.pow(x-other.x, 2);
        dist += Math.pow(y-other.y, 2);
        dist += Math.pow(z-other.z, 2);

        return dist;
    }


    /**
     * The Distance to the other data.
     * @param other to compare against.
     * @return the distance to the other data.
     *
     * @throws IllegalStateException if other is null.
     */
    public double distance(Data3d other){
        return Math.sqrt(distanceSquare(other));
    }

}
