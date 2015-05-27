using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.Text;

public class FractalGen : MonoBehaviour {
	int chunkSize = 500;
	bool addingCollider = false;
	string constructionBit = "Cube";
	string LString = "";
	HashSet<Vector3> dontDupe = new HashSet<Vector3>();
	List<Vector3> output = new List<Vector3>();
	float x = 0;
	float y = 0;
	float z = 0;
	
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

	////a: x+, b: z+, c: x-, d: z-, starting: ace
	Dictionary<string, string> loops = new Dictionary<string, string>(){
		{"a", "faf"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"},
		{"e", "ded"},
		{"f", "fe"}
	};

	Dictionary<string, string> rules = new Dictionary<string, string>(){
		{"a", "faf"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"},
		{"e", "ded"},
		{"f", "fe"}
	};
	

	void Start () {
//		string LString = generateLStringDeterministic(rules, 5, "ace");
		StartCoroutine(generateLStringDeterministic(levycurve, 13, "a"));

//		List<Vector3> output = new List<Vector3>();
//
//		float x = 0;
//		float y = 0;
//		float z = 0;
//
//		float increment = 1f;
//		foreach(char c in LString){
//			switch(c){
//				case 'a':
//					x += increment;
//					break;
//				case 'b':
//					y += increment;
//					break;
//				case 'c':
//					z += increment;
//					break;
//				case 'd':
//					x -= increment;
//					break;	
//				case 'e':
//					y -= increment;
//					break;
//				case 'f':
//					z -= increment;
//					break;
//			}
//			output.Add(new Vector3(x, y, z));
//		}
//		foreach(char c in LString){
//			switch(c){
//				case 'a':
//					x += increment;
////					y += increment;
//					break;
//				case 'b':
//					z += increment;
////					y -= increment;
//					break;
//				case 'c':
//					x -= increment;
////					y += increment;
//					break;
//				case 'd':
//					z -= increment;
////					y -= increment;
//					break;	
//			}
//			output.Add(new Vector3(x, y, z));
//		}
//
//		makeChunk(getNewListWithNoDuplicates(output));

//		GameObject.Find("FPSController").transform.position = output[0];
	}

	/*
	 * Generate an L string using specified rules
	 */
	IEnumerator generateLStringDeterministic(Dictionary<string, string> rules, int iterations, string initString){
		Stack<string> stringStack = new Stack<string>();

		int n = 0;
		string part = initString;
		bool done = false;
		while(!done){

			//take us to n+1
			int s = 0;
//			print("converting " + part);
			while(s != part.Length){
				string Lreplacement = rules[part[s].ToString()];
				
				part = part.Remove(s, 1);
				part = part.Insert(s, Lreplacement);
				s += Lreplacement.Length;
			}
			n++;
//			print("iteration " + n + " is " + part);



			//length limit must be multiple of chunk size
			while(part.Length > chunkSize){
				//push second half of string to stack with iteration level at front
//				print("pushing: " + (n.ToString() + ":" + part.Substring((int)Mathf.Floor(part.Length / 2)) + " to stack"));
				stringStack.Push(n.ToString() + ":" + part.Substring((int)Mathf.Floor(part.Length / 2)));
				
				//make part just the first half
				part = part.Substring(0, (int)Mathf.Floor(part.Length / 2) - 1);
			}

			if(n == iterations && stringStack.Count == 0){
				//MAKE CHUNK FROM PART
				if(part.Contains(":"))
				   part = part.Substring(part.IndexOf(":") + 1);
				StartCoroutine(makeChunk(part));
//				print("system was small or we're on last chunk. make chunk. done");
				done = true;
				break;
			}else if(n == iterations && stringStack.Count != 0){
				//MAKE CHUNK FROM PART
				if(part.Contains(":"))
					part = part.Substring(part.IndexOf(":") + 1);
				StartCoroutine(makeChunk(part));
//				print("segment is last iteration level. making chunk of: " + part);

				if(stringStack.Peek().Contains(iterations.ToString())){
					//pop next segment
					part = stringStack.Pop();
					//remove iteration level from string
					part = part.Substring(part.IndexOf(":") + 1); //? not start on right index?
					//MAKE CHUNK FROM PART
					StartCoroutine(makeChunk(part));
//					print("segment is last iteration level. making chunk of: " + part);

				}

				if(stringStack.Count == 0){
					done = true;
				}else{
					//pop next segment
					part = stringStack.Pop();
					
					//reset iterator to match this segment's iteration level
					n = int.Parse(part.Substring(0, part.IndexOf(":")));
//					print("popped " + part + ". stack count is " + stringStack.Count);
					
					//remove iteration level from string
					part = part.Substring(part.IndexOf(":") + 1); //? not start on right index?
				}

			}

		}

		yield return null;
	}

//	string generateLStringDeterministic(Dictionary<string, string> rules, int iterations, string initString){
//		for(int i = 0; i < iterations; i++){
//			int s = 0;
//			while(s != initString.Length){
//				string Lreplacement = rules[initString[s].ToString()];
//				
//				initString = initString.Remove(s, 1);
//				initString = initString.Insert(s, Lreplacement);
//				s += Lreplacement.Length;
//			}
//		}
//		
//		return initString;
//	}


	/*
	 * Given a list of Vector3s, return a new list of the same
	 * type but without duplicate entries
	 */
//	List<Vector3> getNewListWithNoDuplicates(List<Vector3> list){
//		List<Vector3> finals = new List<Vector3>();
//
//		foreach(Vector3 vec in list){
//			if(dontDupe.Add(vec)){
//				finals.Add(vec);
//			}
//		}
//
//		return finals;
//	}

	/*
	 * Make a chunk of size chunkSize
	 */
	IEnumerator makeChunk(string LString){
		print(LString);

		

		
		float increment = 1f;
		foreach(char c in LString){
			switch(c){
				case 'a':
					x += increment;
					break;
				case 'b':
					z += increment;
					break;
				case 'c':
					x -= increment;
					break;
				case 'd':
					z -= increment;
					break;	
				case 'e':
					z -= increment;
					break;
				case 'f':
					z -= increment;
					break;
			}
			output.Add(new Vector3(x, y, z));
		}


		List<Vector3> finals = new List<Vector3>();
		
		foreach(Vector3 vec in output){
			if(dontDupe.Add(vec)){
				finals.Add(vec);
			}
		}



		List<Vector3> nextChunkVecs = new List<Vector3>();
		int q = 0;
		for(int i = 0; i < finals.Count; i++){
			nextChunkVecs.Add(finals[i]);
			q++;
			
			if(q == chunkSize){
				StartCoroutine(makeChunkParts(nextChunkVecs));
				nextChunkVecs.Clear();
				q = 0;
			}else if(i == finals.Count - 1){
				StartCoroutine(makeChunkParts(nextChunkVecs));
			}
		}

		yield return null;
	}

	/*
	 * Make a chunk of size chunkSize
	 */
//	void makeChunk(List<Vector3> f){
//		List<Vector3> nextChunkVecs = new List<Vector3>();
//		int q = 0;
//		for(int i = 0; i < f.Count; i++){
//			nextChunkVecs.Add(f[i]);
//			q++;
//			
//			if(q == chunkSize){
//				StartCoroutine(makeChunkParts(nextChunkVecs));
//				nextChunkVecs.Clear();
//				q = 0;
//			}else if(i == f.Count - 1){
//				StartCoroutine(makeChunkParts(nextChunkVecs));
//			}
//		}
//	}

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
}
