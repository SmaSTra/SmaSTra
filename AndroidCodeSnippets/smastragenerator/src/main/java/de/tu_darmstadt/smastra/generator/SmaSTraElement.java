package de.tu_darmstadt.smastra.generator;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;

import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;

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


    public SmaSTraElement(String displayName, String description, Class<?> elementClass) {
        this(displayName, description, elementClass, new String[0], null, null);
    }

    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions) {
        this(displayName, description, elementClass, androidPermissions, null, null);
    }


    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions, Collection<Class<?>> neededClasses,
                          List<ConfigurationElement> config) {

        this.displayName = displayName;
        this.description = description;
        this.elementClass = elementClass;
        this.androidPermissions = androidPermissions == null ? new String[0] : androidPermissions;
        this.configuration = config == null ? new ArrayList<ConfigurationElement>() : config;

        if(neededClasses != null) this.neededClasses.addAll(neededClasses);
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
     * Gets the used Configuration.
     * @return the configuration. May be empty.
     */
    public List<ConfigurationElement> getConfiguration() {
        return configuration;
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
