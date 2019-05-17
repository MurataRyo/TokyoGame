using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    XBoxController controller;
    [SerializeField] bool FanLight;

    private AudioClip se;
    private AudioSource audio;

    private bool flag = false;
    private bool flagLog = false;
    private bool f = false;

    private float tSe = 0f;

    void Start()
    {
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
        audio = gameObject.AddComponent<AudioSource>();
        se = Resources.Load<AudioClip>(GetPath.Se + "/LightRotation");
        audio.loop = true;
        audio.clip = se;
    }

    private void Update()
    {
        if (flagLog != flag && !f)
        {
            f = true;
            audio.Play();
        }

        if (flagLog == flag)
        {
            f = false;
            audio.Stop();
        }
        flagLog = flag;

    }

    // 向きを変える
    public void RotateLaunch()
    {
        if (FanLight)
            transform.Rotate(0f, controller.LaunchMoveButton() * -1f, 0f);
        else
            transform.Rotate(0f, 0f, controller.LaunchMoveButton() * -1f);

        if (controller.LaunchMoveButton() != 0f)
        {
            flag = !flag;
            if (audio.time >= 0.9f)
            {
                audio.time = 0.08f;
            }
        }
    }
}

