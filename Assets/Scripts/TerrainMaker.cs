using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainMaker : MonoBehaviour {
	void Start () {
		generateRDPlaneAtY(0);
		generateRDPlaneAtY(1);

		int childrenCount = this.gameObject.transform.childCount;
		for(int h = 0; h < childrenCount; h++) {
			int p = -1;
			while(p < Coords.tris.Length){
				Vector3[] tri = new Vector3[3]{
					Coords.RDverts[Coords.tris[++p]],
					Coords.RDverts[Coords.tris[++p]], 
					Coords.RDverts[Coords.tris[++p]]
				};
				
				for(int k = 0; k < tri.Length; k++){
					tri[k].x += this.gameObject.transform.GetChild(h).position.x;
					tri[k].y += this.gameObject.transform.GetChild(h).position.y;
					tri[k].z += this.gameObject.transform.GetChild(h).position.z;
				}

				Coords.facesInTerrain.Add(tri);

				print(tri[0] + ", " + tri[1] + ", " + tri[2]);


				if(p == Coords.tris.Length - 1) break;
			}
		}

		for(int d = 0; d < Coords.facesInTerrain.Count; d++){
			Vector3 vec1 = Coords.facesInTerrain[d][0];
			Vector3 vec2 = Coords.facesInTerrain[d][1];
			Vector3 vec3 = Coords.facesInTerrain[d][2];

			Vector3 faceAvg = new Vector3(
				(vec1.x + vec2.x + vec3.x) / 3f,
				(vec1.y + vec2.y + vec3.y) / 3f,
				(vec1.z + vec2.z + vec3.z) / 3f
				);
			
			if(!Coords.hidden.Add(faceAvg)){
				Coords.hiddenFaces.Add(Coords.facesInTerrain[d]);
			}
		}
		
		deleteHiddenFaces();


	}

	void deleteHiddenFaces(){
		var distinct = new HashSet<Vector3[]>();    
//		var duplicates = new HashSet<string>();

//		foreach (var s in Coords.vertsInTerrain){
//			if (!distinct.Add(s)){
//				Coords.hiddenFaces.Add(s);
//			}
//		}

		
		print(Coords.facesInTerrain.Count + ", " + Coords.hiddenFaces.Count);
	}
	
	void generateRDPlaneAtY(float y){

		int span = 3;

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
