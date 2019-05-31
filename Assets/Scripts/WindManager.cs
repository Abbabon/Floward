using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add a no-wind condition?
public enum WindState
{
    BackWind=0,
    FrontWind=1,
}

public class WindManager : MonoBehaviour
{
    private static WindManager _instance;
    public static WindManager Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public WindState State;

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
                State = WindState.BackWind;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }


    public void SetState(WindState newState)
    {
        if (State != newState)
        {
            State = newState;
            if (State == WindState.BackWind)
            {
                //TODO: animate, sound?
            }
            else if (State == WindState.FrontWind)
            {
                //TODO: animate, sound?
            }
        }
    }

}
