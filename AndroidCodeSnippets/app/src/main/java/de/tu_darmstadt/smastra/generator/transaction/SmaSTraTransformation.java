package de.tu_darmstadt.smastra.generator.transaction;

import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerator;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;

/**
 * This is the element of the SmaSTra system.
 * It symbolises the root element of the System.
 *
 * @author Tobias Welther
 */
public class SmaSTraTransformation {

    /**
     * The Name to use for displaying.
     */
    private final String displayName;

    /**
     * The List of the inputs usable.
     */
    private final List<Input> inputs;

    /**
     * The Other classes needed.
     */
    private final List<Class<?>> needsOtherClasses;

    /**
     * The Description of the Transaction.
     */
    private final String description;

    /**
     * The List of the output usable.
     */
    private final Output output;

    /**
     * The Name of the Method.
     */
    private final String methodName;

    /**
     * The Name of the Method.
     */
    private final Class<?> clazz;

    /**
     * If the method is static.
     */
    private final boolean isStatic;



    public SmaSTraTransformation(String displayName, List<Input> inputs,
                                 List<Class<?>> needsOtherClasses, String description,
                                 Output output, String methodName, Class<?> clazz, boolean isStatic) {

        this.displayName = displayName;
        this.inputs = inputs;
        this.needsOtherClasses = needsOtherClasses;
        this.description = description;
        this.output = output;
        this.methodName = methodName;
        this.clazz = clazz;
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

    public Class<?> getClazz() {
        return clazz;
    }

    public boolean isStatic() {
        return isStatic;
    }

    public List<Class<?>> getNeedsOtherClasses() {
        return needsOtherClasses;
    }

    public String getDescription() {
        return description;
    }

    public String getDisplayName() {
        return displayName;
    }

    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }
}
