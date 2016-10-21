package de.tu_darmstadt.smastra.rest;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


/**
 * Servlet implementation class SmaSTra
 */
@WebServlet("/exception")
public class UploadException extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	/**
	 * The Base-Folder for the exceptions.
	 */
	private final File basePath = new File("/home/SmaSTra/Exceptions");

    /**
     * Default constructor. 
     */
    public UploadException() {
    	basePath.mkdirs();
    }
    

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
    	long time = System.currentTimeMillis();    	
    	File saveFile = new File(basePath, "Error-" + time + ".exception");
    	
    	try( 	
    			FileWriter writer = new FileWriter(saveFile);
    			BufferedReader reader = req.getReader();
			){
			String line;
    	    while ((line = reader.readLine()) != null) {
    	        writer.write(line + '\r' + "" + '\n');
    	    }
    	}catch (Exception e) {
    		resp.setStatus(500);
    		e.printStackTrace(resp.getWriter());
    		return;
    	}
    	
    	resp.setStatus(200);
    }
    

}
