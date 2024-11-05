using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundType
{
    HitWall,
    trafficViolation
    // Add more sound types as needed
}

[System.Serializable]
public class Sound 
{
    public SoundType soundType;
    public AudioSource SoundEffects;
    public AudioClip clip;
    [Range(0, 100)]
    public float VolumeLevel;
}
