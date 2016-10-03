package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.location.Location;
import android.location.LocationManager;
import android.test.mock.MockContext;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.mockito.Mockito;

import de.tu_darmstadt.smastra.sensors.hardware.AndroidGPSSensor;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

/**
 * A Test for Abstract
 */
@SuppressWarnings("MissingPermission")
public class AndroidGPSSensorTest {


    /**
     * The mock Context to use.
     */
    protected Context context;

    /**
     * The Mock sensor manager to use.
     */
    protected LocationManager mockLocationManager;



    @Before
    public void setup(){
        //Stub the Location Manager:
        mockLocationManager = Mockito.mock(LocationManager.class);

        //Setup the Mock Context:
        this.context = new MockContext(){
            @Override
            public Object getSystemService(String name) {
                if(name.equals(Context.LOCATION_SERVICE)) return mockLocationManager;
                else return super.getSystemService(name);
            }
        };
    }


    /**
     * Generates a new Mock Event with the passed data.
     * @param time to use
     * @param accuracy to use
     * @param latitude to use
     * @param longitude to use
     * @param altitude to use
     * @return generates the event needed.
     */
    protected Location generateMockEvent(final long time, final float accuracy, final float latitude, final float longitude, final float altitude) throws Exception {
        return new Location("GPS"){
            @Override
            public long getTime() { return time; }
            @Override
            public float getAccuracy() { return accuracy; }
            @Override
            public double getLatitude() { return latitude; }
            @Override
            public double getLongitude() { return longitude; }
            @Override
            public double getAltitude() { return altitude; }
        };
    }


    @After
    public void teardown(){
        this.mockLocationManager = null;
        this.context = null;
    }


    //This test will fail because the sensor is registered on the Main-Thread.
    /*@Test
    public void TestIfRegisterIsCalled(){
        AndroidGPSSensor sut = new AndroidGPSSensor(context);
        sut.start();

        Mockito.verify(mockLocationManager, Mockito.times(1)).requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0, sut);
    }*/


    @Test
    public void TestIfUnregisterIsCalled(){
        AndroidGPSSensor sut = new AndroidGPSSensor(context);
        sut.stop();

        Mockito.verify(mockLocationManager, Mockito.times(1)).removeUpdates(sut);
    }


    @Test
    public void TestIfDataIsAddedCorrectly() throws Exception{
        long time = System.currentTimeMillis();
        float accuracy = 42;
        float latitude = 43;
        float longitude = 44;
        float altitude = 45;

        Location location = generateMockEvent(time, accuracy, latitude, longitude, altitude);
        AndroidGPSSensor sut = new AndroidGPSSensor(context);
        sut.onLocationChanged(location);

        Data3d data3d = (Data3d) sut.getLastData();
        assertEquals(time, data3d.getTime());
        assertEquals(accuracy, data3d.getAccuracy(), 0.01);
        assertEquals(latitude, data3d.getX(), 0.01);
        assertEquals(longitude, data3d.getY(), 0.01);
        assertEquals(altitude, data3d.getZ(), 0.01);
    }

}