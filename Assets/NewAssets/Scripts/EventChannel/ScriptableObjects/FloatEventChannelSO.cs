using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/float")]
public class FloatEventChannelSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;
    public void RaiseEvent(float txt)
    {
        OnEventRaised?.Invoke(txt);
    }
   
}
