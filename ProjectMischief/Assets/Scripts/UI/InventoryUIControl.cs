using UnityEngine;
using System.Collections;

public class InventoryUIControl : UIControl 
{
	public string playerName;

	PersistentSceneData sceneDataptr;
	Inventory playerInventory;

    public InventoryUIControl()
        : base(UITypes.inventory)
    { }

    protected override void DurringOnEnable()
	{
		sceneDataptr = PersistentSceneData.GetPersistentData();
		// TODO: find a better way to find the player
		playerInventory = GameObject.Find(playerName).GetComponent<Inventory>();
	}
}
