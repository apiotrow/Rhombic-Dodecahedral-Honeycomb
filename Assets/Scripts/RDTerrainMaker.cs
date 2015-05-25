using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RDTerrainMaker : MonoBehaviour {
	void Start () {
		generateRDPlaneAtY(0, 10);
		generateRDPlaneAtY(1, 7);
		generateRDPlaneAtY(2, 3);
		generateRDPlaneAtY(3, 7);

		deleteHiddenFaces();
	}

	/*
	 * Removes meshes that aren't exposed to the air
	 */ 
	void deleteHiddenFaces(){

		//loop through all RDs in scene
		int childrenCount = this.gameObject.transform.childCount;
		for(int h = 0; h < childrenCount; h++) {

			//get base vertices for 1 triangle
			for(int p = 0; p < Globals.tris.Length; p += 3){
				Vector3 faceAvg = Globals.getTriangleAverage(this.gameObject.transform.GetChild(h).position, p);

				//check if triangle exists by trying to add it to testing hashset.
				//if it exists in the hashset, the face is pressed against another,
				//which means it's underground. in this case, add the vertices
				//to a dictionary for deletion.
				if(!Globals.hiddenTriangleTester.Add(faceAvg)){
					Globals.hiddenTriangles.Add(faceAvg, new int[]{Globals.tris[p], Globals.tris[p + 1], Globals.tris[p + 2]});
				}
			}

		}

		//render faces on all RDs
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
