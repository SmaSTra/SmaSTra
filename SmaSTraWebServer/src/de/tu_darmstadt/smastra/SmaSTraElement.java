package de.tu_darmstadt.smastra;

import java.io.File;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

public class SmaSTraElement implements Comparable<SmaSTraElement> {

	private static final String TYPE_PATH = "type";
	private static final String NAME_PATH = "name";
	private static final String DISPLAY_NAME_PATH = "display";
	private static final String DESCRIPTION_PATH = "description";
	private static final String INPUT_PATH = "inputs";
	private static final String OUTPUT_PATH = "output";
	
	
	/**
	 * The type to use.
	 */
	private final String type;

	/**
	 * The Name of the element.
	 */
	private final String name;
	
	/**
	 * The Display name to use.
	 */
	private final String displayName;
	
	/**
	 * The Description to show.
	 */
	private final String description;
	
	/**
	 * The Inputs for the Element.
	 */
	private final String[] inputs;
	
	/**
	 * The output to use.
	 */
	private final String output;
	
	/**
	 * The Data file path.
	 */
	private final File dataFile;
	
	/**
	 * The Cached output.
	 */
	private JsonObject cachedJson;
	
	
	public SmaSTraElement(String type, String name, String displayName, 
			String description, String[] inputs, String output, File dataFile) {
		this.type = type;
		this.name = name;
		this.displayName = displayName;
		this.description = description;
		this.inputs = inputs;
		this.output = output;
		
		this.dataFile = dataFile;
	}


	/**
	 * The Data File to use.
	 * @return the File to use.
	 */
	public File getDataFile() {
		return dataFile;
	}
	
	/**
	 * Gets the name of the name of the element
	 * @return the name
	 */
	public String getName() {
		return name;
	}
	
	/**
	 * serializes to json.
	 * @return the value to json.
	 */
	public JsonObject toSimpleJson(){
		if(cachedJson != null) return cachedJson;
		
		JsonObject obj = new JsonObject();
		obj.addProperty(TYPE_PATH, type);
		obj.addProperty(NAME_PATH, name);
		obj.addProperty(DISPLAY_NAME_PATH, displayName);
		obj.addProperty(DESCRIPTION_PATH, description);
		
		JsonArray inputArray = new JsonArray();
		obj.add(INPUT_PATH, inputArray);
		for(String input : inputs){
			inputArray.add(input);
		}
		
		obj.addProperty(OUTPUT_PATH, output);
		
		this.cachedJson = obj;
		return obj;
	}
	
	
	@Override
	public int compareTo(SmaSTraElement o) {
		return name.compareTo(o.name);
	}
	
}
