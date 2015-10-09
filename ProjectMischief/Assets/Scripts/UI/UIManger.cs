using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManger : MonoBehaviour 
{
	//public string [] yearChoices= new string[3];
	//public string[] artistChoices= new string[3];
	//public string[] correctChoices = new string[3];
	//public string [] paintingchoices = new string[3];

	[HideInInspector]
	public Text currentPainting;
	[HideInInspector]
	public Text currentYear;
	[HideInInspector]
	public Text currentArtist;
	[HideInInspector]
	public ArtPiece artPiece;
    [HideInInspector]
    public Image art;
	static bool isRunning = false;

	public void CloseUI()
	{
		Time.timeScale = 1.0f;
		Destroy(this.gameObject);
		isRunning = false;
	}

	public void Verify()
	{
		artPiece.currentChoices [0] = currentPainting.text;
		artPiece.currentChoices [1] = currentYear.text;
		artPiece.currentChoices [2] = currentArtist.text;
	}

	void Awake () 
	{
		if(isRunning != true)
		{
			Time.timeScale = 0.0f;
			currentPainting = GameObject.Find ("PaintingChoice").GetComponent<Text> ();
			currentYear = GameObject.Find ("YearChoice").GetComponent<Text> ();
			currentArtist = GameObject.Find ("ArtistChoice").GetComponent<Text> ();
            art = GameObject.Find( "ArtPiece" ).GetComponent<Image>();
			isRunning = true;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
}
