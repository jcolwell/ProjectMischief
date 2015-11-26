using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
    Stats[] currentEquipment = new Stats[(int)EquipmentTypes.MAX];

    public void EquipEquipment(ref Stats equipment)
    {
        if(currentEquipment[(int)equipment.type] != null)
        {
            currentEquipment[(int)equipment.type].isEquipt = false;
        }
        equipment.isEquipt = true;
        currentEquipment[(int)equipment.type] = equipment;

        switch (equipment.type)
        {
            case EquipmentTypes.attire:
                break;

            case EquipmentTypes.footWear:
                gameObject.GetComponent<Moving>().SetSpeed((int)equipment.stat);
                break;

            case EquipmentTypes.headGear:
                gameObject.GetComponent<FogOfWar>().ChangeRadius((int)equipment.stat);
                break;
        }
    }
}
