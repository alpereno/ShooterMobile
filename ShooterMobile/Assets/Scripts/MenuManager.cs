using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject optionsMenuObject;

    public Slider[] volumeSliders;

    private void Start()
    {
        volumeSliders[0].value = AudioManager.instance.songVolumePercent;
        volumeSliders[1].value = AudioManager.instance.effectVolumePercent;
    }

    public void playGame() {
        SceneManager.LoadScene("Game");
    }

    public void optionsMenu() {
        mainMenuObject.SetActive(false);
        optionsMenuObject.SetActive(true);
    }

    public void quit() {
        Application.Quit();
    }

    public void mainMenu() {
        mainMenuObject.SetActive(true);
        optionsMenuObject.SetActive(false);
    }

    public void setVolume(float value) {
        AudioManager.instance.setVolume(volumeSliders[0].value, AudioManager.audioChannel.sound);
    }

    public void setEffectVolume(float value) {
        AudioManager.instance.setVolume(volumeSliders[1].value, AudioManager.audioChannel.effect);
    }
}
