package de.tu_darmstadt.smastra.rest;

import java.io.IOException;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


/**
 * Servlet to simply ping the Web-interface
 */
@WebServlet("/ping")
public class Ping extends HttpServlet {
	
	private static final long serialVersionUID = 1L;
	

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
    @Override
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
    	response.setStatus(200);
    	response.setContentType("text/plain");
    	response.getWriter().append("pong");
	}

}
