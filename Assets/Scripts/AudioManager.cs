using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    public float volume;
    [HideInInspector]
    public AudioSource source;
}
public class AudioManager : MonoBehaviour {
    /*//for testing audio
    float gunTimer = 0f;
    float gunTimer2 = 0f;
    float gunTimer3 = 0f;
    float gunTimer4 = 0f;
    float gunTimer5 = 0f;
    float gunTimer6 = 0f;
    bool trigger = true;
    bool trigger2;
    bool trigger3;
    bool trigger4;
    bool trigger5;
    bool trigger6;
    float m_Time = 0; 
    //*/
    //testing sounds
    public Sound[] sounds;

    //in-game sounds
    public Sound[] troopSounds; 
    void Awake() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
        }
    }
    public void Play(string name, float volume) {
        foreach (Sound s in troopSounds) {
            if (s.name == name) {
                GetComponent<AudioSource>().PlayOneShot(s.clip, s.volume * volume / 10f);
                return; 
            }
        }
        foreach (Sound s in sounds) {
            if (s.name == name) {
                GetComponent<AudioSource>().PlayOneShot(s.clip, s.volume * volume / 10f);
                return; 
            }
        }
    }
    void Update() {
        /*
        if (Time.time > 2f && Time.time < 2.5f)
            trigger = true;
        else
            trigger = false;

        if (gunTimer > 0) {
            gunTimer -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha1) || trigger) {
                gunTimer = 0.16f;
                Play("rifle", 3);
            }
        }
        if (gunTimer2 > 0) {
            gunTimer2 -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha2) || trigger2) {
                gunTimer2 = 0.048f;
                Play(sounds[1].name, sounds[1].volume);
            }
        }
        if (gunTimer3 > 0) {
            gunTimer3 -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha3) || trigger3) {
                gunTimer3 = 0.06f;
                Play(sounds[2].name, sounds[2].volume);
            }
        }
        if (gunTimer4 > 0) {
            gunTimer4 -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha4) || trigger4) {
                gunTimer4 = 0.1f;
                Play(sounds[3].name, sounds[3].volume);
            }
        }
        if (gunTimer5 > 0) {
            gunTimer5 -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha5) || trigger5) {
                gunTimer5 = 1f;
                Play(sounds[4].name, sounds[4].volume);
            }
        }
        if (gunTimer6 > 0) {
            gunTimer6 -= Time.deltaTime;
        } else {
            if (Input.GetKey(KeyCode.Alpha6) || trigger6) {
                gunTimer6 = 1f;
                Play(sounds[5].name, sounds[5].volume);
            }
        }
        //*/
    }
}
