using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{ 
    public float radius = 0.01f;
    public string FOWTag = "Fow";

<<<<<<< HEAD
    RaycastHit hit;
    MeshRenderer filter;
    Mesh mesh;
    Ray ray;
    Color[] colours;
    Vector3[] vertices;
    Vector3 relativePoint;
    Vector3 Orgin;
    Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f) ;

    float sqrRadius;
    int vCount;
=======
    void Start()
    {
        
    }
>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone

    void Update ()
    {
        Orgin = transform.position;
        ray = new Ray(Orgin, Up);

        if( Physics.Raycast( ray, out hit, 500 ))
        {
            filter = hit.collider.gameObject.GetComponent<MeshRenderer>();
            mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;

            if( hit.collider.gameObject.transform.tag == FOWTag )
            { 
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    HalfMesh( ref mesh, relativePoint, radius, ref filter );
            }
        }
    }

    void HalfMesh( ref Mesh mesh, Vector3 position, float inRadius, ref MeshRenderer filter )
    {
<<<<<<< HEAD
        vertices = mesh.vertices;
        sqrRadius = inRadius * inRadius;
        vCount = mesh.vertexCount ;

        colours = mesh.colors;

=======
        Vector3[] vertices = mesh.vertices;
        float sqrRadius = inRadius * inRadius;
        int vCount = mesh.vertexCount ;

        Color[] colours = mesh.colors;

>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone
        if(colours.Length != vCount)
        {
            colours = new Color[vCount];
        }

        float div = 1 / sqrRadius;

        for (int i = 0 ; i < vCount ; ++i)
        {
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            colours[i].a = Mathf.Min(sqrMagnitude * div, 1.0f);
        }

        mesh.colors = colours;
    }


    //For Equipment Stats
    public void ChangeRadius(float r)
    {
        radius = r;
    }
}
