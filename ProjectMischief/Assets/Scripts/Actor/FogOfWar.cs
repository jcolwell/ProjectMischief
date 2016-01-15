using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{ 
    public float radius = 0.01f;
    public string FOWTag = "Fow";

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        Vector3 Orgin = transform.position;
        Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);

        Ray ray = new Ray(Orgin, Up);
        RaycastHit hit;

        GameObject fow = null;
        if (Physics.Raycast(ray, out hit, 500))
        {

            if (hit.collider.gameObject.transform.tag == FOWTag)
            {
                fow = hit.collider.gameObject;
            }
            else
            {
                fow = UIManager.instance.fogOfWar;

            }
        }

        if (fow != null)
        {
            MeshRenderer filter = fow.GetComponent<MeshRenderer>();
            Mesh mesh = fow.GetComponent<MeshFilter>().mesh;


            Vector3 newPos = new Vector3(gameObject.transform.position.x, fow.transform.position.y,
                gameObject.transform.position.z);
            fow.transform.position = newPos;

            Vector3 relativePoint = filter.transform.InverseTransformPoint(hit.point);
            HalfMesh(ref mesh, relativePoint, radius, ref filter);
        }

    }

    void HalfMesh( ref Mesh mesh, Vector3 position, float inRadius, ref MeshRenderer filter )
    {
        Vector3[] vertices = mesh.vertices;
        float sqrRadius = inRadius * inRadius;
        int vCount = mesh.vertexCount ;

        Color[] colours = mesh.colors;

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
        Initialize();
    }
}
