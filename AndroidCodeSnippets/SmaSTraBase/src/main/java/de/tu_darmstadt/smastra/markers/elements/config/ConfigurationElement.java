package de.tu_darmstadt.smastra.markers.elements.config;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

/**
 * This is a sub-Element for the Configuration Annotation.
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
public @interface ConfigurationElement {

    /**
     * The Description of the Configuration element.
     * @return the description or "UNKNOWN" if none present.
     */
    String description() default "UNKNOWN";

    /**
     * The Key to use for the configuration.
     * @return the key used.
     */
    String key();

    /**
     * Of which type the Config should be.
     * @return the class type. Default is String.
     */
    Class configClass() default String.class;
}
