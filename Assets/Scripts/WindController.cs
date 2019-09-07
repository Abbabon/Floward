using System;
using System.Collections;
using System.Collections.Generic;
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

    public int State; // runs between -2 and 2

    public delegate void WindChange();
    public event WindChange OnWindChange;

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
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    public void ChangeState(int amount)
    {
        int newWind = State + amount;
        newWind = Math.Min(Math.Max(newWind, -2), 2);

        State = newWind;
        SoundManager.Instance.ChangeParameter("Sails + Flag", 0.1f);

        //Wind changed direction
        if (newWind * State <= 0){
            if (SailsController.Instance.State == SailsState.SailsUp)
            {
                if (Direction() == WindDirection.BackWind)
                {
                    SoundManager.Instance.ChangeParameter("Sails + Flag", 0.4f);
                }
                if (Direction() == WindDirection.BackWind)
                {
                    SoundManager.Instance.ChangeParameter("Sails + Flag", 0.6f);
                }
            }
        }

        switch (Strength())
        {
            case 0:
                SoundManager.Instance.ChangeParameter("Wind Level", 0);
                break;
            case 1:
                SoundManager.Instance.ChangeParameter("Wind Level", 0.5f);
                break;
            case 2:
                SoundManager.Instance.ChangeParameter("Wind Level", 1);
                break;
            default:
                break;
        }
        OnWindChange();
    }
}
