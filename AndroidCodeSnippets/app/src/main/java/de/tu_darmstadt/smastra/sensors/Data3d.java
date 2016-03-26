package de.tu_darmstadt.smastra.sensors;

import android.hardware.SensorEvent;

import de.tu_darmstadt.smastra.markers.SmaStraMethod;

/**
 * This class represents a 3-D data entry.
 * <br>It can be anything from Vectoring data, to Acceleration data, etc...
 *
 * @author Tobias Welther
 */
public class Data3d {

    /**
     * The time this data was recorded.
     */
    private final long time;

    /**
     * The Accuracy of the Data.
     */
    private final float accuracy;


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


    public Data3d(long time, float accuracy, float x, float y, float z) {
        this.time = time;
        this.accuracy = accuracy;

        this.x = x;
        this.y = y;
        this.z = z;
    }


    /**
     * Generates an Data3d by an float array.
     * @param array to use.
     * @throws IllegalArgumentException if the array is null or the array does not have 3 entries.
     */
    public Data3d(long time, float accuracy, float[] array){
        if(array == null || array.length != 3) {
            throw new IllegalArgumentException("3D data need to be created by an Array with 3 elements! " +
                    "Found " + (array == null ? "0" : array.length) + " elements.");
        }

        this.time = time;
        this.accuracy = accuracy;

        this.x = array[0];
        this.y = array[1];
        this.z = array[2];
    }

    /**
     * Generates an Event by the Sensor Event.
     * @param event to generate from.
     */
    public Data3d(SensorEvent event) {
        if(event == null || event.values == null || event.values.length != 3){
            throw new IllegalArgumentException("3D data can not be inited from an empty event.");
        }

        this.x = event.values[0];
        this.y = event.values[1];
        this.z = event.values[2];

        this.accuracy = event.accuracy;
        this.time = event.timestamp;
    }


    /**
     * Gets the X-Component of the 3D data.
     * @return the x value.
     */
    @SmaStraMethod
    public float getX() {
        return x;
    }

    /**
     * Gets the Y-Component of the 3D data.
     * @return the x value.
     */
    @SmaStraMethod
    public float getY() {
        return y;
    }

    /**
     * Gets the Z-Component of the 3D data.
     * @return the z value.
     */
    @SmaStraMethod
    public float getZ() {
        return z;
    }

    /**
     * Gets the Time this was recorded in MS since Linux start.
     * @return the time in MS
     */
    @SmaStraMethod
    public long getTime() {
        return time;
    }

    /**
     * The accuracy of the data.
     * @return accuracy
     */
    @SmaStraMethod
    public float getAccuracy() {
        return accuracy;
    }

    /**
     * Calculates the length of the 3d data.
     * <br>The length is defined by the Pythagoras law.
     * @return the length
     */
    @SmaStraMethod
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
    @SmaStraMethod
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
    @SmaStraMethod
    public double distance(Data3d other){
        return Math.sqrt(distanceSquare(other));
    }

}
