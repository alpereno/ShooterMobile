using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public enum audioChannel { sound, effect};

    [SerializeField] private Sound[] sounds;

    public float songVolumePercent { get; private set; }
    public float effectVolumePercent{ get; private set; }
    int activeSongSourceIndex;
    AudioSource[] songSources;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            songSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newSongSource = new GameObject("Song Source " + (i + 1));
                songSources[i] = newSongSource.AddComponent<AudioSource>();
                newSongSource.transform.parent = transform;
            }

            foreach (Sound s in sounds)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.audioClip;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
            }

        }
        songVolumePercent = PlayerPrefs.GetFloat("song vol", .6f);
        effectVolumePercent = PlayerPrefs.GetFloat("effect vol", 1f);
        //songSources[0].loop = true;
    }

    // to play short sound (sound effect etc...)
    // not useful for music cause cant change the volume of clip while its playing it should be Audio Source
    public void playAudio(AudioClip audioClip, Vector3 audioPos, float volumePercent) {
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, audioPos, effectVolumePercent*volumePercent);
        }
    }

    public void playAudio(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.audioSource.Play();
        }
    }

    public void playSong(AudioClip audioClip, float fadeTime = 2) {
        activeSongSourceIndex = 1 - activeSongSourceIndex;
        songSources[activeSongSourceIndex].clip = audioClip;
        songSources[activeSongSourceIndex].Play();

        StartCoroutine(fadeSong(fadeTime));
    }

    public void setVolume(float volumePercent, audioChannel channel) {
        if (channel == audioChannel.sound)
        {
            songVolumePercent = volumePercent;
            songSources[0].volume = volumePercent;
            songSources[1].volume = volumePercent;
        }
        else if (channel == audioChannel.effect)
        {
            effectVolumePercent = volumePercent;
        }
        PlayerPrefs.SetFloat("song vol", songVolumePercent);
        PlayerPrefs.SetFloat("effect vol", effectVolumePercent);
        PlayerPrefs.Save();
    }

    IEnumerator fadeSong(float fadeTime) {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeTime;
            songSources[activeSongSourceIndex].volume = Mathf.Lerp(0, songVolumePercent, percent);
            songSources[1 - activeSongSourceIndex].volume = Mathf.Lerp(songVolumePercent, 0, percent);
            yield return null;
        }
    }


    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;
        public bool loop;

        [HideInInspector] public AudioSource audioSource;
    }
}
