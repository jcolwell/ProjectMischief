using UnityEngine;
using System.Collections;

public class ArtPiece : MonoBehaviour {

	public string [] paintingchoices = new string[3];
	public string [] yearChoices= new string[3];
	public string[] artistChoices= new string[3];
	public string[] correctChoices = new string[3];
	public string[] currentChoices = new string[3];

	void OnTriggerEnter(Collider other)
	{
		Application.LoadLevelAdditive ("UITest");

		UIManger uiManger = GameObject.Find ("UIManger").GetComponent<UIManger> ();

		uiManger.paintingchoices = paintingchoices;
		uiManger.yearChoices = yearChoices;
		uiManger.artistChoices = artistChoices;
		uiManger.correctChoices = correctChoices;
		uiManger.currentPainting.text = currentChoices[0];
		uiManger.currentYear.text = currentChoices[1];
		uiManger.currentArtist.text = currentChoices[2];
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
