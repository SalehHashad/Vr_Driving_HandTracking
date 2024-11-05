using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<TaskSO> tasks = new List<TaskSO>();
    [SerializeField] GameObject model;
    private int currentTaskIndex = 0;
    AudioSource voiceOverAudio;
    private bool taskCompleted = false;
    public event Action OnAudioFinished;
    public event Action TaskIsFinished;

    private void Awake()
    {
        voiceOverAudio = gameObject.AddComponent<AudioSource>();
    }
    void Start()
    {
        InitializeCurrentTask();
    }

    private void InitializeCurrentTask()
    {
        if (currentTaskIndex < tasks.Count)
        {
            TaskSO currentTask = tasks[currentTaskIndex];
            currentTask.PlayVoiceOver(voiceOverAudio);
            StartCoroutine(WaitForVoiceoverFinish(currentTask.VoiceOver.length));
        }
        else
        {
            Debug.Log("All tasks completed!");
        }
    }

    IEnumerator WaitForVoiceoverFinish(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        OnVoiceoverFinished();
    }
    void OnVoiceoverFinished()
    {
        if (currentTaskIndex < tasks.Count)
        {
            TaskSO currentTask = tasks[currentTaskIndex];
            currentTask.SetGlowingObjectMaterial(model);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigggggeeeeerrrr");
            taskCompleted = true;
            if (currentTaskIndex < tasks.Count)
            {
                TaskSO currentTask = tasks[currentTaskIndex];
                currentTask.SetOriginalObjectMaterial(model);
                currentTaskIndex++;
                taskCompleted = false;
                InitializeCurrentTask();
            }
        }
    }


    //private void OnEnable()
    //{
    //    //audioIsFinished.OnEventRaised += HighlightObject;
    //    TaskIsFinished += InitiateotherTask;
    //}

    //private void OnDisable()
    //{
    //    //audioIsFinished.OnEventRaised -= HighlightObject;
    //    TaskIsFinished -= InitiateotherTask;
    //}
    //public IEnumerator PlayInstructions()
    //{
    //    if (currentTaskIndex < tasks.Length)
    //    {
    //        TaskSO currentTask = tasks[currentTaskIndex];

    //        if (currentTask.VoiceOver != null)
    //        {
    //            AudioSource audio = gameObject.AddComponent<AudioSource>();
    //            audio.clip = currentTask.VoiceOver;
    //            audio.PlayOneShot(currentTask.VoiceOver);

    //            // Wait for the audio to finish playing
    //            yield return new WaitForSeconds(audio.clip.length);
    //            //HighlightObject();
    //            model.GetComponent<MeshRenderer>().material = currentTask.GlowMaterial;
    //           // FindObjectOfType<SteeringWheel_Tag>().gameObject.GetComponent<BoxCollider>().isTrigger = true;
    //        }
    //    }
    //}


    //public void InitiateCurrentTask() 
    //{
    //    if (currentTaskIndex < tasks.Length)
    //    {
    //        // Play instructions and highlight object (using coroutine)
    //        StartCoroutine(PlayInstructions());

    //        currentTaskIndex++;
    //    }
    //    else
    //    {
    //        Debug.Log("All tasks completed!");
    //    }
    //}
    //public void InitiateotherTask() 
    //{
    //    //if (currentTaskIndex < tasks.Length)
    //    //{
    //    //    currentTaskIndex++;
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("All tasks completed!");
    //    //}

    //    Debug.Log("All tasks completed!");
    //}

}
