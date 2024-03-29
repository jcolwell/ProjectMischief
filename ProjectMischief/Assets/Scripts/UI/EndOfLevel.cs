﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndOfLevel : MonoBehaviour 
{
    public static bool allPaintingsComplete = false;

    public string playerTag = "Player";
    public string nextLevel;

    public bool loadLevelSelectUI = false;
    public bool isArtGallery = false;
    public static bool isEndOfLevelVisible = false;

    public GameObject model;
    public GameObject mapIcon;

    public Camera cam;
    private Plane[] planes;
    public Collider objCollider;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            if(isArtGallery)
            {
                SceneManager.LoadScene(UIManager.GetUISceneName(UITypes.frontEnd), LoadSceneMode.Single);
                return;                
            }

            if (loadLevelSelectUI)
            {
                UIManager.instance.EndLevel(-1);
                return;
            }

            UIManager.instance.EndLevel(nextLevel);
        }
    }

    void Start()
    {
        if(isArtGallery)
        {
            allPaintingsComplete = true;
        }
        else
        {
            allPaintingsComplete = false;
        }

        model.SetActive(false);
        mapIcon.SetActive(false);

        cam = Camera.main;
        objCollider = model.GetComponent<Collider>();

        UIManager.instance.SetExitWorldPos(transform.position);
    }

    void Update()
    {
        if(allPaintingsComplete)
        {
            model.SetActive(true);
            mapIcon.SetActive(true);

            planes = GeometryUtility.CalculateFrustumPlanes(cam);
            isEndOfLevelVisible = GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
        }

    }
}
