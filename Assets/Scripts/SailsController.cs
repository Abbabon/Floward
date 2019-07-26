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
    public bool Locked = false;

    public float SailsDurability; // in percentage

    [SerializeField] private Animator sailsAnimator;

    private void Awake()
    {
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
                InitializeSailsController();
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeSailsController()
    {
        state = SailsState.SailsDown;
        SailsDurability = 100f;
    }

    public void SetState(SailsState newState)
    {
        if (state != newState)
        {
            state = newState;
            if (state == SailsState.SailsUp)
            {
                //TODO: sound?
                SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.WavingFlag);
            }
            else if (state == SailsState.SailsDown)
            {
                //TODO: sound?

                //too hard:
                //if (WindManager.Instance.State == WindState.BackWind)
                    //PressureController.Instance.ReleaseRandomValve();
            }
            sailsAnimator.SetBool("SailOpen", state == SailsState.SailsUp);
        }
    }

    private void Update()
    {
        if (State == SailsState.SailsUp && WindController.Instance.Strength() == 3 && WindController.Instance.Direction() == WindDirection.FrontWind){
            float newDurabilityValue = SailsDurability - GlobalGameplayVariables.Instance.DurabilityLossPerSecond * Time.deltaTime;
            SailsDurability = Mathf.Clamp(newDurabilityValue, 0f, 100f);
        }

        //TODO: somehow reflect this to the player!

        if (SailsDurability < 0.1f)
        {
            //TODO: Trigger tearing animation and only after that - game over sequence
            TearSailOff();
        }
    }

    private void TearSailOff()
    {
        throw new NotImplementedException();
    }
}
