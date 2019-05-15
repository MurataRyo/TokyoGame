using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTask : MonoBehaviour
{
    public static string sceneName;
    AsyncOperation loadScene;      //ロード先
    const float LoadTimeMin = 1f;  //最低のロード時間
    public enum GameMode
    {
        Title,
        Game
    }

    public static void LoadScene(GameMode nextScene)
    {
        sceneName = nextScene.ToString();
        SceneManager.LoadScene("Load");
    }

    private void Start()
    {
        Time.timeScale = 1;
        loadScene = SceneManager.LoadSceneAsync(sceneName);
        loadScene.allowSceneActivation = false;
        StartCoroutine(load());
    }

    IEnumerator load()
    {
        yield return new WaitForSeconds(LoadTimeMin);
        loadScene.allowSceneActivation = true;
        yield break;
    }
}