package de.tu_darmstadt.smastra.sensors;

import android.hardware.Sensor;

/**
 * @author Tobias Welther
 */
public class AndroidLightSensorTest extends Abstract1dAndroidSensorTest {


    public AndroidLightSensorTest() {
        super(Sensor.TYPE_LIGHT);
    }



    @Override
    protected Abstract1dAndroidSensor generateSut() {
        return new AndroidLightSensor(context, 0);
    }
}
