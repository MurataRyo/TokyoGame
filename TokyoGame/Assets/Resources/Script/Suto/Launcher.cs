using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    XBoxController controller;
    [SerializeField] bool FanLight;

    private AudioClip se;
    private AudioSource audio;

    private 

    void Start()
    {
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
        audio = gameObject.AddComponent<AudioSource>();
        se = Resources.Load<AudioClip>(GetPath.Se + "/LightRotation");
    }

    // 向きを変える
    public void RotateLaunch()
    {
        if (FanLight)
            transform.Rotate(0f, controller.LaunchMoveButton() * -1f, 0f);
        else
            transform.Rotate(0f, 0f, controller.LaunchMoveButton() * -1f);

        if(controller.LaunchMoveButton() != 0f)
        {
            audio.PlayOneShot(se);
        }
    }
}

