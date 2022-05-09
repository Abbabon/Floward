using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public enum WindDirection
{
    FrontWind=-1,
    NoWind = 0,
    BackWind =1
}

public class WindController : MonoBehaviour
{
    private static WindController _instance;
    public static WindController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public int State; // runs between -3 and 3

    public delegate void WindChange();
    public event WindChange OnWindChange;

    [EventRef(MigrateTo="<fieldname>")]
    public string _WindSoundEventName;
    FMOD.Studio.EventInstance _windSoundInstance;

    public int Strength()
    {
        return Math.Abs(State);
    }

    public WindDirection Direction()
    {
        if (State > 0){
            return WindDirection.BackWind;
        }
        else if (State == 0){
            return WindDirection.NoWind;
        }
        else{
            return WindDirection.FrontWind;
        }
    }

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
                State = 0;
                _windSoundInstance = FMODUnity.RuntimeManager.CreateInstance(_WindSoundEventName);
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _windSoundInstance.setParameterByName("Wind Level", 0);
        _windSoundInstance.start();
    }

    public void ChangeState(int amount)
    {
        int newWind = State + amount;
        newWind = Math.Min(Math.Max(newWind, -3), 3);

        //Wind changed direction
        if (newWind * State < 0){
            SoundManager.Instance.PlayOneshotound("Flag");
        }

        State = newWind;
        
        float soundState = Mathf.Abs(State);
        switch (soundState)
        {
            //case 0:
            //    _windSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //    break;
            case 0:
            case 1:
                _windSoundInstance.setParameterByName("Wind Level", 0);
                break;
            case 2:
                _windSoundInstance.setParameterByName("Wind Level", 0.5f);
                break;
            case 3:
                _windSoundInstance.setParameterByName("Wind Level", 1f);
                break;
            default:
                break;
        }

        OnWindChange();
    }
}
