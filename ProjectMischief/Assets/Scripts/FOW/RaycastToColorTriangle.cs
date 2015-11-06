using UnityEngine;
using System.Collections;

public class RaycastToColorTriangle : MonoBehaviour
{

    void Update()
    {
        if( Input.GetMouseButtonDown( 0 ) )
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            if( Physics.Raycast( ray, out hit ) )
            {
                VertixColorPlay vcp = hit.collider.gameObject.GetComponent<VertixColorPlay>();
                if( vcp != null )
                    vcp.ChangeColor( hit.triangleIndex );
            }
        }
    }
}