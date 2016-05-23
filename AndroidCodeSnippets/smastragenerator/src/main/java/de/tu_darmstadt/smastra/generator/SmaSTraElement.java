package de.tu_darmstadt.smastra.generator;

import java.util.Collection;
import java.util.HashSet;

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


    public SmaSTraElement(String displayName, String description, Class<?> elementClass) {
        this(displayName, description, elementClass, new String[0], null);
    }

    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions) {
        this(displayName, description, elementClass, androidPermissions, null);
    }


    public SmaSTraElement(String displayName, String description, Class<?> elementClass, String[] androidPermissions, Collection<Class<?>> neededClasses) {
        this.displayName = displayName;
        this.description = description;
        this.elementClass = elementClass;
        this.androidPermissions = androidPermissions == null ? new String[0] : androidPermissions;

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
     * Generates a Json String from this element.
     * <br>Uses the Generator passed.
     * @param generator to use.
     * @return the generated Json String.
     */
    public String toJsonString(ElementGenerator generator){
        return generator.getGson().toJson(this);
    }
}
