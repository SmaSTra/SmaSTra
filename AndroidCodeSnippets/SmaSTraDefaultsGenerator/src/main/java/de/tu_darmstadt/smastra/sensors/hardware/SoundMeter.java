package de.tu_darmstadt.smastra.sensors.hardware;

import android.Manifest;
import android.content.Context;
import android.media.MediaRecorder;

import java.io.IOException;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.extras.ExtraPermission;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
import de.tu_darmstadt.smastra.markers.elements.extras.ExtraPermission;
import de.tu_darmstadt.smastra.markers.elements.extras.Extras;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * Sound Detection
 *
 */
@SuppressWarnings("MissingPermission")
@Extras(permissions = @ExtraPermission(permission = Manifest.permission.RECORD_AUDIO))
@SensorConfig(displayName = "Sound Sensor", description = "Get Sound MaxAmplitude")
public class SoundMeter implements Sensor {
    private MediaRecorder mRecorder=null;


    /**
     * The Context to use.
     */
    private final Context context;
    protected double lastData = 0;

    public SoundMeter(Context context){this.context = context;}


    @SensorStart
    @Override
    public void start(){

        if(mRecorder == null){

            mRecorder = new MediaRecorder();
            mRecorder.setAudioSource(MediaRecorder.AudioSource.MIC);
            mRecorder.setOutputFormat(MediaRecorder.OutputFormat.THREE_GPP);
            mRecorder.setAudioEncoder(MediaRecorder.AudioEncoder.AMR_NB);
            mRecorder.setOutputFile("/dev/null");

            try{
                mRecorder.prepare();
                mRecorder.start();

            }catch (IllegalStateException e){
                e.printStackTrace();
            }catch (IOException e){
                e.printStackTrace();
            }

        }
    }

    @SensorStop
    @Override
    public void stop(){
        if(mRecorder != null){
            mRecorder.stop();
            mRecorder.reset();
            mRecorder.release();
            mRecorder = null;
        }
    }

    @Override
    public void configure(Map<String, Object> configuration) {

    }

    @Override
    public void configure(String key, Object value) {

    }


    @SensorOutput
    public double getLastData(){
        if(mRecorder != null){
            this.lastData = mRecorder.getMaxAmplitude();
            return lastData;

        }
        else
            return 0;
    }

}
