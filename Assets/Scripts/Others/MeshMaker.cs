using UnityEngine;
using System.Collections;

public class MeshMaker : MonoBehaviour
{
	Mesh mesh;

	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		hexagon();

		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh;

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
