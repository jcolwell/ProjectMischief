using UnityEngine;
using System.Collections;

public class LoadingScreen :MonoBehaviour
{
    public float lifeTimeAfterLevelLoaded = 0.5f;

    public bool levelLoaded = false;
    public float timeElpased = 0.0f;
    public float deltaTime = 0.0f;
    public float lastFramesTime = 0.0f;

    void Start()
    {
        DontDestroyOnLoad(this);

        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();

        SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();

        if (settingData.fixedAspectRatio)// place for settings check
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1.0f;
            canvas.sortingOrder = 6;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public void LevelLoaded()
    {
        levelLoaded = true;
        timeElpased = 0.0f;
        lastFramesTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        CalculateDeltaTime();
        timeElpased += deltaTime;

        if (levelLoaded && timeElpased >= lifeTimeAfterLevelLoaded)
        {
            Destroy(gameObject);
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
