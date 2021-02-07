using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static SessionManager _instance;
    public static SessionManager Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public bool FinishedOneRun;

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

            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
