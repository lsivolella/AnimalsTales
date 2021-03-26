using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    PlayableDirector playableDirector;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void Start()
    {
        if (!SceneController.instance.CinematicsPlayed)
        {
            StartTimeline();
            SceneController.instance.CinematicsPlayed = true;
        }
    }

    private void GetAccessToComponents()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    public void StartTimeline()
    {
        playableDirector.Play();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    StartTimeline();
        //}
    }
}
