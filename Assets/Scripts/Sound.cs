using UnityEngine.Audio;
using UnityEngine;


public enum SoundType
{
    Music,
    SFX
}

[System.Serializable]
public class Sound
{
    public string name;
    public SoundType type;
    public AudioClip clip;
    [Range(0.0f, 1.0f)] public float volume = 1f;
    [Range(0.1f, 3.0f)] public float pitch = 1f;
    public bool loop = false;
    public bool playOnAwake = true;
    [HideInInspector] public AudioSource source;
}