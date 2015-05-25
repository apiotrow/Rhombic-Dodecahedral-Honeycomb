using UnityEngine;
using System.Collections;

public static class Coords {
	public static Vector3[] RDverts = new Vector3[]{
		
		//central cube
		//top 0, 1, 2, 3
		vec(-0.5f, 0.5f, -0.5f),
		vec(-0.5f, 0.5f, 0.5f),
		vec(0.5f, 0.5f, 0.5f), 
		vec(0.5f, 0.5f, -0.5f),
		
		//bottom 4, 5, 6, 7
		vec(-0.5f, -0.5f, -0.5f),
		vec(-0.5f, -0.5f, 0.5f),
		vec(0.5f, -0.5f, 0.5f), 
		vec(0.5f, -0.5f, -0.5f),
		
		
		//surrounding cube centers
		//top 8
		vec(0, 1, 0),
		
		//front 9
		vec(0, 0, -1),
		
		//bottom 10
		vec(0, -1, 0),
		
		//right 11
		vec(1, 0, 0),
		
		//left 12
		vec(-1, 0, 0),
		
		//back 13
		vec(0, 0, 1),
	};

	public static Vector3 vec(float a, float b, float c){
		return (new Vector3(a, b, c));
	}

}
