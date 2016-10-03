package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

/**
 * An implementation of a sensor for the Magnetic strength, uncalibrated
 * @author Tobias Welther
 */
@NeedsOtherClass({ Data3d.class, Abstract3dAndroidSensor.class })
@SensorConfig(displayName = "Uncalibrated Magnetic Field Sensor", description = "This measures the magnetic field strength in x,y,z direction, the values are uncalibrated.")
public class AndroidUncalibratedMagnetometerSensor extends Abstract3dAndroidSensor {


    /**
     * Generates the Sensor.
     * <br>It needs a Context for the SensorManager
     * and a delay (defined in ms).
     *
     * @param context          for the SensorManager.
     */
    public AndroidUncalibratedMagnetometerSensor(Context context) {
        super(context, Sensor.TYPE_MAGNETIC_FIELD_UNCALIBRATED);
    }

}
