using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class Equipment : MonoBehaviour 
{
    public class Stats
    {
        public string speed;
        public string sight;
        public string name;
        public bool isEquipt;
    }

   List<Stats> Equipt;

	void Start ()
    {
        Equipt = new List<Stats>();
        TextAsset text = Resources.Load<TextAsset>( "EquipmentSheet" );
        char[] delim = new char[] { ',' };
        char[] delim2 = new char[] { '\n' };
        string[] artList = text.text.Split( delim2, System.StringSplitOptions.RemoveEmptyEntries );

        for( int i = 0; i < artList.Length; ++i )
        {
            Equipt.Add(new Stats());
            string[] line = artList[i].Split( delim );
            Equipt[i].name = line[0];
            Equipt[i].speed = line[1];
            Equipt[i].sight = line[2];
            Equipt[i].isEquipt = false;
            print( "Name : " + Equipt[i].name + " Sight = " + Equipt[i].sight + " Speed = " + Equipt[i].speed );
        }
    }

    List<Stats> GetEquiptment()
    {
        return Equipt;
    }

}