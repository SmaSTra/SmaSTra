package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorManager;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

/**
 * An implementation of a sensor for the Accelerometer.
 *
 * @author Tobias Welther
 */
@NeedsOtherClass({ Data3d.class, Abstract3dAndroidSensor.class })
@SensorConfig(displayName = "Accelerometer Sensor", description = "The Accelerometer sensor measuring 3d-Acceleration to the Device")
public class AndroidAccelerometerSensor extends Abstract3dAndroidSensor {


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * and a delay (defined in ms).
     *
     * @param context          for the SensorManager.
     * @param samplingPeriodUs to use.
     */
    public AndroidAccelerometerSensor(Context context, int samplingPeriodUs) {
        super(context, Sensor.TYPE_ACCELEROMETER, samplingPeriodUs);
    }


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * <br>The Sampling period is set to fastest by default.
     *
     * @param context          for the SensorManager.
     */
    public AndroidAccelerometerSensor(Context context) {
        super(context, Sensor.TYPE_ACCELEROMETER, SensorManager.SENSOR_DELAY_FASTEST);
    }

}
