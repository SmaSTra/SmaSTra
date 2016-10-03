package de.tu_darmstadt.smastra.sensors;

import android.hardware.Sensor;

import de.tu_darmstadt.smastra.sensors.hardware.AndroidGyroscopeSensor;

/**
 * Created by Tobias Welther on 26.03.2016
 */
public class AndroidGyroscopeSensorTest extends Abstract3dAndroidSensorTest {


    public AndroidGyroscopeSensorTest() {
        super(Sensor.TYPE_GYROSCOPE);
    }


    @Override
    protected Abstract3dAndroidSensor generateSut() {
        return new AndroidGyroscopeSensor(context);
    }
}
