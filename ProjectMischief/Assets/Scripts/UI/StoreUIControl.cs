using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreUIControl : UIControl 
{
    public string playerName;

    public int smokeBombCost;
    public int mirrorCost;
    public int jammerCost;

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

    public StoreUIControl()
        : base(UITypes.store)
    { }

    void Awake()
    {
        sceneDataptr = PersistentSceneData.GetPersistentData();
        // TODO: find a better way to find the player
        GameObject temp = GameObject.Find( playerName );
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

        for(int i = 0; i < numSlots; ++i)
        {
            equipmentSlotsTexts[i] = GameObject.Find( "Upgrade" + (i + 1).ToString() + "Text" ).GetComponent<Text>();
            equipmentSlots[i] = GameObject.Find( "Upgrade" + (i + 1).ToString() );
        }
        prevButton = GameObject.Find( "PrevButton" );
        nextButton = GameObject.Find( "NextButton" );

        currencyText = GameObject.Find("CurrencyText").GetComponent<Text>();

        GameObject upgradeMenu = GameObject.Find( "UpgradeMenu" );
        upgradeMenu.SetActive( false );

        currencyText.text = "Currency\n" + playerCurrency;

        Text tempText = GameObject.Find("SmokeBombText").GetComponent<Text>();
        tempText.text = "SmokeBomb\nCost " + smokeBombCost.ToString();

        tempText = GameObject.Find("PocketMirrorText").GetComponent<Text>();
        tempText.text = "PocketMirror\nCost " + mirrorCost.ToString();

        tempText = GameObject.Find("CameraZapperText").GetComponent<Text>();
        tempText.text = "CameraZapper\nCost " + jammerCost.ToString();
    }

    void OnDestroy()
    {
        sceneDataptr.SetPlayerCurrency(playerCurrency);
    }

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

            currencyText.text = "Currency\n" + playerCurrency;
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

        if(cost <= playerCurrency)
        {
            switch ((ToolTypes)tool)
            {
                case ToolTypes.eJammer:
                    sceneDataptr.IncreaseNumJammers();
                    break;

                case ToolTypes.eMirror:
                    sceneDataptr.IncreaseNumMirrors();
                    break;

                case ToolTypes.eSmokeBomb:
                    sceneDataptr.IncreaseNumSmokeBombs();
                    break;
            }
        }
    }
}
