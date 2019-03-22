using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTask : MonoBehaviour
{
    enum TitleMode
    {
        title,
        select
    }
    TitleMode titleMode;
    TitleMode titleModeLog;
    WorldChoiceTask worldChoiceTask;
    WorldData worldData;
    GameObject canvas;
    GameObject title;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        canvas = Utility.GetCanvas();
        titleModeLog = titleMode = TitleMode.title;
        AddTasks();
    }

    private void AddTasks()
    {
        worldData = gameObject.AddComponent<WorldData>();
        CameraPosReset();
        AddTitle();
    }

    private void CameraPosReset()
    {
        Camera.main.transform.position = worldData.worlds[0].cameraPos;
        Camera.main.transform.eulerAngles = worldData.worlds[0].cameraAngle;
    }

    private void AddTitle()
    {
        Sprite sprite = Resources.Load<Sprite>(GetPath.Title + "/Title");
        title = Utility.UiAdd(sprite,new Vector2(0,0),Utility.GAME_SIZE);
    }

    // Update is called once per frame
    void Update()
    {
        switch (titleMode)
        {
            case TitleMode.title:
                TitleMove();
                break;

            case TitleMode.select:
                SelectMove();
                break;
        }

        if (titleModeLog != titleMode)
        {
            switch (titleModeLog)
            {
                case TitleMode.title:
                    TitleToSelect();
                    break;

                case TitleMode.select:
                    break;
            }
        }
        titleModeLog = titleMode;
    }

    private void TitleMove()
    {
        if (Utility.EnterButton())
            titleMode = TitleMode.select;
    }

    private void SelectMove()
    {
        if(Utility.EnterButton())
        {
            SceneTask.LoadScene(SceneTask.GameMode.Game);
        }
    }

    private void TitleToSelect()
    {
        worldChoiceTask = gameObject.AddComponent<WorldChoiceTask>();
        Destroy(title);
    }
}
