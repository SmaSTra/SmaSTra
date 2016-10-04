package de.tu_darmstadt.smastra.generator.transformation;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;

/**
 * Builder pattern for a SmaSTra Transaction.
 *
 * @author Tobias Welther
 */
public class SmaSTraTransformationBuilder {

    /**
     * The List of the inputs usable.
     */
    private List<Input> inputs = new ArrayList<>();

    /**
     * The Other classes needed.
     */
    private final List<Class<?>> needsOtherClasses = new ArrayList<>();

    /**
     * The config to use.
     */
    private final List<ConfigurationElement> config = new ArrayList<>();

    /**
     * The Proxy properties to use.
     */
    private final List<ProxyPropertyObj> proxyProperties = new ArrayList<>();

    /**
     * The extras to use.
     */
    private final List<AbstractSmaSTraExtra> extras = new ArrayList<>();

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

    /**
     * The DisplayName to use.
     */
    private String displayName;



    public SmaSTraTransformationBuilder setDescription(String description) {
        this.description = description;
        return this;
    }

    public SmaSTraTransformationBuilder setOutput(Output outputs) {
        this.output = outputs;
        return this;
    }

    public SmaSTraTransformationBuilder setMethodName(String methodName) {
        this.methodName = methodName;
        return this;
    }

    public SmaSTraTransformationBuilder setClass(Class<?> clazz) {
        this.clazz = clazz;
        return this;
    }

    public SmaSTraTransformationBuilder setStatic(boolean aStatic) {
        isStatic = aStatic;
        return this;
    }

    public SmaSTraTransformationBuilder addInput(Input input){
        this.inputs.add(input);
        return this;
    }

    public SmaSTraTransformationBuilder addNeededClass(Collection<Class<?>> otherClasses){
        this.needsOtherClasses.addAll(otherClasses);
        return this;
    }

    public SmaSTraTransformationBuilder addProxyProperties(Collection<ProxyPropertyObj> proxyProperties){
        this.proxyProperties.addAll(proxyProperties);
        return this;
    }

    public SmaSTraTransformationBuilder addConfig(Collection<ConfigurationElement> config){
        this.config.addAll(config);
        return this;
    }

    public SmaSTraTransformationBuilder addNeededClass(Class<?>[] otherClasses){
        this.needsOtherClasses.addAll(Arrays.asList(otherClasses));
        return this;
    }

    public SmaSTraTransformationBuilder addInputs(Collection<Input> inputs) {
        this.inputs.addAll(inputs);
        return this;
    }

    public SmaSTraTransformationBuilder setDisplayName(String displayName) {
        this.displayName = displayName;
        return this;
    }

    public SmaSTraTransformationBuilder addExtra(AbstractSmaSTraExtra extra){
        if(extra != null) this.extras.add(extra);
        return this;
    }

    public SmaSTraTransformationBuilder addExtras(Collection<AbstractSmaSTraExtra> extras){
        if(extras != null) this.extras.addAll(extras);
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

    public String getDisplayName() {
        return displayName;
    }

    public List<ConfigurationElement> getConfig() {
        return config;
    }

    public List<AbstractSmaSTraExtra> getExtras() {
        return extras;
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
        if(displayName == null) throw new ElementGenerationFailedException("No displayName defined.");

        return new SmaSTraTransformation(displayName, inputs, needsOtherClasses, description, output,
                methodName, clazz, isStatic, config, proxyProperties, extras);
    }
}
