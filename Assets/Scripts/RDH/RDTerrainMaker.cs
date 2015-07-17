using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Creates planes of Rhombic Dodecahedrons, and deletes hidden faces
 */
public class RDTerrainMaker : MonoBehaviour {
	bool combineMeshes = true;

	void Start () {

		generateRDPlaneAtY(-1, 50);

		for(int a = 0; a < 10; a++){
			generateRDPlaneAtY(a, 20 - (a * 2));
		}

		deleteHiddenFaces();

		//combine meshes from child objects
		if(combineMeshes){
			MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			int i = 0;
			while (i < meshFilters.Length){
				combine [i].mesh = meshFilters [i].sharedMesh;
				combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
				meshFilters [i].gameObject.active = false;
				i++;
			}
			transform.GetComponent<MeshFilter>().mesh = new Mesh();
			transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
			transform.gameObject.active = true;

			MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			meshc.sharedMesh = transform.GetComponent<MeshFilter>().mesh;

			int childrenCount = this.gameObject.transform.childCount;
			for(int h = 0; h < childrenCount; h++) {
				Destroy(transform.GetChild(h).gameObject);
			}
		}
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
				//which means it's underground. if it's underground, add the vertices
				//to a dictionary for deletion.
				if(!Globals.hiddenTriangleTester.Add(faceAvg)){
					Globals.hiddenTriangles.Add(faceAvg, new int[]{Globals.tris[p], Globals.tris[p + 1], Globals.tris[p + 2]});
				}
			}
		}

		//render faces on all RDs
		for(int h = 0; h < childrenCount; h++) {
			this.gameObject.transform.GetChild(h).GetComponent<RDMaker>().buildIt(this.gameObject.transform.GetChild(h).position, combineMeshes);
		}
	}

	/*
	 * Generates a plane of RDs at height y, with width and height span
	 */
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

	/*
	 * Instantiates a single RD and childs it to this game object
	 */
	void instantiateRD(Vector3 pos){
		string prefab;

		if(Random.Range(0f, 1f) < 0.5f){
			prefab = "rd2";
		}else{
			prefab = "rd";
		}
		GameObject newRD = GameObject.Instantiate(Resources.Load("Prefabs/" + prefab), pos, Quaternion.identity) as GameObject;
		newRD.transform.SetParent(this.gameObject.transform);
	}
}
