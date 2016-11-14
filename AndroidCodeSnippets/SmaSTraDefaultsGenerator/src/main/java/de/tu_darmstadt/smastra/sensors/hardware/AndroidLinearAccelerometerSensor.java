package de.tu_darmstadt.smastra.sensors.hardware;

import android.content.Context;
import android.hardware.Sensor;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.sensors.Abstract3dAndroidSensor;
import de.tu_darmstadt.smastra.sensors.Data3d;
/**
 * Created by Glen on 25.10.2016.
 */
@NeedsOtherClass({ Data3d.class, Abstract3dAndroidSensor.class })
@SensorConfig(displayName = "Linear Accelerometer Sensor", description = "The Accelerometer sensor measuring 3d-Acceleration to the Device, excluding the force of gravity")
public class AndroidLinearAccelerometerSensor extends Abstract3dAndroidSensor {


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * and an sensorType defined in the Sensor class (by int)
     * and a delay (defined in ).
     *
     * @param context    for the SensorManager.
     */
    public AndroidLinearAccelerometerSensor(Context context) {
        super(context, Sensor.TYPE_LINEAR_ACCELERATION);
    }
}
