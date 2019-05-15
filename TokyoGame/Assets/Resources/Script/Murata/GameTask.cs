using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public static int choiceStage = 1;      //選択できるステージ数
    public static int nowStage = 1;         //プレイ中のステージ
    [HideInInspector] public GameObject whiteOut;
    [HideInInspector] public Image image;
    [HideInInspector] public float alpha;

    private void Start()
    {
        modeLog = mode = Mode.main;
        whiteOut = GameObject.FindGameObjectWithTag("WhiteOut");
        image = whiteOut.GetComponent<Image>();
        image.color = new Vector4(1f, 1f, 1f, alpha);
        alpha = 0f;

        if (stageData != null)
            Instantiate(stageData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            mode = Mode.gameClear;

        //タイトル画面へ
        if (mode != Mode.main && Utility.EnterButton())
        {
            if (mode == Mode.gameClear)
            {
                if (nowStage < WorldData.MAX_WORLDS)
                {
                    nowStage++;
                    TitleTask.StageLoad();
                    SceneTask.LoadScene(SceneTask.GameMode.Game);
                }
                else
                {
                    SceneTask.LoadScene(SceneTask.GameMode.Title);
                    return;
                }
            }

            SceneTask.LoadScene(SceneTask.GameMode.Game);
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
        if (choiceStage == nowStage && nowStage < WorldData.MAX_WORLDS)
        {
            choiceStage++;
        }
        Sprite sprite = Resources.Load<Sprite>(GetPath.Game + "/GameClear");
        Utility.UiAdd(sprite, Vector2.zero, Utility.GAME_SIZE);
    }
}
