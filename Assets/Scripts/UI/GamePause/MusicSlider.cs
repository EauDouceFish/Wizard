using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour, IController
{
    [SerializeField] private Slider musicSlider;

    private AudioModel audioModel;
    private IUnRegister unRegister;

    private void Start()
    {
        audioModel = this.GetModel<AudioModel>();

        if (musicSlider != null)
        {
            musicSlider.value = audioModel.MusicVolume.Value;
            musicSlider.onValueChanged.AddListener(OnSliderValueChanged);
            unRegister = audioModel.MusicVolume.RegisterWithInitValue(OnModelVolumeChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        audioModel.SetMusicVolume(value);
    }

    private void OnModelVolumeChanged(float newVolume)
    {
        if (musicSlider != null && !Mathf.Approximately(musicSlider.value, newVolume))
        {
            musicSlider.value = newVolume;
        }
    }

    private void OnDestroy()
    {
        unRegister?.UnRegister();

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}