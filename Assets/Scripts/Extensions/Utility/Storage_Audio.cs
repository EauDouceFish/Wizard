using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Storage : IUtility
{
    private AudioSourceRefsSO m_audioSourceRefs;

    private AudioSourceRefsSO audioSourceRefs
    {
        get
        {
            if (m_audioSourceRefs == null)
            {
                m_audioSourceRefs = Resources.Load<AudioSourceRefsSO>("Config/Audio/AudioSourceRefsSO");
                if (m_audioSourceRefs == null)
                {
                    Debug.LogError("�޷�����AudioSourceRefsSO������·����Resources/Config/Audio/AudioSourceRefsSO");
                }
                else
                {
                    Debug.Log("AudioSourceRefsSO���سɹ����ѻ���");
                }
            }
            return m_audioSourceRefs;
        }
    }


    public AudioSourceRefsSO GetAudioSourceRefs() => audioSourceRefs;
    
    // ����Music
    public AudioClip GetMusicBoss() => audioSourceRefs.musicBoss;
    public AudioClip GetMusicMainMenu() => audioSourceRefs.musicMainMenu;
    public AudioClip GetMusicNatureRegion() => audioSourceRefs.musicNatureRegion;
    public AudioClip GetMusicDesertRegion() => audioSourceRefs.musicDesertRegion;
    public AudioClip GetMusicFrostRegion() => audioSourceRefs.musicForstRegion;
    public AudioClip GetMusicBarrenRegion() => audioSourceRefs.musicBarrenRegion;
    public AudioClip GetMusicVolcanoRegion() => audioSourceRefs.musicVolcanoRegion;

    // ��ЧSounds
    public AudioClip[] GetBubbleSounds() => audioSourceRefs.bubbleSounds;
    public AudioClip[] GetElementPillarSounds() => audioSourceRefs.elementPillarSounds;
    public AudioClip[] GetEnemyDefeatedSounds() => audioSourceRefs.enemyDefeatedSounds;
    public AudioClip[] GetButtonClickSounds() => audioSourceRefs.buttonClickSounds;

}
