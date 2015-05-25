using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Coords {

	public static HashSet<Vector3> hiddenTriangleTester = new HashSet<Vector3>();
	public static Dictionary<Vector3, int[]> hiddenTriangles = new Dictionary<Vector3, int[]>();
	public static List<int[]> triList = new List<int[]>();

	public static List<Vector3[]> trianglesInTerrain = new List<Vector3[]>();

	public static int[] tris = new int[] {
		//TOP
		//front
		8, 3, 9,
		9, 0, 8,
		//right
		8, 2, 11,
		11, 3, 8,
		//back
		8, 1, 13,
		13, 2, 8,
		//left
		8, 0, 12,
		12, 1, 8,
		
		//BELT
		//frontright
		9, 3, 11,
		11, 7, 9,
		//backright
		11, 2, 13,
		13, 6, 11,
		//backleft
		13, 1, 12,
		12, 5, 13,
		//frontleft
		12, 0, 9,
		9, 4, 12,
		
		//BOTTOM
		//front
		10, 4, 9,
		9, 7, 10,
		//right
		10, 7, 11,
		11, 6, 10,
		//back
		10, 6, 13,
		13, 5, 10,
		//left
		10, 5, 12,
		12, 4, 10,
	};
	
	public static readonly Vector3[] RDverts = new Vector3[]{
		
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
