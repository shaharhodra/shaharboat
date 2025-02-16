using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public GameObject objSaveLoad;
	public SaveArray scp;
	public bool xx = false;
	public bool p = false;
	// Use this for initialization
	void Start () {
		objSaveLoad = GameObject.FindGameObjectWithTag("MainCamera");
		scp = objSaveLoad.GetComponent<SaveArray>();

	}
	
	// Update is called once per frame
	void Update () {

		if (p){
		if (xx){
			scp.picarray[0] = false;
		}
		p = false;
		}
	
	}
}
