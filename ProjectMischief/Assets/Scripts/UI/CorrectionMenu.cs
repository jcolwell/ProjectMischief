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
	CorrectionUIControl uiControl;

	public Text [] fields = new Text[(int)CorrectionFieldTypes.eMAXTYPES];
	Text uiManCurChoice = null;


	void Awake () 
	{
		fields [(int)CorrectionFieldTypes.eCurrentField] = GameObject.Find ("CurrentField").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCurrentChoice] = GameObject.Find ("CurrentChoice").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice1] = GameObject.Find ("CorrectionInfo1").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice2] = GameObject.Find ("CorrectionInfo2").GetComponent<Text> ();
		fields [(int)CorrectionFieldTypes.eCorrectionChoice3] = GameObject.Find ("CorrectionInfo3").GetComponent<Text> ();

        GameObject temp = GameObject.Find("UIManger");
        uiControl = temp.GetComponent<CorrectionUIControl>();
	}

	public void CorrectPainting()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Painting";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentPainting.text;
		uiManCurChoice = uiControl.currentPainting;

        ArtContext artContext = ArtManager.instance.GetPainting( uiControl.artContextID );

		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
			fields[i].text =  artContext.paintingchoices[i];
		}
	}

	public void CorrectYear()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Year";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentYear.text;
		uiManCurChoice = uiControl.currentYear;

        ArtContext artContext = ArtManager.instance.GetPainting( uiControl.artContextID );

		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
            fields[i].text = artContext.yearChoices[i];
		}
	}

	public void CorrectArtist()
	{
		fields [(int)CorrectionFieldTypes.eCurrentField].text = "Artist";
		fields [(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentArtist.text;
		uiManCurChoice = uiControl.currentArtist;

        ArtContext artContext = ArtManager.instance.GetPainting( uiControl.artContextID );

		for(int i = 0 ; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
		{
            fields[i].text = artContext.artistChoices[i];
		}
	}

	public void SubmitChoice(int choice)
	{
		uiManCurChoice.text = fields [choice].text;
		uiManCurChoice = null;
	}
}
