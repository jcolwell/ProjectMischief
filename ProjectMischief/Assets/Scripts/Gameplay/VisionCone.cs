﻿//======================================================
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
    public uint ticksBetweenUpdate = 1;
    //======================================================

    //======================================================
    // private
    //======================================================    
    Material material;
    Mesh mesh;

    bool canSeePlayer;
    bool earlyOut;

    int[] triangles;

    float lookAtAngle;
    float angleStart;
    float angleEnd;
    float angleDelta;
    float currentAngle;
    float nextAngle;

    uint curTick = 0;

    Vector3 currentSphere;
    Vector3 viewPosOffSet;
    Vector3 distMaxVector;
    Vector3 playerPos;
    Vector3 currentPosMin;
    Vector3 currentPosMax;
    Vector3 viewPos;
    Vector3 nextPosMin = Vector3.zero;
    Vector3 nextPosMax = Vector3.zero;
    Vector3[] vertices;

    Vector3 objectPos;
    //======================================================

    public enum Status
    {
        Idle,
        Alert,
        Visible,
        FollowUp
    }
    Status status;


    //======================================================

    void Start()
    {
        canSeePlayer = false;
        earlyOut = false;
        status = Status.Idle;
        playerPos = Vector3.zero;
        distMaxVector = new Vector3( dist_max, 0.0f, dist_max );

        //MESH THINGS
        mesh = new Mesh();

        // Could be of size [2 * quality + 2] if circle segment is continuous
        mesh.vertices = new Vector3[ 4 * quality ];
        mesh.triangles = new int[ 3 * 2 * quality ];

        Vector3[] normals = new Vector3[ 4 * quality ];
        Vector2[] uv = new Vector2[ 4 * quality ];


        // Could be of size [2 * quality + 2] if circle segment is continuous
        vertices = new Vector3[2 * quality];
        triangles = new int[6 * quality];

        for( int i = 0; i < uv.Length; ++i )
        {
            uv[i] = new Vector2( 0, 0 );
        }

        for( int i = 0; i < normals.Length; ++i )
        {
            normals[i] = new Vector3( 0, 1, 0 );
        }

        mesh.uv = uv;
        mesh.normals = normals;
    }

    //======================================================

    void Update()
    {
        ++curTick;
        if ((ticksBetweenUpdate == 0 || curTick % ticksBetweenUpdate == 0) || 
            !PersistentSceneData.GetPersistentData().tuneViewConeUpdate)
        {
            canSeePlayer = false;
            playerPos = Vector3.zero;

            BuildMesh();
            ReportVision();
            UpdateMeshMaterial();
            mesh.RecalculateBounds();
        }

        if(!earlyOut)
        {
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }
    }

    //======================================================

    private void BuildMesh()
    {
        objectPos = transform.position;
        viewPos = Camera.main.WorldToViewportPoint( objectPos );

        if (viewPos.x < 0.0f || viewPos.x > 1.0f || viewPos.y < 0.0f || viewPos.y > 1.0f)
        {
            viewPosOffSet = Camera.main.WorldToViewportPoint( objectPos + distMaxVector ) - viewPos;
            if (viewPos.x + viewPosOffSet.x < 0.0f || viewPos.x - viewPosOffSet.x > 1.0f
                || viewPos.y + viewPosOffSet.y < 0.0f || viewPos.y - viewPosOffSet.y > 1.0f)
            {
                canSeePlayer = false;
                earlyOut = true;
                return; // the mesh is not visble on screen, no point in doing any calculations so kick out
            }
        }

        earlyOut = false;
        lookAtAngle = GetEnemyAngle();

        angleStart = lookAtAngle - angleFOV;
        angleEnd = lookAtAngle + angleFOV;
        angleDelta = ( angleEnd - angleStart ) / quality;

        currentAngle = angleStart;
        nextAngle = angleStart + angleDelta;

        currentSphere = new Vector3(
                    Mathf.Sin( Mathf.Deg2Rad * (currentAngle) ), 0,
                    Mathf.Cos( Mathf.Deg2Rad * (currentAngle) ) );

        currentPosMin = objectPos + currentSphere * dist_min;
        currentPosMax = objectPos + currentSphere * dist_max;
        currentPosMax = RaycastBetweenTwoPoints( ref currentPosMin, ref currentPosMax );

        //Debug.DrawLine( currentPosMin, currentPosMax, Color.black, 1 / 30, true );

        for( int i = 0; i < (quality - 1); ++i )
        {
            currentSphere = new Vector3(
                    Mathf.Sin( Mathf.Deg2Rad * ( nextAngle ) ), 0,
                    Mathf.Cos( Mathf.Deg2Rad * ( nextAngle ) ) );

            nextPosMin = objectPos + currentSphere * dist_min;
            nextPosMax = objectPos + currentSphere * dist_max;
            nextPosMax = RaycastBetweenTwoPoints( ref nextPosMin, ref nextPosMax );

            //Debug.DrawLine( nextPosMin, nextPosMax, Color.black, 1 / 30, true );

            int a = 2 * i;     
            int b = 2 * i + 1; 
            int c = 2 * i + 2; 
            int d = 2 * i + 3;

            vertices[ a ] = currentPosMin;
            vertices[ b ] = currentPosMax;
            vertices[ c ] = nextPosMin;
            vertices[ d ] = nextPosMax;

            triangles[ 6 * i ] = a;       // Triangle1: ABD
            triangles[ 6 * i + 1 ] = b;
            triangles[ 6 * i + 2 ] = d;
            triangles[ 6 * i + 3 ] = d;   // Triangle2: DCA
            triangles[ 6 * i + 4 ] = c;
            triangles[ 6 * i + 5 ] = a;

            currentAngle += angleDelta;
            nextAngle += angleDelta;


            currentPosMax = nextPosMax;
            currentPosMin = nextPosMin;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    //======================================================

    private Vector3 RaycastBetweenTwoPoints( ref Vector3 pos1, ref Vector3 pos2 )
    {
        RaycastHit hit = new RaycastHit();
        float dist = Vector3.Distance( objectPos, pos2 );
        Vector3 v = pos2 - objectPos;
        //Debug.DrawRay( objectPos, v, Color.white, 1 / 30, true );
        v.Normalize();
        if( Physics.Raycast( objectPos, v, out hit, dist, cullingMask ) )
        {
            float rayDist = Vector3.Distance( objectPos, hit.point );
            if( rayDist < dist_min )
            {
                if (hit.collider.CompareTag("Player"))
                {
                    dist = Vector3.Distance(pos1, pos2);
                    if (Physics.Raycast(pos1, v, out hit, dist, cullingMask))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            canSeePlayer = true;
                            playerPos = hit.point;
                            return pos1;
                        }
                        return hit.point;
                    }
                    return pos2;
                }
                return pos1;
            }
            if( hit.collider.CompareTag( "Player" ) )
            {
                canSeePlayer = true;
                playerPos = hit.point;
                //status = Status.Alert;
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
        SendMessageUpwards( "AskStatus" );
        for( int i = 0; i < materials.Count; ++i )
        {
            if(i == ( int )status)
            {
                material = materials[ i ];
                break;
            }
        }
    }

    //======================================================

    private void ReportVision()
    {
        if( canSeePlayer )
        {
            status = Status.Visible;
            SendMessageUpwards( "PlayerVisible", playerPos );
        }
        else
        {
            status = Status.Idle;
            SendMessageUpwards( "PlayerNotVisible" );
        }
    }

    //======================================================
    // Public Messaging functions

    public void FollowUpStatus()
    {
        status = Status.FollowUp;
    }

    public void IdleStatus()
    {
        status = Status.Idle;
    }

    public void AlertStatus()
    {
        status = Status.Alert;
    }

    public void VisibleStatus()
    {
        status = Status.Visible;
    }


}
//======================================================