using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Sound[] sounds;

    [Header("Instance")]
    public static AudioManager instance;

    private void Awake() {

        if (instance == null) {

            instance = this;

        } else {

            Destroy(gameObject);
            return;

        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds) {

            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;

        }
    }

    public void PlaySound(string name) {

        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound.audioSource.Play();

    }
}
