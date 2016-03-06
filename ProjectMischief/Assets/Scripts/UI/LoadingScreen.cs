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

    void Start()
    {
        DontDestroyOnLoad(this);
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.sortingOrder = 6;
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
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
