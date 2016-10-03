package de.tu_darmstadt.smastra.sensors;

import android.hardware.Sensor;

import de.tu_darmstadt.smastra.sensors.hardware.AndroidAccelerometerSensor;

/**
 * Created by Tobias Welther on 26.03.2016
 */
public class AndroidAccelerometerSensorTest extends Abstract3dAndroidSensorTest {


    public AndroidAccelerometerSensorTest() {
        super(Sensor.TYPE_ACCELEROMETER);
    }


    @Override
    protected Abstract3dAndroidSensor generateSut() {
        return new AndroidAccelerometerSensor(context);
    }
}
