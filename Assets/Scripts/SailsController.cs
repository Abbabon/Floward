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
    public bool SailsAttached;

    public delegate void SailsChange();
    public event SailsChange OnSailsChange;

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
        SailsAttached = true;
    }

    public void SetState(SailsState newState)
    {
        if (SailsAttached && state != newState)
        {
            state = newState;
            if (state == SailsState.SailsUp)
            {
                if (WindController.Instance.Strength() > 0 && WindController.Instance.Direction() == WindDirection.FrontWind)
                {
                    SoundManager.Instance.ChangeParameter("Sails + Flag", 0.6f);
                }
                if (WindController.Instance.Strength() > 0 && WindController.Instance.Direction() == WindDirection.BackWind)
                {
                    SoundManager.Instance.ChangeParameter("Sails + Flag", 0.4f);
                }
                else if (WindController.Instance.Strength() == 0)
                {
                    SoundManager.Instance.ChangeParameter("Sails + Flag", 0.3f);
                }

                if (WindController.Instance.Direction() == WindDirection.FrontWind && EngineController.Instance.IsWorking())
                {
                    FuelController.Instance.FuelDrop(GlobalGameplayVariables.Instance.FuelDropOpenSails, GlobalGameplayVariables.Instance.FuelDropOpenSailsHeatLoss);
                }

            }
            else if (state == SailsState.SailsDown)
            {
                SoundManager.Instance.ChangeParameter("Sails + Flag", 0.2f);

                //too hard:
                //if (WindManager.Instance.State == WindState.BackWind)
                //PressureController.Instance.ReleaseRandomValve();
            }
            sailsAnimator.SetBool("SailOpen", state == SailsState.SailsUp);
            OnSailsChange();
        }
    }

    private void Update()
    {
        if (State == SailsState.SailsUp && WindController.Instance.Strength() == 3 && WindController.Instance.Direction() == WindDirection.FrontWind){
            float newDurabilityValue = SailsDurability - GlobalGameplayVariables.Instance.DurabilityLossPerSecond * Time.deltaTime;
            SailsDurability = Mathf.Clamp(newDurabilityValue, 0f, 100f);
        }

        //TODO: somehow reflect this to the player!
        if (SailsDurability < 0.1f){
            TearSailOff();
        }
    }

    private void TearSailOff()
    {
        SailsAttached = false;
        //TODO: call animation
        Debug.Log("Sails Torn!");
    }
}
