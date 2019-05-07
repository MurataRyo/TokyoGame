using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTask : MonoBehaviour
{
    public enum Mode
    {
        main,
        gameOver,
        gameClear
    }
    public Mode mode;
    private Mode modeLog;
    public static GameObject stageData;

    private void Start()
    {
        modeLog = mode = Mode.main;

        if (stageData != null)
            Instantiate(stageData);
    }

    private void Update()
    {
        //タイトル画面へ
        if (mode != Mode.main && Utility.EnterButton())
        {
            SceneTask.LoadScene(SceneTask.GameMode.Title);
            return;
        }

        if (mode != modeLog)
        {
            switch (mode)
            {
                case Mode.gameOver:
                    GameOver();
                    break;

                case Mode.gameClear:
                    GameClear();
                    break;
            }
        }
        modeLog = mode;
    }

    private void GameOverAndClear()
    {
        Time.timeScale = 0f;
    }

    private void GameOver()
    {
        GameOverAndClear();
        Sprite sprite = Resources.Load<Sprite>(GetPath.Game + "/GameOver");
        Utility.UiAdd(sprite, Vector2.zero, Utility.GAME_SIZE);
    }

    private void GameClear()
    {
        GameOverAndClear();
        Sprite sprite = Resources.Load<Sprite>(GetPath.Game + "/GameClear");
        Utility.UiAdd(sprite, Vector2.zero, Utility.GAME_SIZE);
    }
}
