package de.tu_darmstadt.smastra.sensors;

import android.content.Context;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

import static android.hardware.Sensor.TYPE_PROXIMITY;

/**
 * @author Tobias Welther
 */
@NeedsOtherClass({ Abstract1dAndroidSensor.class })
@SensorConfig(displayName = "Proximity Sensor", description = "Captures the nearest distance for the Proximity Sensor")
public class AndroidProximitySensor extends Abstract1dAndroidSensor {


    public AndroidProximitySensor(Context context) {
        super(context, TYPE_PROXIMITY);
    }

}
