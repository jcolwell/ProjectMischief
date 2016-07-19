using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorrectionUIControl : UIControl 
{
    // Public
    public GameObject correctionMenu;
    [HideInInspector]
    public uint artContextID;
    public Text currentPainting;
    public Text currentYear;
    public Text currentArtist;

    public Text paintingNumberText;
    public string paintingNumberExtraText = "Painting #";
    public Image art;

    public Color buttonFadeOutColor;
    public GameObject correctTitleButton;
    public GameObject correctYearButton;
    public GameObject correctArtistButton;
    public GameObject hintButton;
    public GameObject tutorialButton;

    public GameObject [] answersToslideIn;
    public GameObject[] evaluationIcons;
    //public float ;
    public float buttonSwitchTime = 1.5f;
    public GameObject[] fieldChoicebuttons;

    bool isSwitchingButtons;
    ArtFields currentField;
    int curButton = 0;
    Vector3 fieldChoiceStartingPos;
    Vector3 fieldStartingPos;
    float buttonSwitchingElapsedTime = 0.0f;
    float buttonSwitchSpeed = 0.0f;
    Vector3 fieldToFieldChoice;
    GameObject eventSystem;

    //public
    CorrectionUIControl()
        : base(UITypes.Correction)
    { }

    public void SetCurrentFields()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        art.sprite = curContext.art;
        currentPainting.text = curContext.currentChoices[0];
        currentYear.text = curContext.currentChoices[1];
        currentArtist.text = curContext.currentChoices[2];
        if(paintingNumberText != null)
        {
            paintingNumberText.text = paintingNumberExtraText + (artContextID + 1);
        }
    }

        // Functions for Button
    public void Verify()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        curContext.currentChoices[0] = currentPainting.text;
        curContext.currentChoices[1] = currentYear.text;
        curContext.currentChoices[2] = currentArtist.text;
        UIManager.instance.SetPaintingIteractedWith(true, artContextID);
        ArtManager.instance.SetGrade();
        UIManager.instance.UpdatePlayerGradeUI();
    }

    public void CheckHints()
    {
        PersistentSceneData data = PersistentSceneData.GetPersistentData();
        if (data.GetNumHints() == 0)
        {
            hintButton.SetActive(false);
        }
        else
        {
            hintButton.SetActive(true);
        }
    }

    public void StartSwitchingButtons(int _curButton, ArtFields _currentField)
    {
        if (currentField < ArtFields.eMax && curButton >= 0 && curButton < fieldChoicebuttons.Length)
        {
            isSwitchingButtons = true;
            buttonSwitchingElapsedTime = 0.0f;
            curButton = _curButton;
            currentField = _currentField;
            fieldChoiceStartingPos = fieldChoicebuttons[_curButton].transform.position;

            switch(_currentField)
            {
                case ArtFields.eArtist:
                    fieldStartingPos = correctArtistButton.transform.position;
                    break;

                case ArtFields.ePainting:
                    fieldStartingPos = correctTitleButton.transform.position;
                    break;

                case ArtFields.eYear:
                    fieldStartingPos = correctYearButton.transform.position;
                    break;
            }

            buttonSwitchSpeed = Vector3.Distance(fieldChoiceStartingPos, fieldStartingPos) / buttonSwitchTime;
            fieldToFieldChoice = fieldChoiceStartingPos - fieldStartingPos;
            fieldToFieldChoice.Normalize();

            if (eventSystem != null)
            {
                eventSystem.SetActive(false);
            }
        }

    }

    //Private
    void Awake()
    {
        correctionMenu.SetActive(false);
        CheckHints();

        PersistentSceneData data = PersistentSceneData.GetPersistentData();

        if (!data.CheckIfPlayerHasPlayedPainingTut())
        {
            tutorialButton.SetActive(true);
            data.SetHasPlayedPainingTut(true);
        }

        Image buttonImage = correctTitleButton.GetComponent<Image>();
        Button buttonScript = correctTitleButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableTitleCategory;
        if (ArtManager.instance.enableTitleCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }

        buttonImage = correctYearButton.GetComponent<Image>();
        buttonScript = correctYearButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableYearCategory;
        if (ArtManager.instance.enableYearCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }

        buttonImage = correctArtistButton.GetComponent<Image>();
        buttonScript = correctArtistButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableArtistCategory;
        if (ArtManager.instance.enableArtistCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }

        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem");  
        }

        SetCurrentFields();
    }

    void Update()
    {
        if(isSwitchingButtons)
        {
            buttonSwitchingElapsedTime += Time.unscaledDeltaTime;
            fieldChoicebuttons[curButton].transform.position += 
                ((-fieldToFieldChoice) * buttonSwitchSpeed * Time.unscaledDeltaTime);

            switch (currentField)
            {
                case ArtFields.eArtist:
                    correctArtistButton.transform.position += (fieldToFieldChoice * buttonSwitchSpeed * Time.unscaledDeltaTime);
                    break;

                case ArtFields.ePainting:
                    correctTitleButton.transform.position += (fieldToFieldChoice * buttonSwitchSpeed * Time.unscaledDeltaTime);
                    break;

                case ArtFields.eYear:
                    correctYearButton.transform.position += (fieldToFieldChoice * buttonSwitchSpeed * Time.unscaledDeltaTime);
                    break;
            }

            if (buttonSwitchingElapsedTime >= buttonSwitchTime)
            {
                isSwitchingButtons = false;
                CorrectionMenu corMenu = correctionMenu.GetComponent<CorrectionMenu>();
                corMenu.SubmitChoice(curButton);

                fieldChoicebuttons[curButton].transform.position = fieldChoiceStartingPos;
                switch (currentField)
                {
                    case ArtFields.eArtist:
                        correctArtistButton.transform.position = fieldStartingPos;
                        break;

                    case ArtFields.ePainting:
                        correctTitleButton.transform.position = fieldStartingPos;
                        break;

                    case ArtFields.eYear:
                        correctYearButton.transform.position = fieldStartingPos;
                        break;
                }

                corMenu.CloseMenu();

                if (eventSystem != null)
                {
                    eventSystem.SetActive(true);
                }
            }
        }
    }

    // protected
    protected override void DurringCloseUI()
    {
        GameObject player = GameObject.Find("Actor");
        if(player == null)
        {
            player = GameObject.Find("Actor(Clone)");
            if (player == null)
            {
                return;
            }
        }
        Moving playerMover = player.GetComponent<Moving>();
        playerMover.ResetClick();

    }
}
