using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public enum SoundKey
{
    BGM,
    EnemyHit,
    GameEnd,
    GameTouch,
    PlayerGetItem,
    PlayerGetWeaponC,
    PlayerHit,
    PlayerLevelUp,
    WeaponBLaunch,
    WeaponDExplosion,
    WeaponDLaunch
}
[System.Serializable]
public struct Sound
{
    public Sound(SoundKey key, AudioClip audioClip)
    {
        this.key = key; 
        this.audioClip = audioClip;
    }
    public SoundKey key;
    public AudioClip audioClip;
}

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioSource sourceBGM;
    [SerializeField] private AudioSource sourceSFX;

    public bool isLoadComplete { get; private set; }

    public Dictionary<SoundKey, Sound> soundDictionary = new Dictionary<SoundKey, Sound>();

    public void Load(Action callback = null)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            foreach (Sound sound in sounds)
            {
                soundDictionary.Add(sound.key, sound);

                yield return null;
            }

            isLoadComplete = true;

            callback?.Invoke();
        }
    }
    public void PlayBGM(SoundKey key)
    {
        if (!sourceBGM.isPlaying && 
            soundDictionary.TryGetValue(key, out Sound sound))
        {
            sourceBGM.clip = sound.audioClip;
            sourceBGM.Play();
        }
    }
    public void StopBGM()
    {
        sourceBGM.Stop();
    }
    public void PlaySFX(SoundKey key)
    {
        if (soundDictionary.TryGetValue(key, out Sound sound))
        {
            sourceSFX.PlayOneShot(sound.audioClip);
        }
    }
    public void Init(float volumeBGM, float volumeSFX)
    {
        SetVolumeBGM(volumeBGM);
        SetVolumeSFX(volumeSFX);
    }
    public void SetVolumeBGM(float volume)
    {
        sourceBGM.volume = volume;
    }
    public void SetVolumeSFX(float volume)
    {
        sourceSFX.volume = volume;
    }
}