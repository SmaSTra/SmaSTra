package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;

/**
 * This is the basic class for a sensor.
 * <br>It reads data and passes the read data further to somewhere else.
 *
 * @author Tobias Welther
 */
public abstract class Abstract1dAndroidSensor implements SensorEventListener ,de.tu_darmstadt.smastra.markers.interfaces.Sensor {


    /**
     * The Uses sensor to read.
     */
    private final Sensor usedSensor;

    /**
     * The Sampling period for the Sensor to use.
     */
    private final int samplingPeriodUs;

    /**
     * The SensorManager to use.
     */
    private final SensorManager sensorManager;

    /**
     * A cache of the last data.
     */
    protected double lastData = 0;


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * and an sensorType defined in the Sensor class (by int)
     * and a delay (defined in ).
     *
     * @param context for the SensorManager.
     * @param sensorType to get.
     * @param samplingPeriodUs to use.
     */
    public Abstract1dAndroidSensor(Context context, int sensorType, int samplingPeriodUs) {
        this.sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        this.usedSensor = sensorManager.getDefaultSensor(sensorType);
        this.samplingPeriodUs = samplingPeriodUs;
    }


    /**
     * Gets the used sensor.
     */
    public Sensor getUsedSensor() {
        return usedSensor;
    }


    /**
     * Starts listening for Sensor data.
     */
    @Override
    public void start(){
        sensorManager.registerListener(this, usedSensor, samplingPeriodUs);
    }


    /**
     * Stops listening for data.
     */
    @Override
    public void stop(){
        sensorManager.unregisterListener(this);
    }


    /**
     * Gets the last Data received.
     * @return the last data.
     */
    public double getLastData() {
        return lastData;
    }

    /**
     * Gets the sample Rate.
     * @return the sampling rate.
     */
    public int getSamplingPeriodUs() {
        return samplingPeriodUs;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        try{
            this.lastData = event.values[0];
        }catch(IllegalArgumentException exp){
            // Just some invalid data from the sensor.
            // This may happen if it's not warmed up,
            // not initialized yet or not present at all.
        }
    }


    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        //Not needed.
    }
}
