using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    private static WorldStateManager _instance;
    public static WorldStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

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
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartWind()
    {

    }
}
