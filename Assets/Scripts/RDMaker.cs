using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RDMaker : MonoBehaviour {
	Mesh mesh;

	/*
	 * Build the Rhombic Dodecahedron 
	 */
	public void buildIt(Vector3 pos){
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear(); 
		
		//build RD
		setVertices();
		setTriangles(pos);
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
		mesh.vertices = Globals.RDverts;
	}

	/*
	 * Finds which triangles are underground and removes them
	 */ 
	void setTriangles(Vector3 pos){
		List<int[]> trisInThisRD = new List<int[]>(); //triangles that will be rendered

		for(int p = 0; p < Globals.tris.Length; p += 3){
			Vector3 faceAvg = Globals.getTriangleAverage(pos, p);

			//if triangle isn't supposed to be hidden, add it to trisInThisRD
			if(!Globals.hiddenTriangles.ContainsKey(faceAvg)){
				trisInThisRD.Add(new int[]{Globals.tris[p], Globals.tris[p + 1], Globals.tris[p + 2]});
			}
		}

		//dump the tri list into an array and assign it to mesh.triangles
		int[] newTriArray = new int[(trisInThisRD.Count * 3)];
		int b = -1;
		for(int k = 0; k < trisInThisRD.Count; k++){
			newTriArray[++b] = trisInThisRD[k][0];
			newTriArray[++b] = trisInThisRD[k][1];
			newTriArray[++b] = trisInThisRD[k][2];
		}
		mesh.triangles = newTriArray;
	}

}
