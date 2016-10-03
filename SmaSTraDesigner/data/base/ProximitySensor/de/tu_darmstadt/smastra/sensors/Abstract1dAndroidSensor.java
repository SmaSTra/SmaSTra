package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.config.Configuration;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
import de.tu_darmstadt.smastra.utils.ConfigParserUtils;

/**
 * This is the basic class for a sensor.
 * <br>It reads data and passes the read data further to somewhere else.
 *
 * @author Tobias Welther
 */
@Configuration(elements =  {
        @ConfigurationElement(key = Abstract3dAndroidSensor.CONFIG_SAMPLING_RATE, configClass = Integer.class, description = "Sets the sampling rate. This is the delay in Miliseconds between 2 Events.")
})
public abstract class Abstract1dAndroidSensor implements SensorEventListener ,de.tu_darmstadt.smastra.markers.interfaces.Sensor {

    //Static config fields:
    protected static final String CONFIG_SAMPLING_RATE = "SamplingRate";


    /**
     * The Uses sensor to read.
     */
    private final Sensor usedSensor;

    /**
     * The Sampling period for the Sensor to use.
     */
    private int samplingPeriodUs;

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
     * and an sensorType defined in the Sensor class (by int).
     *
     * @param context for the SensorManager.
     * @param sensorType to get.
     */
    public Abstract1dAndroidSensor(Context context, int sensorType) {
        this.sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        this.usedSensor = sensorManager.getDefaultSensor(sensorType);
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
    @SensorStart
    @Override
    public void start(){
        sensorManager.registerListener(this, usedSensor, samplingPeriodUs);
    }


    /**
     * Stops listening for data.
     */
    @SensorStop
    @Override
    public void stop(){
        sensorManager.unregisterListener(this);
    }


    @Override
    public void configure(Map<String,Object> config){
        this.samplingPeriodUs = ConfigParserUtils.parseInt(config.get(CONFIG_SAMPLING_RATE), this.samplingPeriodUs);
    }

    @Override
    public void configure(String key, Object value) {
        if(CONFIG_SAMPLING_RATE.equals(key)){
            this.samplingPeriodUs = ConfigParserUtils.parseInt(value, this.samplingPeriodUs);
        }
    }

    /**
     * Gets the last Data received.
     * @return the last data.
     */
    @SensorOutput
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
