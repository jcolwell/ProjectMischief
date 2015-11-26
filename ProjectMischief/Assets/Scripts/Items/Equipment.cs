using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Equipment : MonoBehaviour 
{

   List<Stats> Equipt;

	void Start ()
    {
        Equipt = new List<Stats>();
        TextAsset text = Resources.Load<TextAsset>( "EquipmentSheet" );
        char[] delim = new char[] { ',' };
        char[] delim2 = new char[] { '\n' };
        string[] equipmentList = text.text.Split( delim2, System.StringSplitOptions.RemoveEmptyEntries );

        for( int i = 0; i < equipmentList.Length; ++i )
        {
            Equipt.Add(new Stats());
            string[] line = equipmentList[i].Split( delim );
            Equipt[i].name = line[0];
            //Equipt[i].type = line[1].ToLower();
            Equipt[i].stat = System.Convert.ToSingle( line[2]);
            Equipt[i].isEquipt = false;
            //print( "Name : " + Equipt[i].name + " Sight = " + Equipt[i].sight + " Speed = " + Equipt[i].speed );
        }
    }

    List<Stats> GetEquiptment()
    {
        return Equipt;
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