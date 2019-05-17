using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTask : MonoBehaviour
{
    const float t = 0.08f;
    public static void ChangeBgm(string path)
    {
        AudioSource source = Utility.GetTaskObject().GetComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>(path);
        source.time = t;
        source.Play();
    }

    public static void ChangeBgm(string path,float v)
    {
        AudioSource source = Utility.GetTaskObject().GetComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>(path);
        source.volume = v;
        source.time = t;
        source.Play();
    }

    public static void PlaySe(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        source.time = t;
        source.clip = clip;
        source.PlayOneShot(clip);
        Destroy(go, clip.length);
    }

    public static void PlaySe(string path,float v)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        source.time = t;
        source.clip = clip;
        source.volume = v;
        source.PlayOneShot(clip);
        Destroy(go, clip.length);
    }

    public static AudioSource Audio()
    {
        return GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }
}
