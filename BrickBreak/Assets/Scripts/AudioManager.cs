using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static Action<string> PlayClip;
    
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<AudioProperty> audioPairs;

    Dictionary<string, AudioProperty> audioDict;
    bool loaded = false;
    List<AudioSource> activeSources = new List<AudioSource>();

    private void Awake()
    {
        PlayClip += PlayAudioClip;
        activeSources.Add(Instantiate(audioSource, transform));
        activeSources[0].gameObject.SetActive(true);

        audioDict = new Dictionary<string, AudioProperty>();
        foreach (var pair in audioPairs)
        {
            if (audioDict.ContainsKey(pair.name))
            {
                Debug.LogError("Found duplicate audio key! Only first instance will be used: " + pair.name);
            }
            else audioDict.Add(pair.name, pair);
        }

        Debug.Log($"Successfully loaded {audioDict.Count} entries into the audio manager.");
        loaded = true;
    }

    public void PlayAudioClip(string key)
    {
        if (loaded)
        {
            if (audioDict.ContainsKey(key))
            {
                var property = audioDict[key];

                // find an available source, if none then make one
                int source = 0;
                while (source < activeSources.Count && activeSources[source].isPlaying)
                    source++;

                if (source >= activeSources.Count)
                {
                    var newSource = Instantiate(audioSource, transform);
                    newSource.gameObject.SetActive(true);
                    activeSources.Add(newSource);
                }

                // load and play
                activeSources[source].clip = property.clip;
                activeSources[source].volume = property.volume;
                activeSources[source].Play();
            }
        }
    }

    public void EnableSound(bool enable)
    {
        foreach (var source in activeSources)
        {
            source.mute = !enable;
        }
    }

    [Serializable]
    struct AudioProperty
    {
        public string name;
        public AudioClip clip;
        public float volume;
    }
}
