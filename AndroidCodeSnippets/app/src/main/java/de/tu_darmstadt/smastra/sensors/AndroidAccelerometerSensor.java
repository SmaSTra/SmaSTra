package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;

import de.tu_darmstadt.smastra.markers.Exportable;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;

/**
 * An implementation of a sensor for the Accelerometer.
 *
 * @author Tobias Welther
 */
@Exportable
@NeedsOtherClass({ Data3d.class, Abstract3dAndroidSensor.class })
public class AndroidAccelerometerSensor extends Abstract3dAndroidSensor {


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * and a delay (defined in ).
     *
     * @param context          for the SensorManager.
     * @param samplingPeriodUs to use.
     */
    public AndroidAccelerometerSensor(Context context, int samplingPeriodUs) {
        super(context, Sensor.TYPE_ACCELEROMETER, samplingPeriodUs);
    }
}
