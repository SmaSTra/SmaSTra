package de.tu_darmstadt.smastra.generator.elements;

/**
 * This is an out-parameter.
 *
 * @author Tobias Welther
 */
public class Output {

    /**
     * The output parameter.
     */
    private final Class<?> outputParam;


    /**
     * The name of the method.
     */
    private final String name;



    public Output(Class<?> outputParam, String name) {
        this.outputParam = outputParam;
        this.name = name;
    }


    /**
     * Gets the Output parameter.
     * @return the output.
     */
    public Class<?> getOutputParam() {
        return outputParam;
    }

    /**
     * Returns the name of the method.
     * @return the name.
     */
    public String getName() {
        return name;
    }
}
