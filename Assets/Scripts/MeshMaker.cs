using UnityEngine;
using System.Collections;

public class MeshMaker : MonoBehaviour
{
	Mesh mesh;

	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		makeCubeJack();

		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh;
	}

	void makeCubeJack()
	{
		mesh.Clear();       

		mesh.vertices = new Vector3[] {

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
			//top
			vec(0, 1, 0),

			//front
			vec(0, -1, -1),

			//bottom
			vec(0, -1, 0),

			//right
			vec(1, 0, 0),

			//left
			vec(-1, 0, 0),

			//back
			vec(0, 0, 1),

		};

		mesh.triangles = new int[] {
			//top
			0, 1, 2,
			2, 3, 0,

			//front
			4, 0, 3,
			3, 7, 4,

			//bottom
			5, 4, 7,
			7, 6, 5,

			//right
			7, 3, 2,
			2, 6, 7,

			//left
			5, 1, 0,
			0, 4, 5,

			//back
			6, 2, 1,
			1, 5, 6,
		};

	}

	Vector3 vec(float a, float b, float c){
		return (new Vector3(a, b, c));
	}


	void hexagon()
	{
		mesh.Clear();

		mesh.vertices = new Vector3[] {
			new Vector3(0, 0, 0),
			new Vector3(-1f, 0, 0),
			new Vector3(-0.5f, 0, 1), 
			new Vector3(0.5f, 0, 1),
			new Vector3(1, 0, 0),
			new Vector3(0.5f, 0, -1),
			new Vector3(-0.5f, 0, -1)
		};
		mesh.triangles = new int[] {
			0, 1, 2,
			0, 2, 3,
			0, 3, 4,
			0, 4, 5,
			0, 5, 6,
			0, 6, 1
		};


	}
}
