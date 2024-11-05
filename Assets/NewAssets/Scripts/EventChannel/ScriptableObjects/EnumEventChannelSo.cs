using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/enum")]
public class EnumEventChannelSo : ScriptableObject
{
    public UnityAction<SoundType> OnEventRaised;
    public void RaiseEvent(SoundType soundType)
    {
        OnEventRaised?.Invoke(soundType);
    }

}
