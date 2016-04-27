package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;
import android.test.mock.MockContext;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.mockito.Mockito;

import java.lang.reflect.Field;

import static org.junit.Assert.assertEquals;

/**
 *
 * @author  Tobias Welther
 */
public abstract class Abstract1dAndroidSensorTest {


    /**
     * The Sensor-Type to use.
     */
    private final int sensorType;

    /**
     * The mock Context to use.
     */
    protected Context context;

    /**
     * The Mock sensor manager to use.
     */
    protected SensorManager mockSensorManager;

    /**
     * The Stub sensor used.
     */
    protected Sensor mockSensor;


    protected Abstract1dAndroidSensorTest(int sensorType){
        this.sensorType = sensorType;
    }


    @Before
    public void setup(){
        //Stub the Sensor:
        mockSensor = Mockito.mock(Sensor.class);


        //Setup the Mock SensorManager:
        mockSensorManager = Mockito.mock(SensorManager.class);
        Mockito.when(mockSensorManager.getDefaultSensor(sensorType)).thenReturn(mockSensor);


        //Setup the Mock Context:
        this.context = new MockContext(){
            @Override
            public Object getSystemService(String name) {
                if(name.equals(Context.SENSOR_SERVICE)) return mockSensorManager;
                else return super.getSystemService(name);
            }
        };
    }

    /**
     * Generates a SUT.
     * @return the sut.
     */
    protected abstract Abstract1dAndroidSensor generateSut();


    /**
     * Generates a new Mock Event with the passed data.
     * @param time to use
     * @param accuracy to use
     * @param data to use
     * @return generates the event needed.
     */
    protected SensorEvent generateMockEvent(long time, float accuracy, float data) throws Exception {
        SensorEvent sensorEvent = Mockito.mock(SensorEvent.class);
        Field field = SensorEvent.class.getDeclaredField("values");
        field.setAccessible(true);
        field.set(sensorEvent, new float[]{data});

        sensorEvent.timestamp = time;
        sensorEvent.accuracy = (int) accuracy;
        sensorEvent.sensor = mockSensor;

        return sensorEvent;
    }


    @After
    public void teardown(){
        this.mockSensor = null;
        this.mockSensorManager = null;
        this.context = null;
    }


    @Test
    public void TestIfSensorIsSetCorrectly(){
        Abstract1dAndroidSensor sut = generateSut();
        assertEquals(mockSensor, sut.getUsedSensor());
    }


    @Test
    public void TestIfRegisterIsCalled(){
        Abstract1dAndroidSensor sut = generateSut();
        sut.start();

        Mockito.verify(mockSensorManager, Mockito.times(1)).registerListener(sut, mockSensor, sut.getSamplingPeriodUs());
    }


    @Test
    public void TestIfUnregisterIsCalled(){
        Abstract1dAndroidSensor sut = generateSut();
        sut.stop();

        Mockito.verify(mockSensorManager, Mockito.times(1)).unregisterListener(sut);
    }


    @Test
    public void TestIfDataIsAddedCorrectly() throws Exception{
        long time = System.currentTimeMillis();
        float accuracy = 42;
        float data = 42;

        SensorEvent sensorEvent = generateMockEvent(time, accuracy, data);
        Abstract1dAndroidSensor sut = generateSut();
        sut.onSensorChanged(sensorEvent);

        double value = sut.getLastData();
        assertEquals(data, value, 0.01);
    }


}
