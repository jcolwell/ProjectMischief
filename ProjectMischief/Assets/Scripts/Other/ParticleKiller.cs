using UnityEngine;
using System.Collections;

public class ParticleKiller : MonoBehaviour
{
    private ParticleSystem part;
    
    void Start()
    {
        part = this.GetComponent<ParticleSystem>();
    }
    void Update ()
    {
       if (!part.loop)
       {
            Destroy(gameObject, part.duration);
       }
    }
}
