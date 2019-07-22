using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    private static FlagController _instance;
    public static FlagController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private Ship_ctrl ship_Ctrl;

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
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update(){
        ship_Ctrl.wind = WindController.Instance.State;
    }
}
