using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.Text;

public class FractalGen : MonoBehaviour {
	//the smaller the chunk size is, the higher framerate is when building.
	//however, small chunks make the bug where the last chunk won't render
	//more pronounced.
	int chunkSize = 50;
	int maxChunks = 100;
	bool addingCollider = false;
	string constructionBit = "marker";
	HashSet<Vector3> dontDupe = new HashSet<Vector3>();
	float x = 0;
	float y = 0;
	float z = 0;
	Queue<string> toRenderChunkQueue = new Queue<string>();
	Queue<GameObject> beenRenderedChunkQueue = new Queue<GameObject>();
	bool permanent = true;
	Vector3 camGoTo;

	void Start () {
		StartCoroutine(generateLStringDeterministic(Grammars.levycurve, 30, "a"));
		StartCoroutine(chunkFactory());
	}

	void Update(){
		if(!permanent){
			if(beenRenderedChunkQueue.Count > maxChunks){
				Destroy(beenRenderedChunkQueue.Dequeue());
			}
		}

		Vector3 pos = camGoTo;
		pos.y = Camera.main.transform.position.y;
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, pos, Time.deltaTime);
	}
	
	/*
	 * Generate an L string using specified rules.
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
			while(part.Length > chunkSize){
				//push second half of string to stack with iteration level at front
				stringStack.Push(n.ToString() + ":" + part.Substring((int)Mathf.Floor(part.Length / 2)));
				
				//make part just the first half
				part = part.Substring(0, (int)Mathf.Floor(part.Length / 2) - 1);
			}

			//if on last iteration and nothing is on the stack, we're on last segment
			if(n == iterations && stringStack.Count == 0){
				//send chunk to the queue for production.
				//yield so we can take a moment to build the chunk
				toRenderChunkQueue.Enqueue(part);
				yield return null;
				//break;
			//if on last iteration but things are on the stack, we have more
			}else if(n == iterations && stringStack.Count != 0){
				//send chunk to the queue for production.
				//yield so we can take a moment to build the chunk
				toRenderChunkQueue.Enqueue(part);
				yield return null;

				//if next item on stack is on this level of iteration, it
				//must be same length or less, so we can chunk it without halving
				if(stringStack.Count != 0 && stringStack.Peek().Contains(iterations.ToString())){
					//pop next segment
					part = stringStack.Pop();

					//remove iteration level from string
					part = part.Substring(part.IndexOf(":") + 1);

					//send chunk to the queue for production.
					//yield so we can take a moment to build the chunk
					toRenderChunkQueue.Enqueue(part);
					yield return null;
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
	 * Waits for L string segments to arrive in the chunk queue
	 * and generates them.
	 */
	IEnumerator chunkFactory(){
		while(true){
			if(toRenderChunkQueue.Count != 0){
				makeChunk(toRenderChunkQueue.Dequeue());
			}
			yield return null;
		}
	}

	/*
	 * --> IEnumerator chunkFactory
	 * Make a chunk of size chunkSize
	 */
	void makeChunk(string LString){
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

			if(permanent){
				if(dontDupe.Add(new Vector3(x, y, z))){
					finals.Add(new Vector3(x, y, z));
				}
			}else{
				finals.Add(new Vector3(x, y, z));
			}
		}
		camGoTo = finals[0];

		List<Vector3> nextChunkVecs = new List<Vector3>();
		int q = 0;
		for(int i = 0; i < finals.Count; i++){
			nextChunkVecs.Add(finals[i]);
			q++;
			
			if(q == chunkSize){
				makeChunkParts(nextChunkVecs);
				nextChunkVecs.Clear();
				q = 0;
			}else if(i == finals.Count - 1){
				makeChunkParts(nextChunkVecs);
			}
		}
	}

	/*
	 * -->void makeChunk
	 * Make a single game object whose mesh is a combination of multiple
	 * smaller meshes
	 */
	void makeChunkParts(List<Vector3> chunkVecs){
		//instantiate a new chunk
		GameObject newChunk = GameObject.Instantiate(Resources.Load("Prefabs/Fractal/Chunk")) as GameObject;
		newChunk.transform.SetParent(GameObject.Find("Chunks").transform);

		//instantiate chunk parts and parent them to the new chunk
		foreach(Vector3 vec in chunkVecs){
			GameObject g =
				GameObject.Instantiate(Resources.Load("Prefabs/Fractal/" + constructionBit), vec, Quaternion.Euler(new Vector3(90f, 0f, 0f))) as GameObject;
			g.transform.SetParent(newChunk.gameObject.transform);
//			yield return null; //makes trippy effect
		}

		//now combine all their meshes
		combineMeshes(newChunk);
	}

	/*
	 * -->IEnumerator makeChunkParts
	 * Combines meshes of the children of passed in GameObject
	 */
	void combineMeshes(GameObject newChunk){
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
		if(!permanent)
			beenRenderedChunkQueue.Enqueue(newChunk);

//		yield return null;
	}
}
