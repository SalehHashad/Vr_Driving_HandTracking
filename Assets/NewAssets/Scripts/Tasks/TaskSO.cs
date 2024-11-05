using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tasks",menuName ="Task")]
public class TaskSO : ScriptableObject
{
    public string TaskName;
    public AudioClip VoiceOver;
    //public GameObject Model;
    public Material OriginalMaterial;
    public Material GlowingMaterial;


    public void PlayVoiceOver(AudioSource audio)
    {
        if (VoiceOver !=null && audio !=null)
        {
            audio.clip = VoiceOver;
            audio.Play();
        }
    }

    public void SetGlowingObjectMaterial(GameObject glowingObject)
    {
        if (glowingObject && GlowingMaterial)
        {
            Renderer renderer = glowingObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.material = GlowingMaterial;
            }
        }
    }

    public void SetOriginalObjectMaterial(GameObject glowingObject)
    {
        if (glowingObject && OriginalMaterial)
        {
            Renderer renderer = glowingObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.material = OriginalMaterial;
            }
        }
    }



}
