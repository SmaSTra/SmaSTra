package de.tu_darmstadt.smastra.markers;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This annotation tells the SmaSTra system, that another class is needed.
 * <br>For example Accelerometer listener needs 3d Data (wrapper for data).
 *
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface NeedsOtherClass {
    Class<?>[] value();
}
