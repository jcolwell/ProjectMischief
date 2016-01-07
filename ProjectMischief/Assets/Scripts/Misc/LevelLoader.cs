using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour 
{
    public GameObject loadingScreen;
    public float minTimeToLoad = 1.0f;

    float timeElpased = 0.0f;
    float deltaTime = 0.0f;
    float lastFramesTime = 0.0f;

    AsyncOperation async;

    GameObject curLoadingScreen = null;

    IEnumerator Load(string level)
    {
        async = Application.LoadLevelAsync(level);
        async.allowSceneActivation = false;
        yield return async;
    }
    
    IEnumerator Load(int levelId)
    {
        async = Application.LoadLevelAsync(levelId);
        async.allowSceneActivation = false;
        yield return async;
    }

    public void LoadLevel(string level)
    {
        lastFramesTime = Time.realtimeSinceStartup;
        timeElpased = 0.0f;
        curLoadingScreen = GameObject.Instantiate(loadingScreen);
        StartCoroutine(Load(level));
    }

    public void LoadLevel(int levelId)
    {
        lastFramesTime = Time.realtimeSinceStartup;
        timeElpased = 0.0f;
        curLoadingScreen = GameObject.Instantiate(loadingScreen);
        StartCoroutine(Load(levelId));
    }

    void Update()
    {
        CalculateDeltaTime();
        timeElpased+= deltaTime;

        if (timeElpased >= minTimeToLoad)
        {
            curLoadingScreen.GetComponent<LoadingScreen>().LevelLoaded();
            async.allowSceneActivation = true;
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
