package de.tu_darmstadt.smastra.generator.sensor;

import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerator;
import de.tu_darmstadt.smastra.generator.SmaSTraElement;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;

/**
 * This is the representation of a Sensor
 * in the SmaSTra Framework.
 *
 * @author Tobias Welther
 */
public class SmaSTraSensor extends SmaSTraElement {

    /**
     * The List of the output usable.
     */
    private final Output output;

    /**
     * The Name of the Method to get the Data.
     */
    private final String dataMethodName;

    /**
     * The Method to start the sensor.
     */
    private final String startMethod;

    /**
     * The Method to stop the sensor.
     */
    private final String stopMethod;


    public SmaSTraSensor(String displayName, String description, List<Class<?>> needsOtherClasses, Output output,
                         String dataMethodName, String startMethod, String stopMethod, Class<?> clazz,
                         List<ConfigurationElement> configuration, List<ProxyPropertyObj> proxyProperties,
                         List<AbstractSmaSTraExtra> extras) {

        super(displayName, description, clazz, needsOtherClasses, configuration, proxyProperties, extras);

        this.output = output;
        this.dataMethodName = dataMethodName;
        this.startMethod = startMethod;
        this.stopMethod = stopMethod;
    }


    public String getDisplayName() {
        return displayName;
    }

    public Output getOutput() {
        return output;
    }

    public String getDataMethodName() {
        return dataMethodName;
    }

    public String getStartMethod() {
        return startMethod;
    }

    public String getStopMethod() {
        return stopMethod;
    }

    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }

}
