package de.tu_darmstadt.smastra.generation;

import org.junit.Test;

import java.io.File;

import de.tu_darmstadt.smastra.generator.SmaSTraGeneratorBootstrap;

/**
 * A temporary workaround to generate Stuff.
 * @author Tobias Welther
 */
public class GenerateFilesAsTest {

    @Test
    public void generateSources() throws Throwable{
        //Create the Elements:
        SmaSTraGeneratorBootstrap.GenerateElementsTo(new File("base"));
        //SmaSTraGeneratorBootstrap.GenerateElementsTo(new File("base"), AndroidActivityReognitionSensor.class);

        //Create the DataTypes:
        SmaSTraGeneratorBootstrap.GenerateDataTypesTo(new File("datatypes"));
    }
}
