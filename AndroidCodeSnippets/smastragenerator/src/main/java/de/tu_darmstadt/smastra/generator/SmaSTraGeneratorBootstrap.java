package de.tu_darmstadt.smastra.generator;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.datatype.DataType;
import de.tu_darmstadt.smastra.generator.datatype.DataTypeParser;
import de.tu_darmstadt.smastra.generator.datatype.DataTypeSerializer;
import de.tu_darmstadt.smastra.generator.datatype.EnumDataType;
import de.tu_darmstadt.smastra.generator.datatype.EnumDataTypeParser;
import de.tu_darmstadt.smastra.generator.datatype.EnumDataTypeSerializer;

/**
 * This starts generating the Resources.
 * @author Tobias Welther
 */
public class SmaSTraGeneratorBootstrap {


    /**
     * The Main Bootstrap method.
     * @param args to use.
     */
    public static void main(String args[]) throws Throwable {
        File output = new File("base");
        File output2 = new File("datatypes");
        if(args.length > 0) output = new File(args[0]);
        if(args.length > 1) output = new File(args[1]);

        GenerateElementsTo(output);
        GenerateDataTypesTo(output2);
    }

    /**
     * Copies all found DataTypes to the Folder passed.
     * @param destinationFolder to copy to.
     * @throws IOException if something goes wrong.
     */
    public static void GenerateDataTypesTo(File destinationFolder) throws IOException{
        if(destinationFolder.exists()) FileUtils.deleteDirectory(destinationFolder);
        if(!destinationFolder.exists()) destinationFolder.mkdir();


        //Parse and add Normal classes:
        int normalTypes = 0;
        DataTypeSerializer serializer = new DataTypeSerializer();
        for(DataType type : DataTypeParser.getAllFromClassLoader()){
            File saveTo = new File(destinationFolder, type.getClazz().getCanonicalName() + ".json");
            if(saveTo.exists()) saveTo.delete();

            String content = serializer.serialize(type, null, null).toString();
            try(FileWriter writer = new FileWriter(saveTo)){
                IOUtils.write(content, writer);
                normalTypes++;
            }
        }

        System.out.println("Created " + normalTypes + " Normal DataTypes.");

        //Parse and add enum classes:
        int enumTypes = 0;
        EnumDataTypeSerializer enumSerializer = new EnumDataTypeSerializer();
        for(EnumDataType type : EnumDataTypeParser.getAllFromClassLoader()){
            File saveTo = new File(destinationFolder, type.getEnumClass().getCanonicalName() + ".json");
            if(saveTo.exists()) saveTo.delete();

            String content = enumSerializer.serialize(type, null, null).toString();
            try(FileWriter writer = new FileWriter(saveTo)){
                IOUtils.write(content, writer);
                enumTypes++;
            }
        }

        System.out.println("Created " + enumTypes + " Enum DataTypes.");
    }


    /***
     * Starts generating.
     * This takes some time!
     *
     * @throws IOException when something goes wrong!
     */
    public static void GenerateElementsTo(File destinationFolder) throws IOException {
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraElement> elements = generator.getAllElementsFromClassloader();

        File srcDir = new File(new File(new File("src"), "main"), "java");
        if(!destinationFolder.exists()) destinationFolder.mkdir();

        int created = 0;
        for(SmaSTraElement element : elements){
            File tileDir = new File(destinationFolder, element.getDisplayName().replace(" ", "").replace("_", ""));

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


    /***
     * Starts generating.
     * This takes some time!
     *
     * @throws IOException when something goes wrong!
     */
    public static void GenerateElementsTo(File destinationFolder, Class<?>... classes) throws IOException {
        ElementGenerator generator = new ElementGenerator();
        Collection<SmaSTraElement> elements = generator.getAllElementsFromClasses(classes);

        File srcDir = new File(new File(new File("src"), "main"), "java");
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
