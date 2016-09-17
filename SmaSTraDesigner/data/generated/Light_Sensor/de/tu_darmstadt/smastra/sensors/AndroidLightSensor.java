package de.tu_darmstadt.smastra.sensors;

import android.content.Context;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

import static android.hardware.Sensor.TYPE_LIGHT;

/**
 * @author Tobias Welther
 */
@NeedsOtherClass({ Abstract1dAndroidSensor.class })
@SensorConfig(displayName = "Light Sensor", description = "Captures the Light value in lux")
public class AndroidLightSensor extends Abstract1dAndroidSensor {


    public AndroidLightSensor(Context context) {
        super(context, TYPE_LIGHT);
    }

}
