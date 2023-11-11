using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIOptionsPanel : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        ReadAudioMixerVolumes();
    }

    public void Show()
    {
        this.panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        this.panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Toggle()
    {
        if (!this.panel.activeSelf)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void ReadAudioMixerVolumes()
    {
        this.audioMixer.GetFloat("MasterSoundVolume", out float soundVolumeRaw);
        this.soundSlider.value = Mathf.Pow(10.0f, soundVolumeRaw / 20.0f);

        this.audioMixer.GetFloat("MasterMusicVolume", out float musicVolumeRaw);
        this.musicSlider.value = Mathf.Pow(10.0f, musicVolumeRaw / 20.0f);
    }

    public void UpdateMasterSoundVolume()
    {
        float newSoundVolume = Mathf.Clamp(Mathf.Log10(this.soundSlider.value) * 20f, -80f, 0f);
        this.audioMixer.SetFloat("MasterSoundVolume", newSoundVolume);
    }

    public void UpdateMasterMusicVolume()
    {
        float newMusicVolume = Mathf.Clamp(Mathf.Log10(this.musicSlider.value) * 20f, -80f, 0f);
        this.audioMixer.SetFloat("MasterMusicVolume", newMusicVolume);
    }

}
