using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsSlider : MonoBehaviour, IController
{
    [SerializeField] private Slider soundEffectsSlider;

    private AudioModel audioModel;
    private IUnRegister unRegister;

    private void Start()
    {
        audioModel = this.GetModel<AudioModel>();

        if (soundEffectsSlider != null)
        {
            soundEffectsSlider.value = audioModel.SoundEffectVolume.Value;
            soundEffectsSlider.onValueChanged.AddListener(OnSliderValueChanged);
            unRegister = audioModel.SoundEffectVolume.RegisterWithInitValue(OnModelVolumeChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        audioModel.SetSoundEffectVolume(value);
    }

    private void OnModelVolumeChanged(float newVolume)
    {
        if (soundEffectsSlider != null && !Mathf.Approximately(soundEffectsSlider.value, newVolume))
        {
            soundEffectsSlider.value = newVolume;
        }
    }

    private void OnDestroy()
    {
        // 清理事件监听
        unRegister?.UnRegister();
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}