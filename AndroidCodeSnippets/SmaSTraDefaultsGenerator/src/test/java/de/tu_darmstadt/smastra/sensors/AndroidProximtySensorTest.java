package de.tu_darmstadt.smastra.sensors;

import android.hardware.Sensor;

/**
 * @author Tobias Welther
 */
public class AndroidProximtySensorTest extends Abstract1dAndroidSensorTest {


    public AndroidProximtySensorTest() {
        super(Sensor.TYPE_PROXIMITY);
    }



    @Override
    protected Abstract1dAndroidSensor generateSut() {
        return new AndroidProximitySensor(context, 0);
    }
}
