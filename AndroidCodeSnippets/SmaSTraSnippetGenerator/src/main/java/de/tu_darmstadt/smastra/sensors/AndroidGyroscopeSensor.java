package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;

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
     * <br>It needs a Context for the SensorManager
     * and a delay (defined in ).
     *
     * @param context          for the SensorManager.
     * @param samplingPeriodUs to use.
     */
    public AndroidGyroscopeSensor(Context context, int samplingPeriodUs) {
        super(context, Sensor.TYPE_GYROSCOPE, samplingPeriodUs);
    }

    @Override
    @SensorOutput
    public Vector3d getLastData() {
        return super.getLastData();
    }
}
