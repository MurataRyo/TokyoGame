using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTask : MonoBehaviour
{
    public static string sceneName;
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
        SceneManager.LoadScene(sceneName);
    }
}