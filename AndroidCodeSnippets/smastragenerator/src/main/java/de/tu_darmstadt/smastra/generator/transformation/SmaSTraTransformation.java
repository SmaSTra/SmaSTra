package de.tu_darmstadt.smastra.generator.transformation;

import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerator;
import de.tu_darmstadt.smastra.generator.SmaSTraElement;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;

/**
 * This is the Sensor element of the SmaSTra system.
 * It is a building component for the GUI.
 *
 * @author Tobias Welther
 */
public class SmaSTraTransformation extends SmaSTraElement {

    /**
     * The List of the inputs usable.
     */
    private final List<Input> inputs;

    /**
     * The List of the output usable.
     */
    private final Output output;

    /**
     * The Name of the Method.
     */
    private final String methodName;

    /**
     * If the method is static.
     */
    private final boolean isStatic;



    public SmaSTraTransformation(String displayName, List<Input> inputs, String[] androidPermissions,
                                 List<Class<?>> needsOtherClasses, String description,
                                 Output output, String methodName, Class<?> clazz, boolean isStatic, List< ConfigurationElement> config) {

        super(displayName, description, clazz, androidPermissions, needsOtherClasses, config);
        this.inputs = inputs;
        this.output = output;
        this.methodName = methodName;
        this.isStatic = isStatic;
    }


    public List<Input> getInputs() {
        return inputs;
    }

    public Output getOutput() {
        return output;
    }

    public String getMethodName() {
        return methodName;
    }

    public boolean isStatic() {
        return isStatic;
    }

    public String getDisplayName() {
        return displayName;
    }

    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }
}
