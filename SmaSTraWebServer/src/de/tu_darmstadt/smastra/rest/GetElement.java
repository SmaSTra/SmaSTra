package de.tu_darmstadt.smastra.rest;

import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.file.Files;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import de.tu_darmstadt.smastra.SmaSTraElement;
import de.tu_darmstadt.smastra.SmaSTraFileRepository;


/**
 * Servlet implementation class SmaSTra
 */
@WebServlet("/get")
public class GetElement extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	/**
	 * The repo for the SmaStra system.
	 */
	private final SmaSTraFileRepository repo;

    /**
     * Default constructor. 
     */
    public GetElement() {
    	repo = SmaSTraFileRepository.get();
    }
    

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
    @Override
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		String element = request.getParameter("name");
		if(element == null) element = request.getHeader("name");
		
    	if(element == null){
    		response.setStatus(400); //Not Param!
    		return;
    	}
    	
    	SmaSTraElement foundElement = repo.getElement(element);
    	if(foundElement == null){
    		response.setStatus(404);
    		return;
    	}
    	
    	response.setContentType("application/octet-stream");
    	File dataFile = foundElement.getDataFile();
    	response.setContentLengthLong(dataFile.length());
    	
    	stream(dataFile, response.getOutputStream());
	}
    
    
    /**
     * Streams the file to the output stream.
     * @param file to use.
     * @param out to use.
     */
    private void stream(File file, OutputStream out) throws IOException {
		out.write(Files.readAllBytes(file.toPath()));
		out.flush();
    }

}
