using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSize : MonoBehaviour {

    Renderer rend;
    // Use this for initialization
    protected virtual void Start () {
        
    }

    // Update is called once per frame
    protected virtual void Update () {
        
    }

    protected void GetTextureOne(float x, float y)
    {
        if (GetComponent<Renderer>())
        {
            rend = GetComponent<Renderer>();
            TextureSizeChange(rend, transform.lossyScale,x,y);
        }
    }

    protected void GetTexture()
    {
        if (GetComponent<Renderer>())
        {
            rend = GetComponent<Renderer>();
            TextureSizeChange(rend, transform.lossyScale);
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Renderer>())
            {
                rend = child.GetComponent<Renderer>();
                TextureSizeChange(rend, child.transform.lossyScale);
            }
        }
    }

    void TextureSizeChange(Renderer mate, Vector3 scale,float x,float y)
    {
        mate.material.SetTextureScale("_MainTex", new Vector2(scale.x / x, scale.y / y));
        if (mate.material.name == "VolcanoGround (Instance)")
        {
            mate.material.SetTextureScale("_SubTex", new Vector2(scale.x, scale.y));
        }
    }

    void TextureSizeChange(Renderer mate,Vector3 scale)
    {
        mate.material.SetTextureScale("_MainTex", new Vector2(scale.x, scale.y));
        if (mate.material.name == "VolcanoGround (Instance)")
        {
            mate.material.SetTextureScale("_SubTex", new Vector2(scale.x, scale.y));
        }
    }
}
