package de.tu_darmstadt.smastra.markers;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is a marker Annotation to mark methods useable by SmaSTra.
 *
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.METHOD)
public @interface Transformation {
    String desctiption() default "None";
}
