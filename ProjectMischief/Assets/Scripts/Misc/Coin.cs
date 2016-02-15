using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour 
{
    public int currencyValue = 1;

    void OnTriggerEnter( Collider other )
    {
        if(other.CompareTag("Player"))
        {
            UIManager.instance.IncreaseCoinsEarned(currencyValue);
            Destroy( this.gameObject );
        }
    }
}
