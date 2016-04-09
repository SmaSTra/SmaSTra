package de.tu_darmstadt.smastra.generator.elements;

/**
 * This is an out-parameter.
 *
 * @author Tobias Welther
 */
public class Output {

    /**
     * A generic Void output.
     */
    public static final Output VOID_OUTPUT = new Output(Void.class);


    /**
     * The output parameter.
     */
    private final Class<?> outputParam;


    public Output(Class<?> outputParam) {
        this.outputParam = outputParam;
    }


    /**
     * Gets the Output parameter.
     * @return the output.
     */
    public Class<?> getOutputParam() {
        return outputParam;
    }
}
