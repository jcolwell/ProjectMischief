using UnityEngine;
using System.Collections;

public class LoadingScreen :MonoBehaviour
{
    public float lifeTimeAfterLevelLoaded = 0.5f;

    bool levelLoaded = false;
    float timeElpased = 0.0f;
    float timeWhenLoaded = 0.0f;
    float deltaTime = 0.0f;
    float lastFramesTime = 0.0f;

    public bool animated = false;
    public GameObject[] images;
    public float timeBetweenFrames;

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

        if(animated)
        {
            for(int i =1; i < images.Length; ++i)
            {
                images[i].SetActive(false);
            }
        }
    }

    public void LevelLoaded()
    {
        levelLoaded = true;
        timeWhenLoaded = timeElpased;
        lastFramesTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        CalculateDeltaTime();
        timeElpased += deltaTime;

        if (levelLoaded && timeElpased >= lifeTimeAfterLevelLoaded + timeWhenLoaded)
        {
            Destroy(gameObject);
        }

        if(animated)
        {
            int div = (int)(timeElpased/timeBetweenFrames);
            images[(div - 1) % images.Length].SetActive(false);
            images[(div) % images.Length].SetActive(true);
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
