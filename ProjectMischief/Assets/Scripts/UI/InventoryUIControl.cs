using UnityEngine;
using System.Collections;

public class InventoryUIControl : UIControl 
{
	public string playerName;

	PersistentSceneData sceneDataptr;
	Inventory playerInventory;

	void OnEnable()
	{
		sceneDataptr = PersistentSceneData.GetPersistentData();
		// TODO: find a better way to find the player
		playerInventory = GameObject.Find(playerName).GetComponent<Inventory>();
	}



    // private
    void Start()
    {
        UIManager.instance.RegisterUI( gameObject, UITypes.inventory );
    }

    void OnDestroy()
    {
        UIManager.instance.UnRegisterUI( UITypes.inventory );
    }
}
