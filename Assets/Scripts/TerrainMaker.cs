using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainMaker : MonoBehaviour {
	void Start () {
		generateRDPlaneAtY(0, 5);
		generateRDPlaneAtY(1, 2);

		deleteHiddenFaces();
	}

	/*
	 * Removes meshes that aren't exposed to the air
	 */ 
	void deleteHiddenFaces(){

		//loop through all RDs in scene
		int childrenCount = this.gameObject.transform.childCount;
		for(int h = 0; h < childrenCount; h++) {
			int p = -1;

			//get base vertices for 1 triangle
			while(p < Coords.tris.Length){
				Vector3[] tri = new Vector3[3]{
					Coords.RDverts[Coords.tris[++p]],
					Coords.RDverts[Coords.tris[++p]], 
					Coords.RDverts[Coords.tris[++p]]
				};

				//add positional offset to base vertices to get actual vertices
				for(int k = 0; k < tri.Length; k++){
					tri[k].x += this.gameObject.transform.GetChild(h).position.x;
					tri[k].y += this.gameObject.transform.GetChild(h).position.y;
					tri[k].z += this.gameObject.transform.GetChild(h).position.z;
				}

				//average the 3 vertices to derive a positional signature for this triangle
				Vector3 faceAvg = new Vector3(
					(tri[0].x + tri[1].x + tri[2].x) / 3f,
					(tri[0].y + tri[1].y + tri[2].y) / 3f,
					(tri[0].z + tri[1].z +  tri[2].z) / 3f
					);

				//check if triangle exists by trying to add it to testing hashset.
				//if it exists in the hashset, the face is pressed against another,
				//which means it's underground. in this case, add the vertices
				//to a dictionary for deletion.
				if(!Coords.hiddenTriangleTester.Add(faceAvg)){
					Coords.hiddenTriangles.Add(faceAvg, new int[]{Coords.tris[p - 2], Coords.tris[p - 1], Coords.tris[p]});
//					print((p - 2) + ", " + (p - 1) + ", " + p);
//					print(Coords.tris[p - 2] + ", " + Coords.tris[p - 1] + ", " +  Coords.tris[p]);
				}


				//add to list of all triangles in scene
//				Coords.trianglesInTerrain.Add(tri);

				//break if we've completed last triangle of this RD
				if(p == Coords.tris.Length - 1) break;
			}
		}

		//loop through every triangle in scene
//		for(int d = 0; d < Coords.trianglesInTerrain.Count; d++){
//			Vector3 vec1 = Coords.trianglesInTerrain[d][0];
//			Vector3 vec2 = Coords.trianglesInTerrain[d][1];
//			Vector3 vec3 = Coords.trianglesInTerrain[d][2];
//
//			//average the 3 vertices to derive a positional signature for this triangle
//			Vector3 faceAvg = new Vector3(
//				(vec1.x + vec2.x + vec3.x) / 3f,
//				(vec1.y + vec2.y + vec3.y) / 3f,
//				(vec1.z + vec2.z + vec3.z) / 3f
//				);
//
			//check if triangle exists by trying to add it to testing hashset.
			//if it exists in the hashset, the face is pressed against another,
			//which means it's underground. in this case, add the vertices
			//to a list for deletion.
//			if(!Coords.hiddenTriangleTester.Add(faceAvg)){
//				Coords.hiddenTriangles.Add(Coords.trianglesInTerrain[d]);
//			}
//		}

		print(Coords.trianglesInTerrain.Count + ", " + Coords.hiddenTriangles.Count);

		childrenCount = this.gameObject.transform.childCount;
		for(int h = 0; h < childrenCount; h++) {
			this.gameObject.transform.GetChild(h).GetComponent<RDMaker>().buildIt(this.gameObject.transform.GetChild(h).position);
		}


	}
	
	void generateRDPlaneAtY(float y, float span){

		if(y%2 == 0){
			for(int z = 0; z < span; z++){
				if(z%2 == 0){
					for(int x = 0; x < span; x += 2){
						instantiateRD(new Vector3(x, y, z));
					}
				}else{
					for(int x = 1; x < span; x += 2){
						instantiateRD(new Vector3(x, y, z));
					}
				}
			}
		}else{
			for(int z = 0; z < span; z++){
				if(z%2 == 0){
					for(int x = 0; x < span; x += 2){
						instantiateRD(new Vector3(x + 1, y, z));
					}
				}else{
					for(int x = 1; x < span; x += 2){
						instantiateRD(new Vector3(x + 1, y, z));
					}
				}
			}
		}
	}

	void instantiateRD(Vector3 pos){
		GameObject newRD = GameObject.Instantiate(Resources.Load("Prefabs/rd"), pos, Quaternion.identity) as GameObject;
		newRD.transform.SetParent(this.gameObject.transform);
	}


}
