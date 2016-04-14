using UnityEngine;
using System.Collections;

public class ParticleEffectsManager : MonoBehaviour
{
    public enum Effect
    {
        SmokeBomb,
        Sparks,
        Jammer
    }

    public GameObject[] particleEffects;

    public void Instatiate(Effect effect, ref Vector3 position)
    {
        
    }
}
