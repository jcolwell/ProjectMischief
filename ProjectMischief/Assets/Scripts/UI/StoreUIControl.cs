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

    public GameObject prevButton;
    public GameObject nextButton;
    public GameObject upgradeMenu;
    public GameObject[] equipmentSlots;
    public AudioClip song;

    public Text currencyText;

    // private
    PersistentSceneData sceneDataptr;
    Inventory playerInventory;

    Text[] equipmentSlotsTexts;
    Stats[] equipmentInSlot = new Stats[numSlots];

    BackgroundMusicManager manager;

    int currentEquipment = 0;
    int playerCurrency = 0;
    const int numSlots = 6;


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

                float stat = equipmentInSlot[i].stat + 1; 

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
                equipmentSlotsTexts[i].text = equipmentInSlot[i].name + "\n" + statType + " "
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
        upgradeMenu.SetActive( true );

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
    }

    void Start()
    {
        for( int i = 0; i < numSlots; ++i )
        {
            equipmentSlotsTexts[i] = equipmentSlots[i].GetComponentInChildren<Text>();
        }

        UpdateCurrency();
        UpdateToolAndHintButtons();
        upgradeMenu.SetActive( false );
    }

    void UpdateToolAndHintButtons()
    {
        Transform temp = transform.FindDeepChild("SBAmount");
        Text tempText = null;

        if( temp != null )
        {
            tempText = temp.gameObject.GetComponent<Text>();
            tempText.text = sceneDataptr.GetNumTools( ToolTypes.eSmokeBomb ).ToString();
        }

        temp = transform.FindDeepChild("PMAmount");
        if( temp != null )
        {
            tempText = temp.gameObject.GetComponent<Text>();
            tempText.text = sceneDataptr.GetNumTools(ToolTypes.eMirror).ToString();
        }

        temp = transform.FindDeepChild("CZAmount");
        if( temp != null )
        {
            tempText = temp.gameObject.GetComponent<Text>();
            tempText.text = sceneDataptr.GetNumTools(ToolTypes.eJammer).ToString();
        }

        temp = transform.FindDeepChild("HintsText");
        if( temp != null )
        {
            tempText = temp.gameObject.GetComponent<Text>();
            tempText.text = "Hints\nCost " + hintCost.ToString() + "\t\tYou Have " + sceneDataptr.GetNumHints().ToString();
        }
    }

    void UpdateCurrency()
    {
        currencyText.text = nameOfCurrency + "\n" + playerCurrency;
    }

    protected override void DurringDestroy()
    {
        sceneDataptr.SetPlayerCurrency( playerCurrency );
    }

    protected override void DurringOnEnable()
    {
        UIManager.instance.SetAllUIActive( false, UITypes.store );
        manager = UIManager.instance.GetMusicManger();
        manager.ChangeSong( song );
    }

    protected override void DurringCloseUI()
    {
        UIManager.instance.SetAllUIActive( true, UITypes.store );
        manager = UIManager.instance.GetMusicManger();
        manager.Pause();
    }
}
