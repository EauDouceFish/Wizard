using QFramework;
using UnityEngine;

public class AudioModel : AbstractModel
{
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SOUND_EFFECT_VOLUME_KEY = "SoundEffectVolume";

    public BindableProperty<float> MusicVolume { get; private set; }
    public BindableProperty<float> SoundEffectVolume { get; private set; }

    protected override void OnInit()
    {
        // ��PlayerPrefs������������
        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);
        float savedSoundVolume = PlayerPrefs.GetFloat(SOUND_EFFECT_VOLUME_KEY, 1.0f);

        MusicVolume = new BindableProperty<float>(savedMusicVolume);
        SoundEffectVolume = new BindableProperty<float>(savedSoundVolume);

        // ���������仯�����浽PlayerPrefs
        MusicVolume.RegisterWithInitValue(OnMusicVolumeChanged);
        SoundEffectVolume.RegisterWithInitValue(OnSoundEffectVolumeChanged);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume.Value = Mathf.Clamp01(volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        SoundEffectVolume.Value = Mathf.Clamp01(volume);
    }

    private void OnMusicVolumeChanged(float newVolume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, newVolume);
        PlayerPrefs.Save();

        this.SendEvent(new MusicVolumeChangedEvent { newVolume = newVolume });
    }

    private void OnSoundEffectVolumeChanged(float newVolume)
    {
        PlayerPrefs.SetFloat(SOUND_EFFECT_VOLUME_KEY, newVolume);
        PlayerPrefs.Save();

        this.SendEvent(new SoundEffectVolumeChangedEvent { newVolume = newVolume });
    }
}