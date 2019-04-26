using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParent : MonoBehaviour
{
    GameObject parent;

    void Awake()
    {
        parent = GameObject.Find("LineLightParent");
    }

    void Start()
    {
        transform.parent = parent.transform;
    }
}
