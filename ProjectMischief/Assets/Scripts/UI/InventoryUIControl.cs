using UnityEngine;
using System.Collections;

public class InventoryUIControl : UIControl 
{


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
