using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldChoiceTask : MonoBehaviour
{
    private World[] worlds;                     //ワールドデータ
    public  ChoiceClass choiceClass;            //選択用class
    private Camera mCamera;                     //カメラ
    private const float CAMERA_TIME = 0.40f;    //カメラの移動時間
    public IEnumerator moveCamera;
    private XBox xBox;
    private GameObject titleBarPrefab;

    public const float choiceBarPos =  -350f;
    private List<ChoiceBar> choiceBars = new List<ChoiceBar>();

    // Start is called before the first frame update
    void Awake()
    {
        xBox = Utility.GetXBox();
        titleBarPrefab = Resources.Load<GameObject>(GetPath.TitlePrefab + "/TitleBar");
        worlds = GetComponent<WorldData>().worlds;
        choiceClass = new ChoiceClass(worlds.Length);
        mCamera = Camera.main;

    }

    private void StartUiAdd()
    {
        choiceBars.Add(CreateChoiceBar(worlds[choiceClass.nowChoice].worldname, Vector2.zero));
        choiceBars.Add(CreateChoiceBar(worlds[choiceClass.nowChoice + 1].worldname, Vector2.down * ChoiceBar.UpDownRange));
        choiceBars.Add(CreateChoiceBar(worlds[choiceClass.choiceNum - 1].worldname, Vector2.up * ChoiceBar.UpDownRange));
    }

    // Update is called once per frame
    void Update()
    {
        CameraChoice();
    }

    private void CameraChoice()
    {
        if (moveCamera != null)
            return;

        if (Input.GetKeyDown(KeyCode.D))
        {
            choiceClass.ChoiceChange(true);
            StartCoroutine(MoveChoiceBars(true, CAMERA_TIME));
            moveCamera = MoveCamera(worlds[choiceClass.nowChoice].cameraPos, worlds[choiceClass.nowChoice].cameraAngle, CAMERA_TIME, MoveMode.normal);
            StartCoroutine(moveCamera);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            choiceClass.ChoiceChange(false);
            StartCoroutine(MoveChoiceBars(false, CAMERA_TIME));
            moveCamera = MoveCamera(worlds[choiceClass.nowChoice].cameraPos, worlds[choiceClass.nowChoice].cameraAngle, CAMERA_TIME,MoveMode.normal);
            StartCoroutine(moveCamera);
        }
    }

    private IEnumerator MoveChoiceBars(bool flag,float t)
    {
        float timer = 0;

        RemoveChoiceBar(flag);
        foreach (ChoiceBar choiceBar in choiceBars)
        {
            choiceBar.nextPos = new Vector2(0f, Utility.BoolToInt(flag) * ChoiceBar.UpDownRange) +new Vector2( choiceBar.go.transform.position.x, choiceBar.go.transform.position.y);
        }

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > t)
            {
                foreach (ChoiceBar choiceBar in choiceBars)
                {
                    choiceBar.go.transform.position = choiceBar.nextPos;
                    choiceBar.ColorChange();
                }
                choiceBars.Add(CreateChoiceBar(worlds[ChoiceNum(Utility.BoolToInt(flag))].worldname, (flag ? Vector2.down : Vector2.up) * ChoiceBar.UpDownRange));
                yield break;
            }

            foreach(ChoiceBar choiceBar in choiceBars)
            {
                choiceBar.AddPos(Utility.BoolToInt(flag) * ChoiceBar.UpDownRange / t * Time.deltaTime);
                choiceBar.ColorChange();
            }
            yield return null;
        }
    }

    //boolには上か下かを判定させる
    private void RemoveChoiceBar(bool flag)
    {
        ChoiceBar DestroyBar = choiceBars[0];

        for(int i = 1;i < choiceBars.Count;i++)
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

    public IEnumerator MoveCamera(Vector3 cameraPos, Vector3 cameraAngle,float t, MoveMode moveMode)
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
                moveCamera = null;

                if (moveMode == MoveMode.start)
                    StartUiAdd();

                yield break;
            }
            float time = timer / t;
            mCamera.transform.position = Vector3.Lerp(pos, cameraPos,time);
            mCamera.transform.eulerAngles = MoveRangeAngle(angle, cameraAngle, time);
            yield return null;
        }
    }

    private Vector3 MoveRangeAngle(Vector3 now, Vector3 next, float t)
    {
       return  new Vector3(Mathf.LerpAngle(now.x, next.x, t), Mathf.LerpAngle(now.y, next.y, t), Mathf.LerpAngle(now.z, next.z, t));
    }

    private ChoiceBar CreateChoiceBar(string stageName,Vector2 addPos)
    {
        GameObject go = Instantiate(titleBarPrefab);
        go.transform.SetParent(Utility.GetCanvas().transform);
        go.transform.localPosition = new Vector2(0f,choiceBarPos) + addPos;
        ChoiceBar choiceBar = new ChoiceBar(stageName, go);

        return choiceBar;
    }
}

public class ChoiceBar
{
    public GameObject go;
    Text stageName;
    public const float UpDownRange = 200f;
    Image BackImage;
    public Vector2 nextPos;
    public ChoiceBar(string stageName, GameObject go)
    {
        this.go = go;
        BackImage = go.GetComponent<Image>();
        foreach (Transform child in go.transform)
        {
            if(child.name == "Text")
            {
                this.stageName = child.gameObject.GetComponent<Text>();
            }
        }
        this.stageName.text = stageName;
        ColorChange();
        nextPos = go.transform.position;
    }

    public void AddPos(float addPos)
    {
        go.transform.position += new Vector3(0f, addPos,0f);
    }

    public void ColorChange()
    {
        float f = WorldChoiceTask.choiceBarPos - go.transform.localPosition.y;
        f = Mathf.Abs(f);
        float alpha = (1 / -WorldChoiceTask.choiceBarPos) * (-WorldChoiceTask.choiceBarPos - f * 1.5f);
        if (alpha < 0.1f)
            alpha = 0.1f;
        AlphaColor(BackImage, alpha);
        AlphaColor(stageName, alpha);
    }

    private void AlphaColor(Image image,float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
    private void AlphaColor(Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}


