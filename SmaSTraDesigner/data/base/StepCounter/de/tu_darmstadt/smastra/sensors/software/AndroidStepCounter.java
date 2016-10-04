package de.tu_darmstadt.smastra.sensors.software;

import android.content.Context;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.sensors.Abstract1dAndroidSensor;

/**
 * This Sensor collects the Steps of the User.
 * @author Tobias Welther
 */
@NeedsOtherClass(Abstract1dAndroidSensor.class)
@SensorConfig(displayName = "Step Counter", description = "This gives the current steps.")
public class AndroidStepCounter extends Abstract1dAndroidSensor {


    public AndroidStepCounter(Context context){
        super(context, android.hardware.Sensor.TYPE_STEP_COUNTER);
    }

}
