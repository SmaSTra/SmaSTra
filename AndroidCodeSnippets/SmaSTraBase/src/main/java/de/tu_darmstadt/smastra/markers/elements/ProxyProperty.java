package de.tu_darmstadt.smastra.markers.elements;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This annotation is to indicate a Proxy-Property.
 * This property should be given to the outside of SmaSTra.
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.METHOD)
public @interface ProxyProperty {
    String name();
}
