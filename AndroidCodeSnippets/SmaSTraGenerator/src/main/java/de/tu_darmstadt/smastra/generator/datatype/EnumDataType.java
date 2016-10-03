package de.tu_darmstadt.smastra.generator.datatype;

/**
 * A Pojo for an Enum Data-Type.
 * @author Tobias Welther
 */

public class EnumDataType {

    /**
     * The class to extract.
     */
    public final Class<? extends Enum> enumClass;


    public EnumDataType(Class<? extends Enum> enumClass) {
        this.enumClass = enumClass;
    }


    public Class<? extends Enum> getEnumClass() {
        return enumClass;
    }
}
