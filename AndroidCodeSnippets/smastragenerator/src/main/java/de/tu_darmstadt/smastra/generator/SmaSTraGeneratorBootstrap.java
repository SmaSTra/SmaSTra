package de.tu_darmstadt.smastra.generator;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;

/**
 * This starts generating the Resources.
 * @author Tobias Welther
 */
public class SmaSTraGeneratorBootstrap {


    /***
     * Starts generating.
     * This takes some time!
     *
     * @throws IOException when something goes wrong!
     */
    public static void Generate(File destinationFolder) throws IOException {
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraElement> elements = new HashSet<>();
        elements.addAll(generator.readTransformationsFromClassLoaded());
        elements.addAll(generator.readSensorsFromClassLoaded());

        File srcDir = new File(new File(new File("src"), "main"), "java");
        //File targetDir = new File("generated");
        if(!destinationFolder.exists()) destinationFolder.mkdir();

        int created = 0;
        for(SmaSTraElement element : elements){
            File tileDir = new File(destinationFolder, element.getDisplayName().replace(" ", "_"));

            //Create Directory:
            if(tileDir.exists()) FileUtils.forceDelete(tileDir);
            tileDir.mkdir();

            //Create the File-Listings to copy:
            List<Class<?>> classesToCopy = new ArrayList<>(element.getNeededClasses());
            classesToCopy.add(element.getElementClass());

            //Write General File:
            File metaFile = new File(tileDir, "metadata.json");
            PrintWriter writer = null;
            try{
                writer = new PrintWriter( metaFile );
                writer.println( element.toJsonString(generator) );
                writer.flush();
            }catch (Throwable exp){ exp.printStackTrace(); }
            finally { IOUtils.closeQuietly(writer); }

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
     * Parses a class to a file-path.
     * @param base to use.
     * @param clazz to parse
     * @return the parsed Class as File-Path.
     */
    private static File classToFile(File base, Class<?> clazz){
        String path = clazz.getCanonicalName();
        path = path.replace(".", File.separator) + ".java";

        return new File(base, path);
    }

}
