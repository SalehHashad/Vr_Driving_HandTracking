using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    public EnumEventChannelSo HitSoundEffect;
    public EnumEventChannelSo TrafficViolation_SoundEffects;


    private void OnEnable()
    {
        HitSoundEffect.OnEventRaised += PlaySoundEffect;
        TrafficViolation_SoundEffects.OnEventRaised += PlaySoundEffect;
    }

    private void OnDisable()
    {
        HitSoundEffect.OnEventRaised -= PlaySoundEffect;
        TrafficViolation_SoundEffects.OnEventRaised -= PlaySoundEffect;

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }


    public void PlaySoundEffect(SoundType soundType)
    {
        foreach (var sound in sounds)
        {
            if (sound.soundType == soundType)
            {
                sound.SoundEffects.PlayOneShot(sound.clip, sound.VolumeLevel);
                break;
            }
        }
    }
}
