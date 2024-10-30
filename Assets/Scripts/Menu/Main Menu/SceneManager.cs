using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    [SerializeField] float minTimeToLoadScene = 1f;
    [SerializeField] float timeBeforeSceneChange = 1f;
    [FormerlySerializedAs("uI_LoadingScreen")] [SerializeField] UIScreenLoading uiScreenLoading = null;
    public GameObject imageHead;
    public Image blackImage;
    public bool showImage = true;

    public void LoadSceneAsync(string sceneName, string textInBetween = "", float textSize = 24f, bool useLoadBar = false, RoboFaces roboFace = RoboFaces.None)
    {
        StartCoroutine(AsynchronousLoadWithFake(sceneName, textInBetween, textSize, useLoadBar, roboFace));
    }

    IEnumerator AsynchronousLoadWithFake(string scene, string textInBetween, float textSize, bool useLoadBar, RoboFaces roboFace)
    {
        float loadingProgress;
        float timeLoading = 0;
        uiScreenLoading.UpdateLoadingBar(0);
        uiScreenLoading.SetTextSize(textSize);
        uiScreenLoading.FadeWithBlackScreen(textInBetween, useLoadBar, roboFace != RoboFaces.None);
        uiScreenLoading.SetNewRoboface(roboFace);
        uiScreenLoading.LockFade();
        var t = timeBeforeSceneChange;
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            yield return null;
        }

        AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            timeLoading += Time.unscaledDeltaTime;
            loadingProgress = ao.progress + 0.1f;
            loadingProgress = loadingProgress * timeLoading / minTimeToLoadScene;

            if (useLoadBar) uiScreenLoading.UpdateLoadingBar(loadingProgress);

            // Se completo la carga
            if (loadingProgress >= 1)
            {
                ao.allowSceneActivation = true;
                loadingProgress = 1;
            }


            yield return null;
        }

        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        uiScreenLoading.UnlockFade();
    }

    public void BlackScreenUnfade()
    {
        uiScreenLoading.BlackScreenUnfade();
    }

    public void FakeLoad(float time)
    {
        StartCoroutine(FakeLoadingWithBlackScreen(time));
    }

    public void FakeLoad(float time, string text)
    {
        StartCoroutine(FakeLoadingWithBlackScreen(time, text));
    }

    IEnumerator FakeLoadingWithBlackScreen(float time)
    {
        uiScreenLoading.FadeWithBlackScreen();
        uiScreenLoading.LockFade();
        yield return new WaitForSecondsRealtime(time);
        uiScreenLoading.UnlockFade();
    }

    IEnumerator FakeLoadingWithBlackScreen(float time, string text)
    {
        uiScreenLoading.FadeWithBlackScreen(text);
        uiScreenLoading.LockFade();
        yield return new WaitForSecondsRealtime(time);
        uiScreenLoading.UnlockFade();
    }
}