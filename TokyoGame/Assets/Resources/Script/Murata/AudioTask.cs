using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTask : MonoBehaviour
{
    public static void PlayBgm()
    {

    }

    public static void PlaySe(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.PlayOneShot(clip);
        Destroy(go, clip.length);
    }

    public static AudioSource Audio()
    {
        return GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }
}
