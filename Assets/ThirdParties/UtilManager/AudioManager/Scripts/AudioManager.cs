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
    GameEndDefeat,
    GameTouch,
    PlayerGetItem,
    PlayerGetWeaponC,
    PlayerHit,
    PlayerLevelUp,
    WeaponBLaunch,
    WeaponDExplosion,
    WeaponDLaunch,
    GameEndClear,
    GameEndChallenge,
    PlayerGetWeaponA,
    PlayerGetSpecialItem,
    BarrierHit,
    EnemyDeadA,
    EnemyDeadB,
    EnemyDeadC,
    EnemyDeathD,
    EnemyDeadE,
    EnemyDeadF,
    EnemyDeadG,
    EenemyDeathH,
    EenemyDeathI,
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
    [SerializeField] private Sound[] enemySounds;
    [SerializeField] private AudioSource sourceBGM;
    [SerializeField] private AudioSource sourceSFX;

    [SerializeField] private AudioSource[] SFXSources;

    public bool isLoadComplete { get; private set; }

    public Dictionary<SoundKey, Sound> soundDictionary = new Dictionary<SoundKey, Sound>();
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
    public void Load(Action callback = null)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            List<Sound> soundList = new List<Sound>();
            soundList.AddRange(sounds);
            soundList.AddRange(enemySounds);

            foreach (Sound sound in soundList)
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
            AudioSource src = null;
            int overlapCount = 0;

            for (int i = 0; i < SFXSources.Length; i++)
            {
                if (!SFXSources[i].isPlaying)
                {
                    src = SFXSources[i];
                    break;
                }
                else if (SFXSources[i].clip == sound.audioClip)
                {
                    overlapCount++;

                    if (overlapCount > 1)
                    {
                        return;
                    }
                }
            }

            if (src != null)
            {
                src.clip = sound.audioClip;
                src.Play();
            }
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
        for (int i = 0; i < SFXSources.Length; i++)
        {
            SFXSources[i].volume = volume;
        }
    }
}