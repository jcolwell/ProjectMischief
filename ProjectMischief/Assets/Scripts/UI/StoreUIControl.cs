using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreUIControl : UIControl 
{
    public string playerName;

    PersistentSceneData sceneDataptr;
    Inventory playerInventory;

    int currentEquipment = 0;
    const int numSlots = 6;
    public Stats[] equipmentInSlot = new Stats[numSlots];

    GameObject prevButton;
    GameObject nextButton;

    Text[] equipmentSlotsTexts;
    GameObject[] equipmentSlots;

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

        GameObject upgradeMenu = GameObject.Find( "UpgradeMenu" );
        upgradeMenu.SetActive( false );
    }

    // Functions for buttons

    public void UpdateUpgradeMenu()
    {
        for( int i = 0; i < numSlots; ++i )
        {
            if( currentEquipment + i < sceneDataptr.GetStoreEquipment().Count )
            {
                equipmentInSlot[i] = sceneDataptr.GetStoreEquipment()[currentEquipment + i];
                equipmentSlotsTexts[i].text = equipmentInSlot[i].name;
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

        if(equipmentInSlot[buttonId] != null)
        {
            sceneDataptr.SetCurEquipment( ref equipmentInSlot[buttonId] );

            sceneDataptr.GetPlayerEquipment().Add( equipmentInSlot[buttonId] );
            sceneDataptr.GetStoreEquipment().Remove( equipmentInSlot[buttonId] );

            if( playerInventory != null )
            {
                playerInventory.EquipEquipment( ref equipmentInSlot[buttonId] );
            }

            equipmentInSlot[buttonId] = null;
            equipmentSlotsTexts[buttonId].text = "SOLD";
        }
    }
}
