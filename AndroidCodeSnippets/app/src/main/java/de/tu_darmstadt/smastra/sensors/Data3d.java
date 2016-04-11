package de.tu_darmstadt.smastra.sensors;

import android.hardware.SensorEvent;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.Transformation;

/**
 * This class represents a 3-D data entry.
 * <br>It can be anything from Vectoring data, to Acceleration data, etc...
 *
 * @author Tobias Welther
 */
@NeedsOtherClass(Vector3d.class)
public class Data3d extends Vector3d {

    /**
     * The time this data was recorded.
     */
    private long time;

    /**
     * The Accuracy of the Data.
     */
    private float accuracy;


    public Data3d(long time, float accuracy, float x, float y, float z) {
        super(x,y,z);

        this.time = time;
        this.accuracy = accuracy;
    }


    /**
     * Generates an Data3d by an float array.
     * @param array to use.
     * @throws IllegalArgumentException if the array is null or the array does not have 3 entries.
     */
    public Data3d(long time, float accuracy, float[] array){
        super(array);

        this.time = time;
        this.accuracy = accuracy;
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
     * Creates a copy of the original data.
     * @param original to use.
     */
    public Data3d(Data3d original){
        super(original);
        
        this.time = original.time;
        this.accuracy = original.accuracy;
    }


    /**
     * Gets the Time this was recorded in MS since Linux start.
     * @return the time in MS
     */
    @Transformation("To Time")
    public long getTime() {
        return time;
    }

    /**
     * The accuracy of the data.
     * @return accuracy
     */
    @Transformation("To Accuracy")
    public float getAccuracy() {
        return accuracy;
    }


    /**
     * Creates a copy of this.
     * @return a copy.
     */
    @Transformation("Copy")
    public Data3d copy(){
        return new Data3d(this);
    }

}
