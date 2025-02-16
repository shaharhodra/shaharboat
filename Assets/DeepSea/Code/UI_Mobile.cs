using UnityEngine;
using System.Collections;

public class UI_Mobile : MonoBehaviour {
	private float roty;
	private bool rotbool = true;
	private float redcolor;
	private float greencolor;
	private float blurcolor;
	private float bulgeScale = 35f;

	private GameObject go;
	private Color testcolor;
	private Color precolor = Color.black;
	private float prefloat00 = 0f;

	void OnGUI() {
		if (GUI.Button(new Rect(245, 23, 100, 20), "Auto Rotation")){
			if(rotbool == true){
				rotbool = false;
			}
			else{
				rotbool = true;
			}
		}

		if (GUI.Button(new Rect(350, 23, 150, 20),"Mobile Shader")){
			Application.LoadLevel (0);
		}

		GUI.Label(new Rect(25, 8, 100, 20), "Rotation");
		roty = GUI.HorizontalScrollbar(new Rect(25, 25, 200, 30), roty, 1.0F, 0.0F, 360.0F);

		GUI.Label(new Rect(25, 60, 100, 20), "Red");
		redcolor = GUI.HorizontalScrollbar(new Rect(25, 80, 200, 30), redcolor, 0.01F, 0.0F, 1.0F);
		GUI.Label(new Rect(25, 90, 100, 20), "Green");
		greencolor = GUI.HorizontalScrollbar(new Rect(25, 110, 200, 30), greencolor, 0.01F, 0.0F, 1.0F);
		GUI.Label(new Rect(25, 120, 100, 20), "Blue");
		blurcolor = GUI.HorizontalScrollbar(new Rect(25, 140, 200, 30), blurcolor, 0.01F, 0.0F, 1.0F);

		GUI.Label(new Rect(25, 160, 100, 20), "Bulge Scale");
		bulgeScale = GUI.HorizontalScrollbar(new Rect(25, 180, 200, 30), bulgeScale, 0.5F, 0.0F, 35.0F);

	}
	
	void Start () {
		go = GameObject.Find ("Sea_Mobile");
		redcolor = go.transform.GetComponent<MeshRenderer> ().material.GetColor ("_GlowColor").r;
		greencolor = go.transform.GetComponent<MeshRenderer> ().material.GetColor ("_GlowColor").g;
		blurcolor = go.transform.GetComponent<MeshRenderer> ().material.GetColor ("_GlowColor").b;
		go.transform.GetComponent<MeshRenderer> ().material.shader = Shader.Find("Mobile/Examples/DeepSea_Opaqe");
	}

	void Update () {
		transform.localEulerAngles = new Vector3 (0, roty, 0);
		testcolor = new Color (redcolor,greencolor,blurcolor);
	
		if (rotbool) {
			roty = transform.localEulerAngles.y + 0.1f;
			transform.localEulerAngles = new Vector3 (0, roty, 0);
				if (roty > 359f) {
					roty = 0f;
				}
			}
		if (precolor != testcolor || prefloat00 != bulgeScale){
			go.transform.GetComponent<MeshRenderer> ().material.SetColor ("_GlowColor", new Color (redcolor,greencolor,blurcolor));
			go.transform.GetComponent<MeshRenderer> ().material.SetFloat ("_BulgeScale", bulgeScale);

			precolor = testcolor;
			prefloat00 = bulgeScale;
		}
	}
}
