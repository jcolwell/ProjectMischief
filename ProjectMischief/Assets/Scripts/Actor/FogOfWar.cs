using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    RaycastHit hit;
    public float radius = 1.0f;


    void Update ()
    {      
        Vector3 Orgin = transform.position;
        Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f) ;
  
        Ray ray = new Ray(Orgin, Up);
        
        if( Physics.Raycast( ray, out hit, 500 ))
        {
            MeshRenderer filter = hit.collider.gameObject.GetComponent<MeshRenderer>();
            Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3 relativePoint;
            Color Org = filter.material.color; 
    
            if( hit.collider.gameObject.transform.tag == "Fow50" )
            { 
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    HalfMesh( ref mesh, relativePoint, radius, ref filter );
            }
        }
    }

    void HalfMesh( ref Mesh mesh, Vector3 position, float inRadius, ref MeshRenderer filter )
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;
        float threquartRadsqr = (inRadius * 0.75f) * (inRadius * 0.75f);
        int vCount = mesh.vertexCount ;

        Color[] colours = new Color[vCount];

        for (int i = 0 ; i < vCount ; ++i)
        {
           //new_vcolor[i] = new Color (0.5f,0.5f,0.5f,1) ; 
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            //if the vertex is too far away, dont carry on
            //if( sqrMagnitude > sqrRadius )
            //{
            // alpha value will always be between 0 and 1
            colours[i].a = Mathf.Min(sqrMagnitude/sqrRadius, 1.0f);
            //}
        }


        mesh.colors = colours;
    }
}
