using UnityEngine;
using System.Collections;
using UnityEngine.UI;

enum CorrectionFieldTypes
{
	eCorrectionChoice1,
	eCorrectionChoice2,
	eCorrectionChoice3,
	eCurrentField,
	eCurrentChoice,
	eMAXTYPES
};

public class CorrectionMenu : MonoBehaviour 
{
	public UIManger uiManger;

	public Text [] fields = new Text[(int)CorrectionFieldTypes.eMAXTYPES];
	Text uiManCurChoice = null;

	// Use this for initialization
	void Awake () 
	{
		fields [(int)CorrectionFieldTypes.eCurrentField] = GameObject.Find ("CurrentField").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCurrentChoice] = GameObject.Find ("CurrentChoice").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice1] = GameObject.Find ("CorrectionInfo1").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice2] = GameObject.Find ("CorrectionInfo2").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice3] = GameObject.Find ("CorrectionInfo3").GetComponent<Text> ();
	}

	public void CorrectPainting()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Painting";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiManger.currentPainting.text;
		uiManCurChoice = uiManger.currentPainting;
		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
			fields[i].text = uiManger.artPiece.paintingchoices[i];
		}
	}

	public void CorrectYear()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Year";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiManger.currentYear.text;
		uiManCurChoice = uiManger.currentYear;
		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
			fields[i].text = uiManger.artPiece.yearChoices[i];
		}
	}

	public void CorrectArtist()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Artist";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiManger.currentArtist.text;
		uiManCurChoice = uiManger.currentArtist;
		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
			fields[i].text = uiManger.artPiece.artistChoices[i];
		}
	}

	public void SubmitChoice(int choice)
	{
		uiManCurChoice.text = fields [choice].text;
		uiManCurChoice = null;
	}
}
