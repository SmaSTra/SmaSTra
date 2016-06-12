package de.tu_darmstadt.smastra.markers.elements;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is a container for the Configuration of a SmaSTra Element.
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface Configuration {

    /**
     * These are the elements in the Configuration.
     * @return the elements in the Configuration needed. Default: none.
     */
    ConfigurationElement[] elements() default {};
}
