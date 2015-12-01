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
    public Color hintColor;
    public Color normalButtonColor;
    public Image[] fieldsImages = new Image[3];
    public Text[] fields = new Text[(int)CorrectionFieldTypes.eMAXTYPES];
    Text uiManCurChoice = null;

    ArtFields currentField = 0;

    void Awake()
    {
        fields[(int)CorrectionFieldTypes.eCurrentField] = GameObject.Find("CurrentField").GetComponent<Text>();
        fields[(int)CorrectionFieldTypes.eCurrentChoice] = GameObject.Find("CurrentChoice").GetComponent<Text>();
        fields[(int)CorrectionFieldTypes.eCorrectionChoice1] = GameObject.Find("CorrectionInfo1").GetComponent<Text>();
        fields[(int)CorrectionFieldTypes.eCorrectionChoice2] = GameObject.Find("CorrectionInfo2").GetComponent<Text>();
        fields[(int)CorrectionFieldTypes.eCorrectionChoice3] = GameObject.Find("CorrectionInfo3").GetComponent<Text>();

        fieldsImages[(int)CorrectionFieldTypes.eCorrectionChoice1] = GameObject.Find("CorrectionChoice1").GetComponent<Image>();
        fieldsImages[(int)CorrectionFieldTypes.eCorrectionChoice2] = GameObject.Find("CorrectionChoice2").GetComponent<Image>();
        fieldsImages[(int)CorrectionFieldTypes.eCorrectionChoice3] = GameObject.Find("CorrectionChoice3").GetComponent<Image>();

        GameObject temp = GameObject.Find("UIMangerCorrection");
        uiControl = temp.GetComponent<CorrectionUIControl>();
    }

    public void CorrectPainting()
    {
        fields[(int)CorrectionFieldTypes.eCurrentField].text = "Painting";
        fields[(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentPainting.text;
        uiManCurChoice = uiControl.currentPainting;

        ArtContext artContext = ArtManager.instance.GetPainting(uiControl.artContextID);

        for (int i = 0; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
        {
            fields[i].text = artContext.paintingchoices[i];
            fieldsImages[i].color = normalButtonColor;
        }

        currentField = ArtFields.ePainting;
    }

    public void CorrectYear()
    {
        fields[(int)CorrectionFieldTypes.eCurrentField].text = "Year";
        fields[(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentYear.text;
        uiManCurChoice = uiControl.currentYear;

        ArtContext artContext = ArtManager.instance.GetPainting(uiControl.artContextID);

        for (int i = 0; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
        {
            fields[i].text = artContext.yearChoices[i];
            fieldsImages[i].color = normalButtonColor;
        }

        currentField = ArtFields.eYear;
    }

    public void CorrectArtist()
    {
        fields[(int)CorrectionFieldTypes.eCurrentField].text = "Artist";
        fields[(int)CorrectionFieldTypes.eCurrentChoice].text = uiControl.currentArtist.text;
        uiManCurChoice = uiControl.currentArtist;

        ArtContext artContext = ArtManager.instance.GetPainting(uiControl.artContextID);

        for (int i = 0; i <= (int)CorrectionFieldTypes.eCorrectionChoice3; ++i)
        {
            fields[i].text = artContext.artistChoices[i];
            fieldsImages[i].color = normalButtonColor;
        }
        currentField = ArtFields.eArtist;
    }

    public void SubmitChoice(int choice)
    {
        uiManCurChoice.text = fields[choice].text;
        uiManCurChoice = null;
    }

    public void Hint()
    {
        if (PersistentSceneData.GetPersistentData().GetNumHints() > 0)
        {
            int correctAnswer = FindCorrectChoice();
            fieldsImages[correctAnswer].color = hintColor;
        }
    }

    int FindCorrectChoice()
    {
        ArtContext artContext = ArtManager.instance.GetPainting(uiControl.artContextID);

        for (int i = 0; i < 3; ++i)
        {
            if (artContext.correctChoices[(int)currentField] == fields[i].text)
            {
                return i;
            }
        }

        return -1;
    }
}
