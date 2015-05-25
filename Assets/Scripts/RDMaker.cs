using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RDMaker : MonoBehaviour {
	Mesh mesh;
	int[] tris;
	
	void Start () {

	}

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
		mesh.vertices = Coords.RDverts;
	}


	void setTriangles(Vector3 pos){
//		Coords.triList.Clear();

		List<int[]> lips = new List<int[]>();

//		for(int k = 0; k < Coords.tris.Length; k += 3){
//			Coords.triList.Add(new int[]{Coords.tris[k], Coords.tris[k + 1], Coords.tris[k + 2]});
//			print(Coords.tris[k] + ", " + Coords.tris[k + 1] + ", " + Coords.tris[k + 2]);
//		}

		int p = -1;
		while(p < Coords.tris.Length){
			Vector3[] tri = new Vector3[3]{
				Coords.RDverts[Coords.tris[++p]],
				Coords.RDverts[Coords.tris[++p]], 
				Coords.RDverts[Coords.tris[++p]]
			};

			//add positional offset to base vertices to get actual vertices
			for(int k = 0; k < tri.Length; k++){
				tri[k].x += pos.x;
				tri[k].y += pos.y;
				tri[k].z += pos.z;
			}

			Vector3 faceAvg = new Vector3(
				(tri[0].x + tri[1].x + tri[2].x) / 3f,
				(tri[0].y + tri[1].y + tri[2].y) / 3f,
				(tri[0].z + tri[1].z +  tri[2].z) / 3f
				);

//			if(Coords.hiddenTriangles.ContainsKey(faceAvg)){
//
//				int[] nn = new int[3]{Coords.hiddenTriangles[faceAvg][0], Coords.hiddenTriangles[faceAvg][1], Coords.hiddenTriangles[faceAvg][2]};
//				print(nn[0] + " " + nn[1] + " " + nn[2]);
//				if(Coords.triList.Contains(nn)){
//
//					Coords.triList.Remove(nn);
//				}
//
//			}

			if(!Coords.hiddenTriangles.ContainsKey(faceAvg)){
				lips.Add(new int[]{Coords.tris[p - 2], Coords.tris[p - 1], Coords.tris[p]});
				
			}


			if(p == Coords.tris.Length - 1) break;
		}



		int[] newTriArray = new int[(lips.Count * 3)];
		print(newTriArray.Length);
		int b = -1;
		for(int k = 0; k < lips.Count; k++){
			newTriArray[++b] = lips[k][0];
			newTriArray[++b] = lips[k][1];
			newTriArray[++b] = lips[k][2];
		}


		mesh.triangles = newTriArray;
	}

}
