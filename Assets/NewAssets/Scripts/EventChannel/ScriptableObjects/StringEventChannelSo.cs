using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/string")]
public class StringEventChannelSo : ScriptableObject
{
    public UnityAction<string> OnEventRaised;
    public void RaiseEvent(string txt)
    {
        OnEventRaised?.Invoke(txt);
    }

}
