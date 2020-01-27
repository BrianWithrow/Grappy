using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public AudioClip clip;

    public string name;

    [Range(0.0f, 2.0f)]
    public float volume;

    [Range(0.1f, 3.0f)]
    public float pitch;

    public bool loop;
    public bool playing;

    [HideInInspector]
    public AudioSource source;
}
