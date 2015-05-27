using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.Text;

public class FractalGen : MonoBehaviour {
	int chunkSize = 2000;
	bool addingCollider = false;
	string constructionBit = "marker";
	HashSet<Vector3> dontDupe = new HashSet<Vector3>();
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
		StartCoroutine(generateLStringDeterministic(spiraly, 13, "ac"));
	}

	/*
	 * Generate an L string using specified rules
	 */
	IEnumerator generateLStringDeterministic(Dictionary<string, string> rules, int iterations, string initString){
		Stack<string> stringStack = new Stack<string>();

		int n = 0;
		string part = initString;
		while(true){
			//take us to n+1 by expanding the current part.
			//only do this if segment isn't on last iteration level.
			int s = 0;
			if(n != iterations){
				while(s != part.Length){
					string Lreplacement = rules[part[s].ToString()];
					
					part = part.Remove(s, 1);
					part = part.Insert(s, Lreplacement);
					s += Lreplacement.Length;
				}
				n++;
			}

			//continually halve the new string until its under our size limit.
			//push remainders to the stack.
			//length limit must be multiple of chunk size
			while(part.Length > chunkSize){
				//push second half of string to stack with iteration level at front
				stringStack.Push(n.ToString() + ":" + part.Substring((int)Mathf.Floor(part.Length / 2)));
				
				//make part just the first half
				part = part.Substring(0, (int)Mathf.Floor(part.Length / 2) - 1);
			}

			//if on last iteration and nothing is on the stack, we're on last segment
			if(n == iterations && stringStack.Count == 0){
				//MAKE CHUNK FROM PART
				StartCoroutine(makeChunk(part));
				break;

			//if on last iteration but things are on the stack, we have more
			}else if(n == iterations && stringStack.Count != 0){
				//MAKE CHUNK FROM PART
				StartCoroutine(makeChunk(part));

				//if next item on stack is on this level of iteration, it
				//must be same length or less, so we can chunk it without halving
				if(stringStack.Count != 0 && stringStack.Peek().Contains(iterations.ToString())){
					//pop next segment
					part = stringStack.Pop();

					//remove iteration level from string
					part = part.Substring(part.IndexOf(":") + 1);

					//MAKE CHUNK FROM PART
					StartCoroutine(makeChunk(part));
				}


				//if that was the last thing on the stack, we're done
				if(stringStack.Count == 0){
					break;

				//if not, get part ready for next iteration
				}else{
					//pop next segment
					part = stringStack.Pop();
					
					//reset iterator to match this segment's iteration level
					n = int.Parse(part.Substring(0, part.IndexOf(":")));
					
					//remove iteration level from string
					part = part.Substring(part.IndexOf(":") + 1);
				}
			}
		}

		yield return null;
	}

	/*
	 * Make a chunk of size chunkSize
	 */
	IEnumerator makeChunk(string LString){
//		print(LString);

		List<Vector3> finals = new List<Vector3>();
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

			if(dontDupe.Add(new Vector3(x, y, z))){
				finals.Add(new Vector3(x, y, z));
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
				GameObject.Instantiate(Resources.Load("Prefabs/Fractal/" + constructionBit), vec, Quaternion.Euler(new Vector3(90f, 0f, 0f))) as GameObject;
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
