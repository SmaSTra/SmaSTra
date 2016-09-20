package de.tu_darmstadt.smastra;

import java.io.File;
import java.io.FileReader;
import java.util.Iterator;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonParser;

public class SmaSTraJsonDeSerializer {

	
	public SmaSTraElement deserialize(File folder) {		
		File metaFile = new File(folder, "metadata.json");
		File dataFile = new File(folder, "data.zip");
		String name = folder.getName();
		
		if(!metaFile.exists() || !dataFile.exists()) return null;
		
		try(FileReader reader = new FileReader(metaFile)){
			JsonElement element = new JsonParser().parse(reader);
			if(!element.isJsonObject()) return null;
			
			return deserialize(name, dataFile, element.getAsJsonObject());
		}catch(Exception exp){
			exp.printStackTrace();
			return null;
		}
	}
	
	
	public SmaSTraElement deserialize(String name, File dataFile, JsonObject obj)
			throws JsonParseException {		
		try{
			String type = obj.get("type").getAsString();
			String displayName = obj.get("display").getAsString();
			String description = obj.get("description").getAsString();
			
			JsonObject inputsObj = obj.has("input") 
					? obj.get("input").getAsJsonObject() 
					: new JsonObject();
			
			String[] inputs = new String[inputsObj.size()];
			Iterator<java.util.Map.Entry<String, JsonElement>> it = inputsObj.entrySet().iterator();
			for(int i = 0; i < inputsObj.size(); i++){
				inputs[i] = it.next().getValue().toString();
			}
			
			String output = obj.has("output") 
					? obj.get("output").getAsString() 
					: "NONE";
			
			return new SmaSTraElement(type, name, displayName, description, inputs, output, dataFile);
		}catch (Exception e) {
			e.printStackTrace();
			return null;
		}		
	}

	
	
}
