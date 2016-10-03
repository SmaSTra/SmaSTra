package de.tu_darmstadt.smastra.generator;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;

import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;

/**
 * This is the base class for different SmaSTra Elements.
 *
 * @author Tobias Welther
 */
public abstract class SmaSTraElement {

    /**
     * The Needed Classes to use.
     */
    protected final Collection<Class<?>> neededClasses = new HashSet<>();

    /**
     * The Class for the Element.
     */
    protected final Class<?> elementClass;

    /**
     * The displayName to use.
     */
    protected final String displayName;
    /**
     * The description to show.
     */
    protected final String description;

    /**
     * The Android permissions needed.
     */
    protected final String[] androidPermissions;

    /**
     * The needed configuration.
     */
    protected final List<ConfigurationElement> configuration;

    /**
     * The Proxy properties of the Element.
     */
    protected final List<ProxyPropertyObj> proxyProperties = new ArrayList<>();

    /**
     * The extras to bundle in.
     */
    protected final List<AbstractSmaSTraExtra> extras = new ArrayList<>();


    public SmaSTraElement(String displayName, String description, Class<?> elementClass) {
        this(displayName, description, elementClass, new String[0], null, null, null, null);
    }

    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions) {
        this(displayName, description, elementClass, androidPermissions, null, null, null, null);
    }


    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions, Collection<Class<?>> neededClasses,
                          List<ConfigurationElement> config, List<ProxyPropertyObj> proxyProperties,
                          List<AbstractSmaSTraExtra> extras) {

        this.displayName = displayName;
        this.description = description;
        this.elementClass = elementClass;
        this.androidPermissions = androidPermissions == null ? new String[0] : androidPermissions;
        this.configuration = config == null ? new ArrayList<ConfigurationElement>() : config;

        if(proxyProperties != null) this.proxyProperties.addAll(proxyProperties);
        if(neededClasses != null) this.neededClasses.addAll(neededClasses);
        if(extras != null ) this.extras.addAll(extras);
    }


    /**
     * Gets the needed Classes.
     * @return the needed Classes.
     */
    public Collection<Class<?>> getNeededClasses() {
        return neededClasses;
    }

    /**
     * Returns the DisplayName to use.
     * @return the display Name.
     */
    public String getDisplayName() {
        return displayName;
    }

    /**
     * Gets the description for the Element.
     * @return the Description as String.
     */
    public String getDescription() {
        return description;
    }

    /**
     * Gets the Android permissions needed.
     * @return the needed Android permissions.
     */
    public String[] getAndroidPermissions() {
        return androidPermissions;
    }

    /**
     * The Main-Class of the Element.
     * @return the class of the Element.
     */
    public Class<?> getElementClass() {
        return elementClass;
    }

    /**
     * Gets the Extras for the Element.
     * @return the extras.
     */
    public Collection<AbstractSmaSTraExtra> getExtras(){
        return extras;
    }

    /**
     * Gets the used Configuration.
     * @return the configuration. May be empty.
     */
    public List<ConfigurationElement> getConfiguration() {
        return configuration;
    }

    /**
     * Gets all Proxy properties.
     * @return all proxy properties.
     */
    public List<ProxyPropertyObj> getProxyProperties() {
        return proxyProperties;
    }

    /**
     * Generates a Json String from this element.
     * <br>Uses the Generator passed.
     * @param generator to use.
     * @return the generated Json String.
     */
    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }
}
