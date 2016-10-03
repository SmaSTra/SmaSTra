package de.tu_darmstadt.smastra.markers.elements.extras;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is the Container for Extras to add.
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface Extras {

    /**
     * This are the extra Broadcasts to bundle in.
     */
    ExtraBroadcast[] broadcasts() default {};

    /**
     * This are the extra Services to bundle in.
     */
    ExtraBroadcast[] services() default {};

    /**
     * This are the extra Libs to bundle in.
     */
    ExtraLibrary[] libraries() default {};
}
