package de.tu_darmstadt.smastra.rest;

import java.io.File;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;


/**
 * Servlet implementation class SmaSTra
 */
@WebServlet("/exceptions")
public class GetException extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	/**
	 * The Base-Folder for the exceptions.
	 */
	private final File basePath = new File("/home/SmaSTra/Exceptions");

    /**
     * Default constructor. 
     */
    public GetException() {
    	basePath.mkdirs();
    }
    

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
    	//Read from / to from Headers:
    	String fromName = req.getHeader("from");
    	String toName = req.getHeader("to");
    	String limitName = req.getHeader("limit");

    	long from = 0;
    	long to = Long.MAX_VALUE;
    	long limit = 10;

    	//parse from / to:
    	if(fromName != null) try{ from = Long.parseLong(fromName); }catch(Throwable exp){}
    	if(toName != null) try{ to = Long.parseLong(toName); }catch(Throwable exp){}
    	if(limitName != null) try{ limit = Long.parseLong(limitName); }catch(Throwable exp){}

    	
    	File[] files = basePath.listFiles();
    	JsonArray root = new JsonArray();
    	if(files != null && files.length > 0){
    		for(File file : files){
    			//Check if the limit is reached:
    			if(limit -- <= 0) break;
    			
    			long time = Long.parseLong(file.getName().replace("Error-", "").replaceAll(".exception", ""));
    			if(time < from || time > to) continue;
    			
    			JsonObject obj = new JsonObject();
    			obj.addProperty("content", readFile(file));
    			obj.addProperty("time", time);
    			
    			root.add(obj);
    		}
    	}
    	
    	resp.setStatus(200);
    	resp.getWriter().write(root.toString());
    }
    
    
	private static String readFile(File path) throws IOException {
		byte[] encoded = Files.readAllBytes(path.toPath());
		return new String(encoded, Charset.defaultCharset());
	}    

}
