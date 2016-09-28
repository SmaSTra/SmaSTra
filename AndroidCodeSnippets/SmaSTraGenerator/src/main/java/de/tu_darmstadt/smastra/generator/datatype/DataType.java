package de.tu_darmstadt.smastra.generator.datatype;

/**
 * This a POJO for the DataType exported to SmaSTra.
 *
 * @author Tobias Welther
 */

public class DataType {

    /**
     * A static referece for non-Constructable Data-Types.
     */
    public static final DataType Unconstructable(Class<?> clazz) {
        return new DataType(clazz, "", new Class<?>[0], false);
    }


    /**
     * The represented Class.
     */
    private final Class<?> clazz;

    /**
     * The Template used.
     */
    private final String template;

    /**
     * The Types used for the creation.
     */
    private final Class<?>[] typeParams;

    /**
     * If this element is creatable.
     */
    private final boolean creatable;


    public DataType(Class<?> clazz, String template, Class<?>[] typeParams, boolean creatable) {
        this.clazz = clazz;
        this.template = template;
        this.typeParams = typeParams;
        this.creatable = creatable;
    }


    public Class<?> getClazz() {
        return clazz;
    }

    public String getTemplate() {
        return template;
    }

    public Class<?>[] getTypeParams() {
        return typeParams;
    }

    public boolean isCreatable() {
        return creatable;
    }
}
