using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManger : MonoBehaviour 
{
	public string [] paintingchoices = new string[3];
	public string [] yearChoices= new string[3];
	public string[] artistChoices= new string[3];
	public string[] correctChoices = new string[3];

	[HideInInspector]
	public Text currentPainting;
	[HideInInspector]
	public Text currentYear;
	[HideInInspector]
	public Text currentArtist;
	
	public void CloseUI()
	{
		Destroy(this.gameObject);
	}

	void Awake () 
	{
		currentPainting = GameObject.Find ("PaintingChoice").GetComponent<Text> ();
		currentYear = GameObject.Find ("YearChoice").GetComponent<Text> ();
		currentArtist = GameObject.Find ("ArtistChoice").GetComponent<Text> ();
	}
}
