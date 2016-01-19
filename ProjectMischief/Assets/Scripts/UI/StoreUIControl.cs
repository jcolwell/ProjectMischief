using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreUIControl : UIControl 
{
    // public
    public string playerName;

    public int smokeBombCost = 0;
    public int mirrorCost = 0;
    public int jammerCost = 0;
    public int hintCost = 0;

    public string nameOfCurrency = "Currency";

    // private
    PersistentSceneData sceneDataptr;
    Inventory playerInventory;

    int currentEquipment = 0;
    int playerCurrency = 0;
    const int numSlots = 6;
    public Stats[] equipmentInSlot = new Stats[numSlots];

    GameObject prevButton;
    GameObject nextButton;

    Text[] equipmentSlotsTexts;
    GameObject[] equipmentSlots;

    Text currencyText;

    //public
    public StoreUIControl()
        : base(UITypes.store, 3)
    { }

    
        // Functions for buttons

    public void UpdateUpgradeMenu()
    {
        for( int i = 0; i < numSlots; ++i )
        {
            if( currentEquipment + i < sceneDataptr.GetStoreEquipment().Count )
            {
                equipmentInSlot[i] = sceneDataptr.GetStoreEquipment()[currentEquipment + i];
                string type = "", statType = "";

                float stat = equipmentInSlot[i].stat; 

                switch (equipmentInSlot[i].type)
                {
                    case EquipmentTypes.attire:
						type = "Shirt";
						statType = "Amount of tools";
                        break;

                    case EquipmentTypes.footWear:
                        type = "Shoes";
                        statType = "Speed";
                        break;

                    case EquipmentTypes.headGear:
                        type = "Hat";
                        statType = "Vision";
                        stat *= 100.0f;
                        break;
                }

                equipmentSlotsTexts[i].text = equipmentInSlot[i].name + "\n" + type + "\n" + statType + " "
                    + stat.ToString() + "\nCost: " + equipmentInSlot[i].cost.ToString();
                equipmentSlots[i].SetActive( true );
            }
            else
            {
                equipmentSlots[i].SetActive( false );
            }
        }

        prevButton.SetActive( currentEquipment != 0 );
        nextButton.SetActive( currentEquipment + numSlots  < sceneDataptr.GetStoreEquipment().Count );
    }

    public void PrevEquipment()
    {
        currentEquipment -= numSlots;
        UpdateUpgradeMenu();
    }

    public void NextEquipment()
    {
        if( currentEquipment + numSlots < sceneDataptr.GetStoreEquipment().Count )
        {
            currentEquipment += numSlots;
        }

        UpdateUpgradeMenu();
    }

    public void CloseUpgradeMenu()
    {
        currentEquipment = 0;
    }

    public void BuyEquipment(int buttonId)
    {
        if(buttonId < 0 || buttonId >= numSlots)
        {
            return;
        }

        if (equipmentInSlot[buttonId] != null && equipmentInSlot[buttonId].cost <= playerCurrency)
        {
            playerCurrency -= equipmentInSlot[buttonId].cost;

            sceneDataptr.SetCurEquipment( ref equipmentInSlot[buttonId] );

            sceneDataptr.GetPlayerEquipment().Add( equipmentInSlot[buttonId] );
            sceneDataptr.GetStoreEquipment().Remove( equipmentInSlot[buttonId] );

            if( playerInventory != null )
            {
                playerInventory.EquipEquipment( ref equipmentInSlot[buttonId] );
            }

            equipmentInSlot[buttonId] = null;
            equipmentSlotsTexts[buttonId].text = "SOLD";

            UpdateCurrency();
        }
    }

    public void BuyTool(int tool)
    {
        if(tool < 0 || tool >= (int)ToolTypes.eToolMAX)
        {
            return;
        }

        // TODO: Make this code not shit
        int cost = 0;
        int numTool = sceneDataptr.GetNumTools( (ToolTypes)tool );

        switch ((ToolTypes)tool)
        {
            case ToolTypes.eJammer:
                cost = jammerCost;
                break;
                
            case ToolTypes.eMirror:
                cost = mirrorCost;
                break;

            case ToolTypes.eSmokeBomb:
                cost = smokeBombCost;
                break;
        }

        if(cost <= playerCurrency && numTool <= sceneDataptr.GetMaxToolnum())
        {
            playerCurrency -= cost;
            sceneDataptr.IncreaseNumTools((ToolTypes)tool);
            UIManager.instance.UpdateToolCount();
            UpdateCurrency();
            UpdateToolAndHintButtons();
        }
    }

    public void BuyHint()
    {
        if (hintCost <= playerCurrency)
        {
            playerCurrency -= hintCost;
            sceneDataptr.IncreaseHints();
            UpdateCurrency();
            UpdateToolAndHintButtons();
        }
    }

    // private
    void Awake()
    {
        sceneDataptr = PersistentSceneData.GetPersistentData();
        // TODO: find a better way to find the player
        GameObject temp = GameObject.Find( playerName );
        if(temp == null)
        {
            temp = GameObject.Find( playerName + "(Clone)" );
        }

        if( temp != null )
        {
            playerInventory = temp.GetComponent<Inventory>();
        }

        playerCurrency = sceneDataptr.GetPlayerCurrency();

        equipmentSlotsTexts = new Text[numSlots];
        equipmentSlots = new GameObject[numSlots];

    }

    void Start()
    {
        // grabing all revelnt objects
        for( int i = 0; i < numSlots; ++i )
        {
            equipmentSlotsTexts[i] = transform.FindDeepChild( "Upgrade" + (i + 1).ToString() + "Text" ).GetComponent<Text>();
            equipmentSlots[i] = transform.FindDeepChild( "Upgrade" + (i + 1).ToString() ).gameObject;
        }
        prevButton = transform.FindDeepChild( "PrevButton" ).gameObject;
        nextButton = transform.FindDeepChild( "NextButton" ).gameObject;

        currencyText = transform.FindDeepChild( "CurrencyText" ).GetComponent<Text>();

        GameObject upgradeMenu = GameObject.Find( "UpgradeMenu" );
        upgradeMenu.SetActive( false );

        UpdateCurrency();
        UpdateToolAndHintButtons();
        
    }

    void UpdateToolAndHintButtons()
    {
        Text tempText = transform.FindDeepChild("SmokeBombText").GetComponent<Text>();
        tempText.text = "SmokeBomb\nYou Have " + sceneDataptr.GetNumTools(ToolTypes.eSmokeBomb).ToString()
            + "\nCost " + smokeBombCost.ToString();

        tempText = transform.FindDeepChild("PocketMirrorText").GetComponent<Text>();
        tempText.text = "PocketMirror\nYou Have " + sceneDataptr.GetNumTools(ToolTypes.eMirror).ToString()
            + "\nCost " + mirrorCost.ToString();

        tempText = transform.FindDeepChild("CameraZapperText").GetComponent<Text>();
        tempText.text = "CameraZapper\nYou Have " + sceneDataptr.GetNumTools(ToolTypes.eJammer).ToString()
            + "\nCost " + jammerCost.ToString();

        tempText = transform.FindDeepChild("HintsText").GetComponent<Text>();
        tempText.text = "Hints\nYou Have " + sceneDataptr.GetNumHints().ToString() + "\nCost " + hintCost.ToString();
    }

    protected override void DurringDestroy()
    {
        sceneDataptr.SetPlayerCurrency( playerCurrency );
    }

    void UpdateCurrency()
    {
        currencyText.text = nameOfCurrency + "\n" + playerCurrency;
    }
}
