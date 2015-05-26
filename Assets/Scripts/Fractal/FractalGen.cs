using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.Text;

public class FractalGen : MonoBehaviour {
	int chunkSize = 300;
	bool addingCollider = false;
	string constructionBit = "Cube";

	//a: x+, b: z+, c: x-, d: z-, starting: a
	Dictionary<string, string> levycurve = new Dictionary<string, string>(){
		{"a", "ab"},
		{"b", "bc"},
		{"c", "cd"},
		{"d", "da"}
	};

	//a: x+, b: z+, c: x-, d: z-, starting: a
	Dictionary<string, string> juliaSetish = new Dictionary<string, string>(){
		{"a", "bab"},
		{"b", "cbc"},
		{"c", "dcd"},
		{"d", "ada"}
	};

	//a: x+, b: z+, c: x-, d: z-, starting: ac
	Dictionary<string, string> spiraly = new Dictionary<string, string>(){
		{"a", "dad"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"}
	};

	//a: x+, b: z+, c: x-, d: z-, starting: ac
	Dictionary<string, string> rhombus = new Dictionary<string, string>(){
		{"a", "bcb"},
		{"b", "ba"},
		{"c", "dad"},
		{"d", "dc"}
	};

	Dictionary<string, string> rules = new Dictionary<string, string>(){
		{"a", "bcb"},
		{"b", "ba"},
		{"c", "dad"},
		{"d", "dc"}
	};
	

	void Start () {
		string LString = generateLStringDeterministic(rhombus, 7, "ca");
		List<Vector3> output = new List<Vector3>();

		float x = 0;
		float y = 0;
		float z = 0;

		float increment = 1f;
		foreach(char c in LString){
			switch(c){
				case 'a':
					x += increment;
//					y += increment;
					break;
				case 'b':
					z += increment;
//					y += increment;
					break;
				case 'c':
					x -= increment;
//					y -= increment;
					break;
				case 'd':
					z -= increment;
//					y -= increment;
					break;	
			}
				
			output.Add(new Vector3(x, y, z));
		}

		makeChunk(getNewListWithNoDuplicates(output));

//		GameObject.Find("FPSController").transform.position = output[0];
	}

	/*
	 * Generate an L string using specified rules
	 */
	string generateLStringDeterministic(Dictionary<string, string> rules, int iterations, string initString){
		for(int i = 0; i < iterations; i++){
			int s = 0;
			while(s != initString.Length){
				string Lreplacement = rules[initString[s].ToString()];

				initString = initString.Remove(s, 1);
				initString = initString.Insert(s, Lreplacement);
				s += Lreplacement.Length;
			}
		}
	
		return initString;
	}

	/*
	 * Given a list of Vector3s, return a new list of the same
	 * type but without duplicate entries
	 */
	List<Vector3> getNewListWithNoDuplicates(List<Vector3> list){
		List<Vector3> finals = new List<Vector3>();
		HashSet<Vector3> nonDups = new HashSet<Vector3>();

		foreach(Vector3 vec in list){
			if(nonDups.Add(vec)){
				finals.Add(vec);
			}
		}

		return finals;
	}

	/*
	 * Make a chunk of size chunkSize
	 */
	void makeChunk(List<Vector3> f){
		List<Vector3> nextChunkVecs = new List<Vector3>();
		int q = 0;
		for(int i = 0; i < f.Count; i++){
			nextChunkVecs.Add(f[i]);
			q++;
			
			if(q == chunkSize){
				StartCoroutine(makeChunkParts(nextChunkVecs));
				nextChunkVecs.Clear();
				q = 0;
			}else if(i == f.Count - 1){
				StartCoroutine(makeChunkParts(nextChunkVecs));
			}
		}
	}

	/*
	 * -->void makeChunk
	 * Make a single game object whose mesh is a combination of multiple
	 * smaller meshes
	 */
	IEnumerator makeChunkParts(List<Vector3> chunkVecs){
		//instantiate a new chunk
		GameObject newChunk = GameObject.Instantiate(Resources.Load("Prefabs/Fractal/Chunk")) as GameObject;
		newChunk.transform.SetParent(GameObject.Find("Chunks").transform);

		//instantiate chunk parts and parent them to the new chunk
		foreach(Vector3 vec in chunkVecs){
			GameObject g =
				GameObject.Instantiate(Resources.Load("Prefabs/Fractal/" + constructionBit), vec, Quaternion.identity) as GameObject;
			g.transform.SetParent(newChunk.gameObject.transform);
		}

		//now combine all their meshes
		yield return StartCoroutine(combineMeshes(newChunk));
	}

	/*
	 * -->IEnumerator makeChunkParts
	 * Combines meshes of the children of passed in GameObject
	 */
	IEnumerator combineMeshes(GameObject newChunk){
		//combine the meshes of the children of the chunk
		MeshFilter[] meshFilters = newChunk.transform.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length){
			combine [i].mesh = meshFilters [i].sharedMesh;
			combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
			meshFilters [i].gameObject.active = false;
			i++;
		}
		newChunk.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		newChunk.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		newChunk.transform.gameObject.active = true;

		//add a collider to the combined mesh
		if(addingCollider){
			MeshCollider meshc = newChunk.AddComponent(typeof(MeshCollider)) as MeshCollider;
			meshc.sharedMesh = newChunk.transform.GetComponent<MeshFilter>().mesh;
		}

		//delete the children of the chunk, as they aren't needed anymore
		int chunkChildCount = newChunk.transform.childCount;
		for(int h = 0; h < chunkChildCount; h++) {
			Destroy(newChunk.transform.GetChild(h).gameObject);
		}
		yield return null;
	}


	void fractalFunc(){
		float x;
		float z;
		
		x = 2f;
		z = 2f;
		
		for(int i = 0; i < 20; i++){
			x = Mathf.Cos(x) * x * 4;
			z = Mathf.Cos(z) * z * 4;
//			output.Add(new Vector3(x, 0f, z));
		}
	}
}
