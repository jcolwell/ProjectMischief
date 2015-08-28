using UnityEngine;
using System.Collections;

public class UIManger : MonoBehaviour 
{



	public void CloseUI()
	{
		Debug.Log ("Cosing UI");
		Destroy(this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
