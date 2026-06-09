// Samuel Markus
// 1/22/2026
// Audio Manager
// References: Brackeys, gamedevbeginner.com for singleton behavior

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } // Singleton
    public Sound[] sounds; // Sound list

    // On Initialization/Awake
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Allows for peristence through scenes

        // Singleton: Check if instance already exists, destroy self if true.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Sound setup
        foreach (Sound s in sounds)
        {
            // Attach to GameObject
            if (s.attachedGameObject == null) { s.attachedGameObject = gameObject; }
            s.source = s.attachedGameObject.AddComponent<AudioSource>();

            // Apply settings
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.mute = s.mute;
            s.source.loop = s.loop;
        }
    }

    // Play audio. Run this function in scripts.
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
            return;
        }
        s.source.Play();
    }
}
