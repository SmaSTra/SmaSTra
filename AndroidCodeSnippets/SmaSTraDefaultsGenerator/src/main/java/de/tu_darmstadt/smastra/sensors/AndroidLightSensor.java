package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.SensorManager;

import de.tu_darmstadt.smastra.markers.elements.SensorConfig;

import static android.hardware.Sensor.TYPE_LIGHT;

/**
 * @author Tobias Welther
 */
@SensorConfig(displayName = "Light Sensor", description = "Captures the Light value in lux")
public class AndroidLightSensor extends Abstract1dAndroidSensor {


    public AndroidLightSensor(Context context, int samplingPeriodUs) {
        super(context, TYPE_LIGHT, samplingPeriodUs);
    }

    public AndroidLightSensor(Context context) {
        super(context, TYPE_LIGHT, SensorManager.SENSOR_DELAY_FASTEST);
    }


}
