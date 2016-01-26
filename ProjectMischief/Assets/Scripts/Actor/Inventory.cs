using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
    public void EquipEquipment(ref Stats equipment)
    {
        if(equipment.type == EquipmentTypes.MAX)
        {
            return;
        }

        switch (equipment.type)
        {
            case EquipmentTypes.attire:
                break;

            case EquipmentTypes.footWear:
                gameObject.GetComponent<Moving>().SetSpeed((int)equipment.stat);
                break;

            case EquipmentTypes.headGear:
                gameObject.GetComponent<FogOfWar>().ChangeRadius(equipment.stat);
                break;
        }
    }

    void Start()
    {
        PersistentSceneData sceneData = PersistentSceneData.GetPersistentData();

        for( int i = 0; i < (int)EquipmentTypes.MAX; ++i )
        {
            Stats curEquip = sceneData.GetCurEquipment( (EquipmentTypes)i );
            if(curEquip != null)
            {
                EquipEquipment( ref curEquip );
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
	public EquipmentTypes type = EquipmentTypes.MAX;
	public string name;
    public int cost;
}