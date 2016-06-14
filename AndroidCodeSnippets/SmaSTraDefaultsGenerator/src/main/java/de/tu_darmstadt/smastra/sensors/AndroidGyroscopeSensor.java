package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

/**
 * An implementation of a sensor for the Accelerometer.
 *
 * @author Tobias Welther
 */
@NeedsOtherClass({ Data3d.class, Abstract3dAndroidSensor.class })
@SensorConfig(displayName = "Gyroscope Sensor", description = "The Gyro sensor measuring 3d-circle acceleration")
public class AndroidGyroscopeSensor extends Abstract3dAndroidSensor {


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager.
     *
     * @param context          for the SensorManager.
     */
    public AndroidGyroscopeSensor(Context context) {
        super(context, Sensor.TYPE_GYROSCOPE);
    }

}
