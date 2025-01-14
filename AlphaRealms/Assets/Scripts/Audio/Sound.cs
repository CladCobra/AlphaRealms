using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

    [Header("Settings")]
    public string name;
    public AudioClip audioClip;
    [Range(0f, 1f)] public float volume;
    [Range(-3f, 3f)] public float pitch;
    public bool loop;
    [HideInInspector] public AudioSource audioSource;

}
