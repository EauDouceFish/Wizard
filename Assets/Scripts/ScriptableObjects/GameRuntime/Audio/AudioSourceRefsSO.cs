using UnityEngine;

[CreateAssetMenu(menuName = "Audio/audioSourceRefs")]
public class AudioSourceRefsSO : ScriptableObject
{
    [Header("“Ù¿÷")]
    [SerializeField] public AudioClip musicMainMenu;
    [SerializeField] public AudioClip musicNatureRegion;
    [SerializeField] public AudioClip musicDesertRegion;
    [SerializeField] public AudioClip musicBarrenRegion;
    [SerializeField] public AudioClip musicForstRegion;
    [SerializeField] public AudioClip musicVolcanoRegion;
    [SerializeField] public AudioClip musicBoss;

    [Header("“Ù–ß")]
    [SerializeField] public AudioClip[] bubbleSounds;
    [SerializeField] public AudioClip[] elementPillarSounds;
    [SerializeField] public AudioClip[] enemyDefeatedSounds;
    //[SerializeField] public AudioClip[] spellCastSounds;
    //[SerializeField] public AudioClip[] playerAttackSounds;

    [Header("UI“Ù–ß")]
    [SerializeField] public AudioClip[] buttonClickSounds;
    [SerializeField] public AudioClip[] errorSounds;
}