package de.tu_darmstadt.smastra.generator;

import org.apache.commons.io.FileUtils;
import org.junit.Test;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.transaction.SmaSTraTransformation;

import static junit.framework.Assert.assertFalse;

/**
 * A simple test for Using the Element Generator.
 * @author Tobias Welther
 */
public class ElementGeneratorTest {


    @Test
    public void testElementsParsingWorks() throws IOException {
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraTransformation> transformations = generator.readTransformationsFromClasslaoded();

        assertFalse(transformations.isEmpty());

        File srcDir = new File(new File(new File("src"), "main"), "java");
        File targetDir = new File("generated");
        if(!targetDir.exists()) targetDir.mkdir();

        int created = 0;
        for(SmaSTraTransformation transformation : transformations){
            File tileDir = new File(targetDir, transformation.getDisplayName());

            //Create Directory:
            if(tileDir.exists()) FileUtils.forceDelete(tileDir);
            tileDir.mkdir();

            //Create the File-Listings to copy:
            List<Class<?>> classesToCopy = new ArrayList<>(transformation.getNeedsOtherClasses());
            classesToCopy.add(transformation.getClazz());

            //Write General File:
            File metaFile = new File(tileDir, "metadata.json");
            try(  PrintWriter out = new PrintWriter( metaFile )  ){
                out.println( transformation.toJsonString(generator) );
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
