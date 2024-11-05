using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractionRCC : MonoBehaviour
{

    //Radio
    private AudioSource Radio_AudioSource;
    public AudioClip[] Radio_Channels;
    private int channel = 0;
   // private bool entered;
    // Start is called before the first frame update
    void Start()
    {
        Radio_AudioSource = gameObject.AddComponent<AudioSource>();

    }
    public void OpenRadio()
    {
     
      Radio_AudioSource.clip = Radio_Channels[channel];
      Radio_AudioSource.Play();
               
        channel++;
        if (channel == Radio_Channels.Length)
            channel = 0; 
    }

    public void CloseRadio()
    {
        Radio_AudioSource.Stop();
    }

    //public void WindowBtnPressed()
    //{
    //    entered = true;
        
    //}

    //public void WindowBtnReleased()
    //{

    //}

    //// Update is called once per frame
    //public void RollDownWindows()
    //{
    //    if (entered)
    //    {

    //    }
    //}
}
