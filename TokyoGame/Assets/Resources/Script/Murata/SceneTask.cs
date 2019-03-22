using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTask : MonoBehaviour
{
    public enum GameMode
    {
        Title,
        Game
    }

    public static void LoadScene(GameMode nextScene)
    {
        SceneManager.LoadScene(nextScene.ToString());
    }
}
