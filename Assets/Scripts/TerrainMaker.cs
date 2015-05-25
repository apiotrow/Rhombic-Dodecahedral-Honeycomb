using UnityEngine;
using System.Collections;

public class TerrainMaker : MonoBehaviour {
	
	void Start () {
		for(int z = 0; z < 20; z++){
			if(z%2 == 0){
				for(int x = 0; x < 20; x += 2){
					instantiateRD(new Vector3(x, 0, z));
				}
			}else{
				for(int x = 1; x < 20; x += 2){
					instantiateRD(new Vector3(x, 0, z));
				}
			}
		}

	}

	void instantiateRD(Vector3 pos){
		GameObject.Instantiate(Resources.Load("Prefabs/rd"), pos, Quaternion.identity);
	}

}
