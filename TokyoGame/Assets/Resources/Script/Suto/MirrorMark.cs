using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMark : MonoBehaviour
{
    [SerializeField] GameObject MirrorMarker;

    void Update()
    {
        MirrorMarker.SetActive(true);
    }
}
