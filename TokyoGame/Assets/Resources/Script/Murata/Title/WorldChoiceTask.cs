using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldChoiceTask : MonoBehaviour
{
    private World[] worlds;                     //ワールドデータ
    public ChoiceClass choiceClass;             //選択用class
    private Camera mCamera;                     //カメラ
    private const float CAMERA_TIME = 0.40f;    //カメラの移動時間
    private const float HIGH_CAMERA_TIME = 0.30f;    //カメラの移動時間
    public IEnumerator moveCamera;
    private XBox xBox;
    private GameObject titleBarPrefab;
    public int count;
    private TitleTask titleTask;

    public const float choiceBarPos = -350f;
    private List<ChoiceBar> choiceBars = new List<ChoiceBar>();

    // Start is called before the first frame update
    void Awake()
    {
        xBox = Utility.GetXBox();
        titleBarPrefab = Resources.Load<GameObject>(GetPath.TitlePrefab + "/TitleBar");
        worlds = GetComponent<WorldData>().worlds;
        choiceClass = new ChoiceClass(worlds.Length);
        mCamera = Camera.main;
        count = 0;
        titleTask = Utility.GetTaskObject().GetComponent<TitleTask>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraChoice();
    }

    private void CameraChoice()
    {
        if (count != 0 || titleTask.enumerator != null)
            return;

        if (xBox.ButtonDown(XBox.AxisStr.LeftJoyUp,false))
        {
            NextBar(true);
        }
        else if (xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, true))
        {
            NextBar(false);
        }
    }

    private void NextBar(bool flag)
    {
        AudioTask.PlaySe(GetPath.Se + "/Landing");
        count++;
        choiceClass.ChoiceChange(flag);
        float time = choiceClass.nowChoice >= GameTask.choiceStage ? HIGH_CAMERA_TIME : CAMERA_TIME;
        moveCamera = MoveCamera(worlds[choiceClass.nowChoice].cameraPos, worlds[choiceClass.nowChoice].cameraAngle, time, MoveMode.normal);
        StartCoroutine(moveCamera);
    }

    //boolには上か下かを判定させる
    private void RemoveChoiceBar(bool flag)
    {
        ChoiceBar DestroyBar = choiceBars[0];

        for (int i = 1; i < choiceBars.Count; i++)
        {
            if (flag && DestroyBar.go.transform.position.y < choiceBars[i].go.transform.position.y)
                DestroyBar = choiceBars[i];

            if (!flag && DestroyBar.go.transform.position.y > choiceBars[i].go.transform.position.y)
                DestroyBar = choiceBars[i];
        }

        Destroy(DestroyBar.go);
        choiceBars.Remove(DestroyBar);
    }

    //-1 0 1のみ
    private int ChoiceNum(int addNum)
    {
        if (choiceClass.nowChoice + addNum >= 0 &&
            choiceClass.nowChoice + addNum < choiceClass.choiceNum)
            return choiceClass.nowChoice + addNum;

        if (choiceClass.nowChoice + addNum < 0)
            return choiceClass.choiceNum - 1;

        return 0;
    }

    public enum MoveMode
    {
        start,
        normal
    }

    public IEnumerator MoveCamera(Vector3 cameraPos, Vector3 cameraAngle, float t, MoveMode moveMode)
    {
        float moveRange = (cameraPos - mCamera.transform.position).magnitude / t;
        float timer = 0;
        Vector3 pos = mCamera.transform.position;
        Vector3 angle = mCamera.transform.eulerAngles;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > t)
            {
                mCamera.transform.position = cameraPos;
                mCamera.transform.eulerAngles = cameraAngle;

                if (moveMode == MoveMode.start)
                count--;
                yield break;
            }
            float time = timer / t;
            mCamera.transform.position = Vector3.Lerp(pos, cameraPos, time);
            mCamera.transform.eulerAngles = MoveRangeAngle(angle, cameraAngle, time);
            yield return null;
        }
    }

    private Vector3 MoveRangeAngle(Vector3 now, Vector3 next, float t)
    {
        return new Vector3(Mathf.LerpAngle(now.x, next.x, t), Mathf.LerpAngle(now.y, next.y, t), Mathf.LerpAngle(now.z, next.z, t));
    }

    private ChoiceBar CreateChoiceBar(int StageNum, Vector2 addPos)
    {
        GameObject go = Instantiate(titleBarPrefab);
        go.transform.SetParent(Utility.GetCanvas().transform);
        go.transform.localPosition = new Vector2(0f, choiceBarPos) + addPos;
        string path = GetPath.StagePlate + "/Stage" + StageNum.ToString();
        if(!worlds[StageNum - 1].choiceFlag)
        {
            path += "_unknown";
        }
        ChoiceBar choiceBar = new ChoiceBar(path, go);

        return choiceBar;
    }
}

public class ChoiceBar
{
    public GameObject go;
    Text stageName;
    public const float UpDownRange = 150f;
    Image BackImage;
    public Vector2 nextPos;
    public ChoiceBar(string stagePath, GameObject go)
    {
        this.go = go;
        BackImage = go.GetComponent<Image>();
        BackImage.sprite = Resources.Load<Sprite>(stagePath);
        ColorChange();
        nextPos = go.transform.position;
    }

    public void AddPos(float addPos)
    {
        go.transform.position += new Vector3(0f, addPos, 0f);
    }

    public void ColorChange()
    {
        float f = WorldChoiceTask.choiceBarPos - go.transform.localPosition.y;
        f = Mathf.Abs(f);
        float alpha = (1 / -WorldChoiceTask.choiceBarPos) * (-WorldChoiceTask.choiceBarPos - f * 2f);
        if (alpha < 0.1f)
            alpha = 0.1f;
        AlphaColor(BackImage, alpha);
    }

    private void AlphaColor(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
}


