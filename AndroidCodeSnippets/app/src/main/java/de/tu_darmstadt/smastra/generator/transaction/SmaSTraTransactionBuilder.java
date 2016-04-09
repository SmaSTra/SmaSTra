package de.tu_darmstadt.smastra.generator.transaction;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;

/**
 * Builder pattern for a SmaSTra Transaction.
 *
 * @author Tobias Welther
 */
public class SmaSTraTransactionBuilder {

    /**
     * The List of the inputs usable.
     */
    private List<Input> inputs = new ArrayList<>();

    /**
     * The Other classes needed.
     */
    private List<Class<?>> needsOtherClasses = new ArrayList<>();

    /**
     * The Description of the Transaction.
     */
    private String description = "";

    /**
     * The List of the output usable.
     */
    private Output output = Output.VOID_OUTPUT;

    /**
     * The Name of the Method.
     */
    private String methodName = null;

    /**
     * The Name of the Method.
     */
    private Class<?> clazz = null;

    /**
     * If the method is static.
     */
    private boolean isStatic = false;





    public SmaSTraTransactionBuilder setDescription(String description) {
        this.description = description;
        return this;
    }

    public SmaSTraTransactionBuilder setOutput(Output outputs) {
        this.output = outputs;
        return this;
    }

    public SmaSTraTransactionBuilder setMethodName(String methodName) {
        this.methodName = methodName;
        return this;
    }

    public SmaSTraTransactionBuilder setClass(Class<?> clazz) {
        this.clazz = clazz;
        return this;
    }

    public SmaSTraTransactionBuilder setStatic(boolean aStatic) {
        isStatic = aStatic;
        return this;
    }

    public SmaSTraTransactionBuilder addInput(Input input){
        this.inputs.add(input);
        return this;
    }

    public SmaSTraTransactionBuilder addNeededClass(Collection<Class<?>> otherClasses){
        this.needsOtherClasses.addAll(otherClasses);
        return this;
    }

    public SmaSTraTransactionBuilder addNeededClass(Class<?>[] otherClasses){
        this.needsOtherClasses.addAll(Arrays.asList(otherClasses));
        return this;
    }

    public SmaSTraTransactionBuilder addInputs(Collection<Input> inputs) {
        this.inputs.addAll(inputs);
        return this;
    }



    public List<Input> getInputs() {
        return inputs;
    }

    public List<Class<?>> getNeedsOtherClasses() {
        return needsOtherClasses;
    }

    public String getDescription() {
        return description;
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

    /**
     * Generates the Transaction.
     *
     * @throws ElementGenerationFailedException when essentials parts are missing (class, name, ...)
     * @return the generated transaction.
     */
    public SmaSTraTransformation build() throws ElementGenerationFailedException {
        if(clazz == null) throw new ElementGenerationFailedException("No class defined.");
        if(methodName == null) throw new ElementGenerationFailedException("No Method-Name defined.");

        return new SmaSTraTransformation(inputs, needsOtherClasses, description, output,
                methodName, clazz, isStatic);
    }

}
