using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void LoadLevel(string level)
    {
        lastFramesTime = Time.realtimeSinceStartup;
        timeElpased = 0.0f;
        curLoadingScreen = GameObject.Instantiate(loadingScreen);

        SceneManager.LoadScene(level, LoadSceneMode.Single);

        curLoadingScreen.GetComponent<LoadingScreen>().LevelLoaded();
    }

    public void LoadLevel(int levelId)
    {
        lastFramesTime = Time.realtimeSinceStartup;
        timeElpased = 0.0f;
        curLoadingScreen = GameObject.Instantiate(loadingScreen);

        SceneManager.LoadScene(levelId, LoadSceneMode.Single);

        curLoadingScreen.GetComponent<LoadingScreen>().LevelLoaded();
    }

    void Update()
    {
        CalculateDeltaTime();
        timeElpased+= deltaTime;
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
