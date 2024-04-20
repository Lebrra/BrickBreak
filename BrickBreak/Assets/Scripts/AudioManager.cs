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
    List<AudioPair> audioPairs;

    Dictionary<string, AudioClip> audioDict;
    bool loaded = false;
    List<AudioSource> activeSources = new List<AudioSource>();

    private void Awake()
    {
        PlayClip += PlayAudioClip;
        activeSources.Add(Instantiate(audioSource, transform));
        activeSources[0].gameObject.SetActive(true);

        audioDict = new Dictionary<string, AudioClip>();
        foreach (var pair in audioPairs)
        {
            if (audioDict.ContainsKey(pair.name))
            {
                Debug.LogError("Found duplicate audio key! Only first instance will be used: " + pair.name);
            }
            else audioDict.Add(pair.name, pair.clip);
        }

        Debug.Log($"Successfully loaded {audioDict.Count} entries into the audio manager.");
        loaded = true;
    }

    public void PlayAudioClip(string key)
    {
        if (loaded)
        {
            var clip = audioDict.ContainsKey(key) ? audioDict[key] : null;
            if (clip)
            {
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
                activeSources[source].clip = clip;
                activeSources[source].Play();
            }
        }
    }

    [Serializable]
    struct AudioPair
    {
        public string name;
        public AudioClip clip;
    }
}
