//======================================================
// File:            VisionCone.cs
// Description:     This purpose of this script is to represent
//                  and act as the eyes of the agent
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//======================================================

//======================================================
// Class
//======================================================
public class VisionCone:MonoBehaviour
{
    //======================================================
    // Public
    //======================================================
    public float dist_min = 2.0f;
    public float dist_max = 5.0f;
    public float angleFOV = 30;
    public int quality = 15;
    public LayerMask cullingMask;
    public List<Material> materials;
    //======================================================

    //======================================================
    // private
    //======================================================    
    Material material;
    Mesh mesh;

    Status status;
    bool canSeePlayer;
    Vector3 playerPos;
    //======================================================

    public enum Status
    {
        Idle,
        Found
    }


    //======================================================

    void Start()
    {
        canSeePlayer = false;
        playerPos = Vector3.zero;

        //MESH THINGS
        mesh = new Mesh();
        // Could be of size [2 * quality + 2] if circle segment is continuous
        mesh.vertices = new Vector3[ 4 * quality ];
        mesh.triangles = new int[ 3 * 2 * quality ];

        Vector3[] normals = new Vector3[ 4 * quality ];
        Vector2[] uv = new Vector2[ 4 * quality ];

        for( int i = 0; i < uv.Length; i++ )
            uv[ i ] = new Vector2( 0, 0 );
        for( int i = 0; i < normals.Length; i++ )
            normals[ i ] = new Vector3( 0, 1, 0 );

        mesh.uv = uv;
        mesh.normals = normals;
    }

    //======================================================

    void Update()
    {
        canSeePlayer = false;
        playerPos = Vector3.zero;

        BuildMesh();
        ReportVision();
        UpdateMeshMaterial();
    }

    //======================================================

    private void BuildMesh()
    {
        float lookAtAngle = GetEnemyAngle();

        float angleStart = lookAtAngle - angleFOV;
        float angleEnd = lookAtAngle + angleFOV;
        float angleDelta = ( angleEnd - angleStart ) / quality;

        float currentAngle = angleStart;
        float nextAngle = angleStart + angleDelta;

        Vector3 currentPosMin = Vector3.zero;
        Vector3 currentPosMax = Vector3.zero;

        Vector3 nextPosMin = Vector3.zero;
        Vector3 nextPosMax = Vector3.zero;

        // Could be of size [2 * quality + 2] if circle segment is continuous
        Vector3[] vertices = new Vector3[ 4 * quality ];
        int[] triangles = new int[ 3 * 2 * quality ];

        for( int i = 0; i < quality; i++ )
        {
            Vector3 currentSphere = new Vector3(
                    Mathf.Sin( Mathf.Deg2Rad * ( currentAngle ) ), 0,
                    Mathf.Cos( Mathf.Deg2Rad * ( currentAngle ) ) );

            Vector3 nextSphere = new Vector3(
                    Mathf.Sin( Mathf.Deg2Rad * ( nextAngle ) ), 0,
                    Mathf.Cos( Mathf.Deg2Rad * ( nextAngle ) ) );

            currentPosMin = transform.position + currentSphere * dist_min;
            currentPosMax = transform.position + currentSphere * dist_max;

            nextPosMin = transform.position + nextSphere * dist_min;
            nextPosMax = transform.position + nextSphere * dist_max;

            currentPosMax = RaycastBetweenTwoPoints( ref currentPosMin, ref currentPosMax );
            nextPosMax = RaycastBetweenTwoPoints( ref nextPosMin, ref nextPosMax );

            int a = 4 * i;
            int b = 4 * i + 1;
            int c = 4 * i + 2;
            int d = 4 * i + 3;

            vertices[ a ] = currentPosMin;
            vertices[ b ] = currentPosMax;
            vertices[ c ] = nextPosMax;
            vertices[ d ] = nextPosMin;

            triangles[ 6 * i ] = a;       // Triangle1: ABC
            triangles[ 6 * i + 1 ] = b;
            triangles[ 6 * i + 2 ] = c;
            triangles[ 6 * i + 3 ] = c;   // Triangle2: CDA
            triangles[ 6 * i + 4 ] = d;
            triangles[ 6 * i + 5 ] = a;

            currentAngle += angleDelta;
            nextAngle += angleDelta;

        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Graphics.DrawMesh( mesh, Vector3.zero, Quaternion.identity, material, 0 );

    }

    //======================================================

    private Vector3 RaycastBetweenTwoPoints( ref Vector3 pos1, ref Vector3 pos2 )
    {
        RaycastHit hit = new RaycastHit();

        float dist = Vector3.Distance( pos1, pos2 );
        Vector3 v = pos2 - pos1;
        v.Normalize();

        if( Physics.Raycast( pos1, v, out hit, dist, cullingMask ) == true )
        {
            if( hit.collider.CompareTag( "Player" ) )
            {
                canSeePlayer = true;
                playerPos = hit.point;
            }

            return hit.point;
        }
        return pos2;
    }

    //======================================================

    float GetEnemyAngle()
    {
        // Left handed CW. z = angle 0, x = angle 90
        return 90 - Mathf.Rad2Deg * Mathf.Atan2( transform.forward.z, transform.forward.x );
    }

    //======================================================

    void UpdateMeshMaterial()
    {
        for( int i = 0; i < materials.Count; ++i )
        {
            if( i == ( int )status && material != materials[ i ] )
            {
                material = materials[ i ];
            }
        }
    }

    //======================================================

    private void ReportVision()
    {
        if( canSeePlayer )
        {
            status = Status.Found;
            SendMessageUpwards( "PlayerVisible", playerPos );
        }
        else
        {
            status = Status.Idle;
            SendMessageUpwards( "PlayerNotVisible" );
        }
    }

    //======================================================
}
//======================================================