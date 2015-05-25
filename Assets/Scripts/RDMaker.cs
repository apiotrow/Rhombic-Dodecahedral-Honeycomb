using UnityEngine;
using System.Collections;

public class RDMaker : MonoBehaviour {
	Mesh mesh;
	int[] tris;
	
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear(); 

		//build RD
		setVertices();
		setTriangles();
		setUVs();

		mesh.RecalculateNormals();

		//add collider
		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh;
	}

	void setUVs(){
		Vector2[] uvs = new Vector2[mesh.vertices.Length];
		for (int i=0; i < uvs.Length; i++){
			uvs [i] = new Vector2(mesh.vertices [i].x, mesh.vertices [i].z);
		}
		
		mesh.uv = uvs;
	}

	void setVertices(){
		mesh.vertices = Coords.RDverts;
	}

	void setTriangles(){

		tris = Coords.tris;
		mesh.triangles = tris;
	}
}
