using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SailsState
{
    SailsDown=0,
    SailsUp=1
}

public class SailsController : MonoBehaviour
{
    private static SailsController _instance;
    public static SailsController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private SailsState state;
    public SailsState State { get => state; set => state = value; }

    public GameObject sails;
    private Animator sailsAnimator;

    private void Awake()
    {
        Debug.Log("AWAKE");
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("DESTROY");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
                InitializeSailsController();
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeSailsController()
    {
        state = SailsState.SailsDown;
        sailsAnimator = sails.GetComponent<Animator>();
    }

    public void SetState(SailsState newState)
    {
        if (state != newState)
        {
            state = newState;
            if (state == SailsState.SailsUp)
            {
                //TODO: sound?
            }
            else if (state == SailsState.SailsUp)
            {
                //TODO: sound?
            }
            sailsAnimator.SetBool("SailsUp", state == SailsState.SailsUp);
            sailsAnimator.SetBool("SailsDown", state == SailsState.SailsDown);
        }
    }
}
