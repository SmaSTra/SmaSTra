package de.tu_darmstadt.smastra.generation;

import org.junit.Test;

import java.io.File;

import de.tu_darmstadt.smastra.generator.SmaSTraGeneratorBootstrap;
import de.tu_darmstadt.smastra.sensors.AndroidActivityReognitionSensor;

/**
 * A temporary workaround to generate Stuff.
 * @author Tobias Welther
 */
public class GenerateFilesAsTest {

    @Test
    public void generateSources() throws Throwable{
        SmaSTraGeneratorBootstrap.Generate(new File("generated"));
        //SmaSTraGeneratorBootstrap.Generate(new File("generated"), AndroidActivityReognitionSensor.class);
    }
}
