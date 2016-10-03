package de.tu_darmstadt.smastra.markers.elements.sensors;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is a marker-Annotation for Sensors.
 * It's needed to know how to use the Sensor.
 *
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface SensorConfig {
    String displayName();
    String description() default "None";
}
