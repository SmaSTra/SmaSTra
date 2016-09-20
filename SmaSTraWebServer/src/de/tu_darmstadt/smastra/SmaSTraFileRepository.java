package de.tu_darmstadt.smastra;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.FileSystem;
import java.nio.file.FileSystems;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.logging.Level;
import java.util.logging.Logger;

import com.google.gson.JsonArray;

public class SmaSTraFileRepository {
	
	/**
	 * The instance to use as singleton.
	 */
	private static SmaSTraFileRepository instance;
	

	/**
	 * The rand to use.
	 */
	private static final Random rand = new Random();
	
	/**
	 * The Map of the 
	 */
	private final Map<String,SmaSTraElement> elements = new HashMap<>();
	
	/**
	 * The deserializer to use.
	 */
	private final SmaSTraJsonDeSerializer deserializer = new SmaSTraJsonDeSerializer();
	
	/**
	 * The path to the path-base.
	 */
	private final File basePath = new File("Elements");
	
	/**
	 * The tmp folder for saving / loading.
	 */
	private final File tmpPath = new File("TMP_DATA");
	
	/**
	 * All the json as String cache.
	 */
	private String cachedAllJson;
	
	
	/**
	 * Creates the Map of files to read.
	 */
	private SmaSTraFileRepository() {
		basePath.mkdirs();
		tmpPath.mkdirs();
		
		File[] files = basePath.listFiles();
		if(files != null){
			Logger.getGlobal().log(Level.INFO, "Loading " + files.length + " Elements to / from: " + basePath.getAbsolutePath());
			for(File file : files) {
				//try to extract if not present:
				extractMetadataFromZip(file);
				
				SmaSTraElement element = deserializer.deserialize(file);
				addNewElement(element);
			}	
		}
	}
	
	/**
	 * Gets all present Elements.
	 * @return all present files.
	 */
	public List<SmaSTraElement> getAllElements(){
		List<SmaSTraElement> elements = new ArrayList<>(this.elements.values());
		Collections.sort(elements);
		return elements;
	}
	
	/**
	 * Gets the element with that name.
	 * @param name to use.
	 * @return the element found or null if not present.
	 */
	public SmaSTraElement getElement(String name){
		if(name == null) return null;
		return elements.get(name);
	}
	
	
	/**
	 * Gets all present Elements.
	 * @return all present files.
	 */
	public String toJsonString(){
		if(cachedAllJson != null) return cachedAllJson;
		
		JsonArray array = new JsonArray();
		for(SmaSTraElement element : elements.values()){
			array.add(element.toSimpleJson());
		}
		
		this.cachedAllJson = array.toString();
		return cachedAllJson;
	}
	
	/**
	 * Adds a new Element.
	 */
	public void addNewElement(SmaSTraElement element){
		if(element != null) this.elements.put(element.getName(), element);
	}
	
	/**
	 * Adds a new Element.
	 * @param name to give it.
	 * @param data byte data for a file.
	 * @return if it worked.
	 */
	public boolean addNewElement(String name, byte[] data){
		if(elements.containsKey(name)) return false;
		
		File tmpFolder = new File(tmpPath, "tmp_" + rand.nextInt());
		tmpFolder.mkdirs();
		
		File tmpDataFile = new File(tmpFolder, "data.zip");
		File tmpMetadataFile = new File(tmpFolder, "metadata.json");
		
		try(FileOutputStream out = new FileOutputStream(tmpDataFile) ){
			out.write(data);
			out.flush();
		}catch (Exception e) {
			e.printStackTrace();
			return false;
		}
		
		//Extract folder:
		extractMetadataFromZip(tmpFolder);
		
		//If not present -> cancel
		if(!tmpMetadataFile.exists()) {
			tmpDataFile.delete();
			tmpFolder.delete();
			return false;
		}
		
		File destDir = new File(this.basePath, name);
		destDir.mkdirs();
		
		File destDataFile = new File(destDir, "data.zip");
		File destMetadataFile = new File(destDir, "metadata.json");
		tmpFolder.delete();

		try{
			Files.move(tmpDataFile.toPath(), destDataFile.toPath(), StandardCopyOption.ATOMIC_MOVE);
			Files.move(tmpMetadataFile.toPath(), destMetadataFile.toPath(), StandardCopyOption.ATOMIC_MOVE);
		}catch(IOException e){
			e.printStackTrace();
			return false;
		}
		
		SmaSTraElement newElement = deserializer.deserialize(destDir);		
		addNewElement(newElement);
		return true;
	}
	
	
	/**
	 * Extracts the Metadata from the Zip file.
	 * @param path to load from.
	 */
	private static final void extractMetadataFromZip(File path){
		File zipFile = new File(path, "data.zip");
		File metadataFile = new File(path, "metadata.json");
		
		//Already extracted or not present:
		if(!zipFile.exists() || metadataFile.exists()) return;
		
		try{
			Path zipPath = zipFile.toPath();
			FileSystem fileSystem = FileSystems.newFileSystem(zipPath, null);
			
			Path source = fileSystem.getPath("metadata.json");
			Files.copy(source, metadataFile.toPath());
		}catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * Gets the singleton as lazy init getter.
	 * @return the instance.
	 */
	public static SmaSTraFileRepository get(){
		return instance == null ? instance = new SmaSTraFileRepository() : instance;
	}
	
}
