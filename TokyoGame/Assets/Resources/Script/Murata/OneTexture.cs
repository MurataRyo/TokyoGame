using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTexture : TextureSize {
    [SerializeField] float x;
    [SerializeField] float y;
    // Use this for initialization
    protected override void Start () {
        GetTextureOne(x,y);
    }
}
