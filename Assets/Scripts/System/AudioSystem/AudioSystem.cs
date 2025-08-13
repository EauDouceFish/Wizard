using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : AbstractSystem
{
    private GameCoreModel gameCoreModel;
    private AudioModel audioModel;
    private Storage storage;

    // 音频源管理
    private AudioSource musicAudioSource;
    private AudioSource soundEffectAudioSource;

    protected override void OnInit()
    {
        gameCoreModel = this.GetModel<GameCoreModel>();
        audioModel = this.GetModel<AudioModel>();
        storage = this.GetUtility<Storage>();

        CreateAudioSources();

        this.RegisterEvent<MusicVolumeChangedEvent>(OnMusicVolumeChanged);
        this.RegisterEvent<SoundEffectVolumeChangedEvent>(OnSoundEffectVolumeChanged);
    }

    private void CreateAudioSources()
    {
        FindExistingAudioSources();
        if (musicAudioSource == null)
        {
            CreateMusicAudioSource();
        }

        if (soundEffectAudioSource == null)
        {
            CreateSoundEffectAudioSource();
        }

        UpdateAudioSourceSettings();
    }

    private void FindExistingAudioSources()
    {
        GameObject existingMusicGO = GameObject.Find("MusicAudioSource");
        if (existingMusicGO != null)
        {
            musicAudioSource = existingMusicGO.GetComponent<AudioSource>();
        }

        GameObject existingSoundGO = GameObject.Find("SoundEffectAudioSource");
        if (existingSoundGO != null)
        {
            soundEffectAudioSource = existingSoundGO.GetComponent<AudioSource>();
        }
    }

    #region 生成管理器

    private void CreateMusicAudioSource()
    {
        GameObject musicGO = new GameObject("MusicAudioSource");
        GameObject.DontDestroyOnLoad(musicGO);
        musicAudioSource = musicGO.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
    }

    private void CreateSoundEffectAudioSource()
    {
        GameObject soundGO = new GameObject("SoundEffectAudioSource");
        GameObject.DontDestroyOnLoad(soundGO);
        soundEffectAudioSource = soundGO.AddComponent<AudioSource>();
        soundEffectAudioSource.playOnAwake = false;
    }

    private void UpdateAudioSourceSettings()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = audioModel.MusicVolume.Value;
        }

        if (soundEffectAudioSource != null)
        {
            soundEffectAudioSource.volume = audioModel.SoundEffectVolume.Value;
        }
    }
    #endregion

    #region 音乐管理

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicAudioSource.clip == musicClip && musicAudioSource.isPlaying) return;

        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void ChangeMusicWithDelay(AudioClip newClip, float delay = 3.0f)
    {
        musicAudioSource.Pause();
        gameCoreModel.GameCoreController.StartCoroutine(ChangeMusicDelayCoroutine(newClip, delay));
    }

    private IEnumerator ChangeMusicDelayCoroutine(AudioClip newClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        musicAudioSource.clip = newClip;
        musicAudioSource.Play();
    }

    public void FadeOutAndChangeMusic(AudioClip newClip, float fadeOutDuration = 1f, float fadeInDuration = 1f)
    {
        gameCoreModel.GameCoreController.StartCoroutine(FadeOutAndChangeMusicCoroutine(newClip, fadeOutDuration, fadeInDuration));
    }

    private IEnumerator FadeOutAndChangeMusicCoroutine(AudioClip newClip, float fadeOutDuration, float fadeInDuration)
    {
        float startVolume = musicAudioSource.volume;
        float elapsedTime = 0f;

        // 淡出
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        musicAudioSource.volume = 0f;

        // 切换音乐
        musicAudioSource.clip = newClip;
        musicAudioSource.Play();

        // 淡入
        elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            musicAudioSource.volume = Mathf.Lerp(0f, startVolume, t);
            yield return null;
        }

        musicAudioSource.volume = startVolume;
    }

    #endregion

    #region 音效管理

    public void PlaySoundEffect(AudioClip audioClip, float volumeMultiplier = 1.0f)
    {
        if (audioClip == null) return;

        float finalVolume = audioModel.SoundEffectVolume.Value * volumeMultiplier;
        soundEffectAudioSource.PlayOneShot(audioClip, finalVolume);
    }

    public void PlaySoundEffect(AudioClip[] audioClipArray, float volumeMultiplier = 1.0f)
    {
        if (audioClipArray == null || audioClipArray.Length == 0) return;

        AudioClip randomClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        PlaySoundEffect(randomClip, volumeMultiplier);
    }

    #endregion

    #region 事件监听

    private void OnMusicVolumeChanged(MusicVolumeChangedEvent evt)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = evt.newVolume;
        }
    }

    private void OnSoundEffectVolumeChanged(SoundEffectVolumeChangedEvent evt)
    {
        if (soundEffectAudioSource != null)
        {
            soundEffectAudioSource.volume = evt.newVolume;
        }
    }

    #endregion
}