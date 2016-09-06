package de.tu_darmstadt.smastra.markers.elements;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is the config Annotation for a Buffer.
 * @author Tobias Welther
 */

@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface BufferInfo {
    String displayName();
    String description() default "None";
}
