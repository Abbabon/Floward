using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindDirection
{
    FrontWind=-1,
    BackWind=1
}

public class WindController : MonoBehaviour
{
    private static WindController _instance;
    public static WindController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public int State; // runs between -3 and 3

    public int Strength()
    {
        return Math.Abs(State);
    }

    public WindDirection Direction()
    {
        return (State < 0) ? WindDirection.FrontWind : WindDirection.BackWind;
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
        newWind = Math.Min(Math.Max(newWind, -3), 3);

        //Wind changed direction
        if (newWind * State < 0){
            SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.WindChange);
        }

        State = newWind;
    }
}
