using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Gameobject")]
public class GameObjectEventChannelSO : ScriptableObject
{
    public UnityAction<GameObject> OnEventRaised;

    public void RaiseEvent(GameObject gameObject)
    {
        OnEventRaised?.Invoke(gameObject);
    }
}