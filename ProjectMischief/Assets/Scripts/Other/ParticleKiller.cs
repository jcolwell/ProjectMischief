using UnityEngine;
using System.Collections;

public class ParticleKiller : MonoBehaviour
{
    ParticleSystem part;
    float lifetime = 0;
    
    void Update ()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(part);
        }
    }

    public void setParticle(ParticleSystem p)
    {
        lifetime = part.startLifetime;
        part = p;
    }
}
