package de.tu_darmstadt.smastra.generator;

import org.apache.commons.io.FileUtils;
import org.junit.Test;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;

import static junit.framework.Assert.assertFalse;

/**
 * A simple test for Using the Element Generator.
 * @author Tobias Welther
 */
public class ElementGeneratorTest {


    @Test
    public void testElementsParsingWorks() throws IOException {
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraElement> elements = new HashSet<>();
        elements.addAll(generator.readTransformationsFromClassLoaded());
        elements.addAll(generator.readSensorsFromClassLoaded());

        assertFalse(elements.isEmpty());

        File srcDir = new File(new File(new File("src"), "main"), "java");
        File targetDir = new File("generated");
        if(!targetDir.exists()) targetDir.mkdir();

        int created = 0;
        for(SmaSTraElement element : elements){
            File tileDir = new File(targetDir, element.getDisplayName());

            //Create Directory:
            if(tileDir.exists()) FileUtils.forceDelete(tileDir);
            tileDir.mkdir();

            //Create the File-Listings to copy:
            List<Class<?>> classesToCopy = new ArrayList<>(element.getNeededClasses());
            classesToCopy.add(element.getElementClass());

            //Write General File:
            File metaFile = new File(tileDir, "metadata.json");
            try(  PrintWriter out = new PrintWriter( metaFile )  ){
                out.println( element.toJsonString(generator) );
                out.flush();
            }

            //Copy Needed Files:
            for(Class<?> clazz : classesToCopy){
                File source = classToFile(srcDir, clazz);
                File destination = classToFile(tileDir, clazz);

                //Create Dir:
                destination.getParentFile().mkdirs();
                FileUtils.copyFile(source, destination);
            }

            created ++;
        }

        System.out.println("Created " + created + " Transformations.");
    }


    /**
     * Reads the Path for the Class.
     * Ready to pass into a File.
     * @param clazz to parse
     * @return the path to use for Files.
     */
    private String readPath(Class<?> clazz){
        return clazz.getCanonicalName().replace(".", File.separator) + ".java";
    }


    /**
     * Parses a class to a file-path.
     * @param base to use.
     * @param clazz to parse
     * @return the parsed Class as File-Path.
     */
    private File classToFile(File base, Class<?> clazz){
        String path = clazz.getCanonicalName();
        path = path.replace(".", File.separator) + ".java";

       return new File(base, path);
    }
}
