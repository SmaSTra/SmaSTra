package de.tu_darmstadt.smastra.generator.buffer;

import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerator;
import de.tu_darmstadt.smastra.generator.SmaSTraElement;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;

/**
 * This is the representation of a Collection
 * in the SmaSTra Framework.
 *
 * @author Tobias Welther
 */
public class SmaSTraBuffer extends SmaSTraElement {

    /**
     * The Name of the Method to get the Data.
     */
    private final String bufferGetMethodName;

    /**
     * The Name of the Method to add Data to the buffer.
     */
    private final String bufferAddMethodName;


    public SmaSTraBuffer(String displayName, String description, String[] androidPermissions, List<Class<?>> needsOtherClasses,
                         String bufferAddMethodName, String bufferGetMethodName, Class<?> clazz,
                         List<ConfigurationElement> configuration, List<ProxyPropertyObj> proxyProperties) {

        super(displayName, description, clazz, androidPermissions, needsOtherClasses, configuration, proxyProperties);

        this.bufferAddMethodName = bufferAddMethodName;
        this.bufferGetMethodName = bufferGetMethodName;
    }


    public String getDisplayName() {
        return displayName;
    }

    public String getBufferGetMethodName() {
        return bufferGetMethodName;
    }

    public String getBufferAddMethodName() {
        return bufferAddMethodName;
    }

    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }

}
