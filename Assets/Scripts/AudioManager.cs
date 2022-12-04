
using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

/*
THANK YOU BRACKEYS
*/

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<Sound> sounds = new List<Sound>();

    void Awake()
    {
        if (AudioManager.Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("There can only be one AudioManager");
            return;
        }

        DontDestroyOnLoad(gameObject);

        AudioManager.Instance = this;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            if (sound.playOnAwake)
                sound.source.Play();
        }
    }

    public void Play(string _name)
    {
        Sound sound = sounds.Find(sounds => sounds.name == _name);
        if (sound != null)
            sound.source.Play();
        else
            Debug.LogError("No sound with name " + _name + " exists.");
    }

    public void Stop(string _name)
    {
        Sound sound = sounds.Find(sounds => sounds.name == _name);
        if (sound != null)
            sound.source.Stop();
        else
            Debug.LogError("No sound with name " + _name + " exists.");
    }

    public void SetVolume(string _name, float _volume)
    {
        Sound sound = sounds.Find(sounds => sounds.name == _name);
        if (sound != null)
            sound.source.volume = _volume;
        else
            Debug.LogError("No sound with name " + _name + " exists.");
    }

    public void SetVolume(SoundType _type, float _volume)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.type == _type)
                sound.source.volume = _volume;
        }
    }
}