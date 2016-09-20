package de.tu_darmstadt.smastra.rest;

import java.io.IOException;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import de.tu_darmstadt.smastra.SmaSTraFileRepository;


/**
 * Servlet implementation class SmaSTra
 */
@WebServlet("/all")
public class Elements extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	
	/**
	 * The repo for the SmaStra system.
	 */
	private final SmaSTraFileRepository repo;

    /**
     * Default constructor. 
     */
    public Elements() {
    	repo = SmaSTraFileRepository.get();
    }
    

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
    @Override
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
    	response.setContentType("application/json");
    	response.getWriter().append(repo.toJsonString());    	
	}

}
