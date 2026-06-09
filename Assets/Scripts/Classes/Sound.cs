// Samuel Markus
// 1/22/2026
// Sound class
// References: Brackeys

using System;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public GameObject attachedGameObject;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool mute;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
