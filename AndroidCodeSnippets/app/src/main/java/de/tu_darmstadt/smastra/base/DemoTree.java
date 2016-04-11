package de.tu_darmstadt.smastra.base;

import android.content.Context;
import android.hardware.SensorManager;

import de.tu_darmstadt.smastra.math.Math3d;
import de.tu_darmstadt.smastra.sensors.AndroidAccelerometerSensor;
import de.tu_darmstadt.smastra.sensors.Vector3d;

/**
 * Created by Tobias Welther on 31.03.2016
 */
public class DemoTree extends SmaSTraTreeExecutor<Vector3d> {


    /**
     * Creates the SmaSTra Execution Tree.
     * @param context  to use.
     */
    public DemoTree(Context context) {
        super(6, context);
    }


    protected void initSensors(){
        sensor0 = new AndroidAccelerometerSensor(context, SensorManager.SENSOR_DELAY_FASTEST);
        sensor0.startListening();

        sensor1 = new AndroidAccelerometerSensor(context, SensorManager.SENSOR_DELAY_FASTEST);
        sensor1.startListening();

        sensor2 = new AndroidAccelerometerSensor(context, SensorManager.SENSOR_DELAY_FASTEST);
        sensor2.startListening();
    }


    @Override
    protected void transform(int level) {
        switch (level){
            case 0 : transform0(); break;
            case 1 : transform1(); break;
            case 2 : transform2(); break;
            case 3 : transform3(); break;
            case 4 : transform4(); break;
            case 5 : transform5(); break;
        }
    }


    private Vector3d resultTransform0;
    private AndroidAccelerometerSensor sensor0;
    private AndroidAccelerometerSensor sensor1;

    private void transform0(){
        Vector3d data1 = sensor0.getLastData();
        Vector3d data2 = sensor1.getLastData();
        resultTransform0 = Math3d.add(data1,data2);
    }


    private Vector3d resultTransform1;
    private AndroidAccelerometerSensor sensor2;

    private void transform1(){
        Vector3d data1 = resultTransform0;
        Vector3d data2 = sensor2.getLastData();
        resultTransform1 = Math3d.add(data1,data2);
    }


    private Vector3d resultTransform2;

    private void transform2(){
        Vector3d data1 = resultTransform1;
        Vector3d data2 = resultTransform0;
        resultTransform2 = Math3d.subtract(data1,data2);
    }


    private Vector3d resultTransform3;

    private void transform3(){
        Vector3d data1 = resultTransform2;
        double data2 = 7;
        resultTransform3 = Math3d.multiply(data1,data2);
    }


    private double resultTransform4;

    private void transform4(){
        Vector3d data1 = resultTransform3;
        resultTransform4 = Math3d.length(data1);
    }

    
    private void transform5(){
        Vector3d data1 = resultTransform2;
        double data2 = resultTransform4;
        data = Math3d.multiply(data1, data2);
    }

}
