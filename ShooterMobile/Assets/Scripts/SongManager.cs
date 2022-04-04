using UnityEngine;
using UnityEngine.SceneManagement;

public class SongManager : MonoBehaviour
{
    [SerializeField] private AudioClip gameSong;
    [SerializeField] private AudioClip menuSong;

    string sceneName;

    private void Start()
    {
        OnLevelWasLoaded(0);
    }

    private void OnLevelWasLoaded(int level)
    {
        string newScene = SceneManager.GetActiveScene().name;
        if (newScene != sceneName)
        {
            sceneName = newScene;
            Invoke("playMusic", .5f);
        }
    }

    void playMusic() {
        AudioClip clip = null;

        if (sceneName == "MenuScene")
        {
            clip = menuSong;
        }
        else if (sceneName == "Game")
        {
            clip = gameSong;
        }

        if (clip != null)
        {
            AudioManager.instance.playSong(clip, 2);
            Invoke("playMusic", clip.length);
        }
    }
}
