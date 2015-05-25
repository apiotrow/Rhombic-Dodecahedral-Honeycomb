using UnityEngine;
using System.Collections;

public class RD : MonoBehaviour
{
	Mesh mesh;

	public void buildRD(Vector3 center)
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear(); 
		
		//build RD
		setVertices(center);
		setTriangles();
		setUVs();
		
		mesh.RecalculateNormals();
		
		//add collider
		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh;
	}
		
	void setUVs()
	{
		Vector2[] uvs = new Vector2[mesh.vertices.Length];
		for (int i=0; i < uvs.Length; i++)
		{
			uvs [i] = new Vector2(mesh.vertices [i].x, mesh.vertices [i].z);
		}
		
		mesh.uv = uvs;
	}
		
	void setVertices(Vector3 v)
	{
		Vector3[] verts = new Vector3[14];
		System.Array.Copy(Coords.RDverts, verts, 14);

		for(int i = 0; i < verts.Length; i++){
			verts[i].x += v.x;
			verts[i].y += v.y;
			verts[i].z += v.z;
		}

		mesh.vertices = verts;
	}
	
	void setTriangles()
	{
		mesh.triangles = new int[] {
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
	}


}
