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
	// counts how many times Update() has been called since LoadMenu() has been called
	int currentTick = 0;

	public void LoadMenu()
	{
		Application.LoadLevelAdditive ("UITest");
		openingMenu = true;
		currentTick = 0;
	}

	void Update()
	{
		//currentTick is checked to make sure that the uimanager has been loaded
		if( openingMenu == true && currentTick > 0)
		{
			openingMenu = false;
			GameObject uiMangerGameObject = GameObject.Find ("UIManger");
			UIManger uiManger = uiMangerGameObject.GetComponent<UIManger> ();
			uiManger.currentPainting.text = currentChoices[0];
			uiManger.currentYear.text = currentChoices[1];
			uiManger.currentArtist.text = currentChoices[2];
			uiManger.artPiece = gameObject.GetComponent<ArtPiece> ();
		}

		++currentTick;
	}
}
