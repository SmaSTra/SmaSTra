package de.tu_darmstadt.smastra.rest;

import java.io.IOException;
import java.io.InputStream;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import de.tu_darmstadt.smastra.SmaSTraFileRepository;


/**
 * Servlet implementation class SmaSTra
 */
@WebServlet("/add")
public class PostElement extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	/**
	 * The repo for the SmaStra system.
	 */
	private final SmaSTraFileRepository repo;

    /**
     * Default constructor. 
     */
    public PostElement() {
    	repo = SmaSTraFileRepository.get();
    }
    

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
    	String name = req.getParameter("name");
    	byte[] data = read(req.getInputStream());
    	boolean worked = repo.addNewElement(name, data);
    	resp.setStatus(worked ? 200 : 400);
    }
    
    
    
    private byte[] read(InputStream in) throws IOException{
    	byte[] data = new byte[in.available()];
    	in.read(data);
    	return data;
    }

}
