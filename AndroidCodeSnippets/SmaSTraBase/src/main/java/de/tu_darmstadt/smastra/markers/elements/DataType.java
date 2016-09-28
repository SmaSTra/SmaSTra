package de.tu_darmstadt.smastra.markers.elements;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is a marker Annotation for DataTypes to extract.
 *
 * @author Tobias Welther
 */
@Target(ElementType.CONSTRUCTOR)
@Retention(RetentionPolicy.RUNTIME)
public @interface DataType {

    /**
     * This can be set, if a custom Template is wanted.
     * By default something like this is generated: 'new Vector3d({0},{1},{2})'
     * @return the template wanted. "" if default template should be generated.
     */
    String template() default "";

    /**
     * When this is set to false, the element can NOT be created!
     * @return if the element can be created
     */
    boolean creatable() default true;
}
