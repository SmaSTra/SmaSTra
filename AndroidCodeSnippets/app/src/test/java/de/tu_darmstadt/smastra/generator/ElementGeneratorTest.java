package de.tu_darmstadt.smastra.generator;

import org.junit.Test;

import java.util.Collection;

import de.tu_darmstadt.smastra.generator.transaction.SmaSTraTransformation;

import static junit.framework.Assert.assertFalse;

/**
 * A simple test for Using the Element Generator.
 * @author Tobias Welther
 */
public class ElementGeneratorTest {


    @Test
    public void testElementsParsingWorks(){
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraTransformation> transformations = generator.readTransformationsFromClasslaoded();

        assertFalse(transformations.isEmpty());
    }
}
