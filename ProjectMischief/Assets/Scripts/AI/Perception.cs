//======================================================
// File: GuardAI.cs
// Description:    This Script will drive Guard AI
//======================================================

//======================================================
// Includes
//======================================================
using System.Collections;
using System.Collections.Generic;
//======================================================

//======================================================
// Memory Record
//======================================================
public class MemoryRecord
{
    //======================================================
    // Perception Status
    //======================================================
    public enum PerceptionStatus
    {
        Unknown = 0,
        Suspect,
        Confirm
    }
    //======================================================

    //==================================================
    // members
    //==================================================
    [UnityEngine.HideInInspector]
    public PerceptionStatus Status = PerceptionStatus.Unknown; 
    public UnityEngine.Vector3 LastSceneLocation = UnityEngine.Vector3.zero;    
    public float LastRecordedTime = float.MaxValue;
    public float Importnace = 0.0f;
 
    //==================================================

    //==================================================
    // Public Methods
    //==================================================
    public MemoryRecord( UnityEngine.Vector3 position, float timeStamp, float importance  )
    {
        LastSceneLocation = position;
        LastRecordedTime = timeStamp;
        Importnace = importance;
    }    
    //==================================================

}
//======================================================

//======================================================
// Perception Module
//======================================================
public class Perception
{
    //==================================================
    // members
    //==================================================
    public float mMemorySpan;
    public float ImportanceWeightConfirmed = 1000.0f;
    public float ImportanceWeightSuspected = 500.0f;
    public float ImportanceWeightTimeBase = 100.0f;
   
    [UnityEngine.HideInInspector]
    public List< MemoryRecord > Records;
    //==================================================

    //==================================================
    // Public Methods
    //==================================================
    public Perception( float memorySpan )
    {
        mMemorySpan = memorySpan;
    }

    //==================================================

    public void Update( float deltaTime )
    {
        AgeMemoryRecords( deltaTime );
        UpdateImportance();
    }

    //==================================================

    public void InsertRecord( MemoryRecord rec )
    {
        Records.Add( rec );
    }

    //==================================================
    // Private Methods
    //==================================================
    
    private void AgeMemoryRecords( float deltaTime )
    {
        foreach( var rec in Records )
        {
            rec.LastRecordedTime += deltaTime;
            
            if( rec.LastRecordedTime > mMemorySpan )
            {
                Records.Remove( rec );            
            }
        }
    }

    //==================================================

    private void UpdateImportance()
    {
        foreach( var rec in Records )
        {
            CalculateImportance( rec );
        }

        Records.Sort( delegate ( MemoryRecord a, MemoryRecord b ) 
        {
            if( a.Importnace > b.Importnace )
                return 1;
            else
                return 0;
        });
    }

    //==================================================

    private void CalculateImportance( MemoryRecord rec )
    {
        float importance = 0.0f;

        if( rec.Status == MemoryRecord.PerceptionStatus.Confirm)
        {
            importance += ImportanceWeightConfirmed;
        }
        else if ( rec.Status == MemoryRecord.PerceptionStatus.Suspect)
        {
            importance += ImportanceWeightSuspected;
        }

        float timePercent = ( 1.0f - System.Math.Min( rec.LastRecordedTime / mMemorySpan, 1.0f ) );
        importance += ImportanceWeightTimeBase * timePercent;

        rec.Importnace = importance;
    }
    
    //==================================================

}
//======================================================