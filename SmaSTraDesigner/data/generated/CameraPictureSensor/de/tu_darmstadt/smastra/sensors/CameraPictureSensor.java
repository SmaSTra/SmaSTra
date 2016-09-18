package de.tu_darmstadt.smastra.sensors;

import android.Manifest;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.hardware.Camera;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.ProxyProperty;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This is a sensor class for the Camera.
 * @author Tobias Welther
 */
@NeedsOtherClass(Picture.class)
@NeedsAndroidPermissions(Manifest.permission.CAMERA)
@SensorConfig(displayName = "CameraPictureSensor", description = "Takes a picture by the camera")
public class CameraPictureSensor implements Sensor {

    /**
     * The surface view to manipulate for camera frames
     */
    private SurfaceView view;

    /**
     * The last picture present
     */
    private Picture lastPicture = new Picture(1,1, Bitmap.createBitmap(1,1, Bitmap.Config.ARGB_8888));

    /**
     * The Camera to use.
     */
    private Camera camera;


    public CameraPictureSensor(Context context){
    }


    @SensorStart
    @Override
    public void start() {
        if(view != null){
            view.getHolder().addCallback(new SurfaceHolder.Callback() {
                @Override
                public void surfaceCreated(SurfaceHolder holder) {
                    camera = Camera.open();
                    try{
                        camera.setPreviewDisplay(view.getHolder());
                    }catch (Throwable exp){
                        exp.printStackTrace();
                    }
                }

                @Override
                public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
                    Camera.Parameters params = camera.getParameters();
                    camera.setParameters(params);
                    camera.startPreview();

                    Camera.PictureCallback call = new Camera.PictureCallback() {
                        @Override
                        public void onPictureTaken(byte[] data, Camera camera) {
                            Bitmap bmp = BitmapFactory.decodeByteArray(data, 0, data.length);
                            lastPicture = new Picture(bmp);

                            camera.takePicture(null, null, this);
                        }
                    };

                    camera.takePicture(null, null, call);
                }

                @Override
                public void surfaceDestroyed(SurfaceHolder holder) {
                    camera.stopPreview();
                    camera.release();
                    camera = null;
                }
            });
        }
    }


    @SensorStop
    @Override
    public void stop() {
        if(view != null && camera != null){
            camera.stopPreview();
            camera.release();
        }
    }


    @ProxyProperty(name="setSurfaceView")
    public void setTextureView(SurfaceView view){
        this.view = view;
    }



    @Override public void configure(Map<String, Object> configuration) {}
    @Override public void configure(String key, Object value) {}


    /**
     * Gets the last Picture present.
     * @return the last picture.
     */
    @SensorOutput
    public Picture getLastData(){
        return lastPicture;
    }

}
