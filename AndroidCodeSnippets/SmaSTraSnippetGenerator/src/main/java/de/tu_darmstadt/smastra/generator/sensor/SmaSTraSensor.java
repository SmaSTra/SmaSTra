package de.tu_darmstadt.smastra.generator.sensor;

import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerator;
import de.tu_darmstadt.smastra.generator.SmaSTraElement;
import de.tu_darmstadt.smastra.generator.elements.Output;

/**
 * This is the representation of a Sensor
 * in the SmaSTra Framework.
 *
 * @author Tobias Welther
 */
public class SmaSTraSensor extends SmaSTraElement {

    /**
     * The Description of the Sensor.
     */
    private final String description;

    /**
     * The List of the output usable.
     */
    private final Output output;

    /**
     * The Name of the Method to get the Data.
     */
    private final String dataMethodName;


    public SmaSTraSensor(String displayName, String description, String[] androidPermissions, List<Class<?>> needsOtherClasses, Output output,
                         String dataMethodName, Class<?> clazz) {

        super(displayName, clazz, androidPermissions, needsOtherClasses);
        this.description = description;
        this.output = output;
        this.dataMethodName = dataMethodName;
    }


    public String getDisplayName() {
        return displayName;
    }

    public String getDescription() {
        return description;
    }

    public Output getOutput() {
        return output;
    }

    public String getDataMethodName() {
        return dataMethodName;
    }

    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }
}
