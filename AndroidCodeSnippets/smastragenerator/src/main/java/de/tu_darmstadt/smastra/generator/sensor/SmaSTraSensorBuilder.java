package de.tu_darmstadt.smastra.generator.sensor;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;

/**
 * Builder pattern for a SmaSTra Sensor.
 *
 * @author Tobias Welther
 */
public class SmaSTraSensorBuilder {

    /**
     * The Other classes needed.
     */
    private final List<Class<?>> needsOtherClasses = new ArrayList<>();

    /**
     * The List of the configuration.
     */
    private final List<ConfigurationElement> configuration = new ArrayList<>();

    /**
     * The Proxy-Properties to use.
     */
    private final List<ProxyPropertyObj> proxyProperties = new ArrayList<>();

    /**
     * The list of all extras to add.
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
    private Class<?> clazz = null;

    /**
     * The DisplayName to use.
     */
    private String displayName;

    /**
     * The Name of the Method to get the Data from.
     */
    private String dataMethodName;

    /**
     * The Method to start the sensor.
     */
    private String startMethod = "";

    /**
     * The Method to stop the sensor.
     */
    private String stopMethod = "";



    public SmaSTraSensorBuilder setDescription(String description) {
        this.description = description;
        return this;
    }

    public SmaSTraSensorBuilder setOutput(Output outputs) {
        this.output = outputs;
        return this;
    }

    public SmaSTraSensorBuilder setClass(Class<?> clazz) {
        this.clazz = clazz;
        return this;
    }

    public SmaSTraSensorBuilder addNeededClass(Collection<Class<?>> otherClasses){
        this.needsOtherClasses.addAll(otherClasses);
        return this;
    }

    public SmaSTraSensorBuilder addProxyProperties(Collection<ProxyPropertyObj> proxyProperties){
        this.proxyProperties.addAll(proxyProperties);
        return this;
    }

    public SmaSTraSensorBuilder addNeededClass(Class<?>[] otherClasses){
        this.needsOtherClasses.addAll(Arrays.asList(otherClasses));
        return this;
    }

    public SmaSTraSensorBuilder setDisplayName(String displayName) {
        this.displayName = displayName;
        return this;
    }

    public SmaSTraSensorBuilder setDataMethodName(String methodName) {
        this.dataMethodName = methodName;
        return this;
    }

    public SmaSTraSensorBuilder addConfigurationElements(List<ConfigurationElement> elements){
        if(elements != null) this.configuration.addAll(elements);
        return this;
    }

    public SmaSTraSensorBuilder addExtra(AbstractSmaSTraExtra extra){
        if(extra != null) this.extras.add(extra);
        return this;
    }

    public SmaSTraSensorBuilder addExtras(Collection<AbstractSmaSTraExtra> extras){
        if(extras != null) this.extras.addAll(extras);
        return this;
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

    public Class<?> getClazz() {
        return clazz;
    }

    public String getDisplayName() {
        return displayName;
    }

    public String getDataMethodName() {
        return dataMethodName;
    }

    public List<ConfigurationElement> getConfiguration() {
        return configuration;
    }

    public List<AbstractSmaSTraExtra> getExtras() {
        return extras;
    }

    public void setStartMethod(String startMethod) {
        if(startMethod == null) return;
        this.startMethod = startMethod;
    }

    public void setStopMethod(String stopMethod) {
        if(stopMethod == null) return;
        this.stopMethod = stopMethod;
    }


    /**
     * Generates the Sensor.
     *
     * @throws ElementGenerationFailedException when essentials parts are missing (class, display name)
     * @return the generated Sensor.
     */
    public SmaSTraSensor build() throws ElementGenerationFailedException {
        if(clazz == null) throw new ElementGenerationFailedException("No class defined.");
        if(displayName == null) throw new ElementGenerationFailedException("No displayName defined.");
        if(output == null) throw new ElementGenerationFailedException("No Output defined.");
        if(dataMethodName == null) throw new ElementGenerationFailedException("No MethodName defined.");

        return new SmaSTraSensor(displayName, description, needsOtherClasses,
                output, dataMethodName, startMethod, stopMethod, clazz,
                configuration, proxyProperties, extras);
    }
}
