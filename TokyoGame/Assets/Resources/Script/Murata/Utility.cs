using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    

    public static Vector2 GAME_SIZE = new Vector2(1920,1080);

    public static GameObject UiAdd(Sprite image,Vector2 pos,Vector2 size)
    {
        GameObject go = new GameObject();
        RectTransform rectTransform = go.AddComponent<RectTransform>();
        rectTransform.position = pos + GAME_SIZE / 2;
        rectTransform.sizeDelta = size;

        go.AddComponent<Image>().sprite = image;
        go.transform.SetParent(GetCanvas().transform);
        return go;
    }

    public static GameObject GetCanvas()
    {
        return GameObject.FindGameObjectWithTag("Canvas");
    }

    public static XBox GetXBox()
    {
        return GetTaskObject().GetComponent<XBox>();
    }

    public static GameObject GetTaskObject()
    {
        return GameObject.FindGameObjectWithTag(GetTag.Tasks);
    }

    public static int BoolToInt(bool flag)
    {
        return flag ? 1 : -1;
    }

    public static bool EnterButton()
    {
        XBox xBox = GetXBox();
        if (xBox.ButtonDown(XBox.Str.A))
            return true;

        if (Input.GetKeyDown(KeyCode.Space))
            return true;

        return false;
    }
}

public struct GetTag
{
    public const string Block = "Block";
    public const string LightSource = "LightSource";
    public const string Mirror = "Mirror";
    public const string Mirror_Back = "MirrorBack";
    public const string Glass = "Glass";
    public const string Player = "Player";
    public const string Tasks = "Tasks";
    public const string Col = "Col";
    public const string Col2 = "Col2";
    public const string Star = "Star";
    public const string Refrect = "Refrect";
    public const string NotRefrect = "NotRefrect";
}

public struct GetPath
{
    public const string Prefab = "Prefab";
    public const string TitlePrefab = Prefab + "/Title";
    public const string Particle = Prefab + "/Particle";
    public const string PlayStage = Prefab + "/PlayStage";

    public const string Image = "Image";
    public const string Title = Image + "/Title";

    public const string Game = Image + "/Game";
    public const string StageBack = Game + "/StageBack";
}


public class Vec2Class
{
    public Vector2 vec2 { get; set; }

    public Vec2Class()
    {

    }

    public Vec2Class(float x, float y)
    {
        vec2 = new Vector2(x, y);
    }

    public Vec2Class(Vector2 vec2)
    {
        this.vec2 = vec2;
    }
}

public class ChoiceClass
{
    public int choiceNum;   //選択できる数
    public int nowChoice;   //現在選択している数

    public ChoiceClass(int choiceNum)
    {
        this.choiceNum = choiceNum;
        this.nowChoice = 0;
    }

    public ChoiceClass(int choiceNum, int nowChoice)
    {
        this.choiceNum = choiceNum;
        this.nowChoice = nowChoice;
    }

    //選択変更  trueがplusでfalseがマイナス
    public void ChoiceChange(bool flag)
    {
        nowChoice += Utility.BoolToInt(flag);
        if(nowChoice == -1 || nowChoice == choiceNum)
        {
            if(flag)
            {
                nowChoice = 0;
            }
            else
            {
                nowChoice = choiceNum - 1;
            }
        }
    }
}