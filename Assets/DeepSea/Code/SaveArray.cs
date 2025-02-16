using UnityEngine;
using System.Collections;

public class SaveArray : MonoBehaviour {

	public bool[] picarray;// = new bool[50];

	public bool SaveDD = false;
	public bool SaveBool = false;
	// Use this for initialization
	void Start () {
//		SaveData ();
//		LoadData ();
	}
	
	// Update is called once per frame
	void Update () {

		if (SaveBool){
			if(SaveDD){
				SaveData ();
//				Test = false;
			}
			else{
				LoadData ();
			}
			SaveBool = false;
		}
	}

	void Arraytest (){
		for (int i =0;i < picarray.Length;i++){
			Debug.Log(i);
		}
	}
	void SaveData () { // 데이타 세이브
		int arrayt = 0;
		string savet = "";
		for (int x = 0; x < picarray.Length;x++){
			if (picarray[x]){
				arrayt = 1;
			}
			else{
				arrayt = 0;
			}
			savet = x.ToString();
			PlayerPrefs.SetInt(savet, arrayt);
		}
		PlayerPrefs.SetInt("picarrayCount",picarray.Length);
		Debug.Log("Save Data");
	}

	void LoadData () { // 데이타 로드
		string loadt = "";
		picarray = new bool[PlayerPrefs.GetInt("picarrayCount")];
		for(int v = 0;v < PlayerPrefs.GetInt("picarrayCount");v++){
			loadt = v.ToString();
			if (PlayerPrefs.GetInt(loadt) == 1){
				picarray[v] = true;
			}
			else{
				picarray[v] = false;
			}
		}
		Debug.Log("Load Data");
	}
}
