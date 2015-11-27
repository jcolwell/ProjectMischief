using UnityEngine;
using System;
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

    void Awake()
    {
        List<Stats> temp = PersistentSceneData.GetPersistentData().GetPlayerEquipment();

        for(uint i = 0; i < temp.Count; ++i)
        {
            Stats tempStat = temp[(int)i];
            if( tempStat.isEquipt )
            {
                EquipEquipment(ref tempStat);
            }
        }
    }
}

public enum EquipmentTypes
{
	headGear,
	attire,
	footWear,
	MAX
};

[Serializable]
public class Stats
{
	public float stat;
	public EquipmentTypes type;
	public string name;
	public bool isEquipt;
}