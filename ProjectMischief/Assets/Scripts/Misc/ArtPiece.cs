using UnityEngine;
using System.Collections;

public class ArtPiece : MonoBehaviour {

	public string [] paintingchoices = new string[3];
	public string [] yearChoices= new string[3];
	public string[] artistChoices= new string[3];
	public string[] correctChoices = new string[3];
	public string[] currentChoices = new string[3];

	[HideInInspector]
	public bool playerIsInRange = false;

	bool openingMenu = false;
	public void LoadMenu()
	{
		Application.LoadLevelAdditive ("UITest");

		openingMenu = true;
	}

	void Update()
	{
		if( openingMenu == true)
		{
			openingMenu = false;
			UIManger uiManger = GameObject.Find ("UIManger").GetComponent<UIManger> ();
			uiManger.currentPainting.text = currentChoices[0];
			uiManger.currentYear.text = currentChoices[1];
			uiManger.currentArtist.text = currentChoices[2];
			uiManger.artPiece = gameObject.GetComponent<ArtPiece> ();
		}
	}

	// Use this for initialization
}
