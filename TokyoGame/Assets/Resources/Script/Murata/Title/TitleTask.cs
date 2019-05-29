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
    GameObject title;
    public IEnumerator enumerator = null;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
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
        if (enumerator != null)
            return;

        switch (titleMode)
        {
            case TitleMode.title:
                TitleMove();
                break;

            case TitleMode.select:
                enumerator = SelectMove();
                StartCoroutine(enumerator);
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

    public static void StageLoad()
    {
        GameTask.stageData = Resources.Load<GameObject>(GetPath.PlayStage + "/Stage" + GameTask.nowStage.ToString());
    }

    private IEnumerator SelectMove()
    {
        if(Utility.EnterButton() && worldChoiceTask.count == 0)
        {
            AudioTask.PlaySe(GetPath.Se + "/Ok");
            GameTask.nowStage = worldChoiceTask.choiceClass.nowChoice + 1;
            StageLoad();
            yield return new WaitForSeconds(1f);
            SceneTask.LoadScene(SceneTask.GameMode.Game);
        }
        enumerator = null;
        yield break;
    }

    private void TitleToSelect()
    {
        AudioTask.PlaySe(GetPath.Se + "/Ok");
        worldChoiceTask = gameObject.AddComponent<WorldChoiceTask>();
        worldChoiceTask.count++;
        Destroy(title);

        worldChoiceTask.moveCamera = worldChoiceTask.MoveCamera(worldData.worlds[0].cameraPos, worldData.worlds[0].cameraAngle,1.0f,WorldChoiceTask.MoveMode.start);
        StartCoroutine(worldChoiceTask.moveCamera);
    }
}
