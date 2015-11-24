using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum EquipmentTypes
{
    headGear,
    attire,
    footWear,
    MAX

};

public class Inventory : MonoBehaviour 
{

    Equipment.Stats[] currentlyEquiped = new Equipment.Stats[(int)EquipmentTypes.MAX];
    List<Equipment.Stats> equipment;

    public void EquipEquipment(Equipment.Stats equipment)
    {

    }


}
