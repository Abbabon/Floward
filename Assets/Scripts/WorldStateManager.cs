using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    private static WorldStateManager _instance;
    public static WorldStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private float windDirectionChangeTimer = 0f;
    [SerializeField] private double nextWindDirectionChange = 0f;

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

    private void Start()
    {
        windDirectionChangeTimer = 0;
        RandomizeNextWindChange();
    }

    private void Update()
    {
        if (GameManager.Instance.IsRunning)
        {
            if (TutorialController.Instance.EnableWindChanges)
                ChangeWindDirection();
        }
    }

    public void ChangeWindDirection()
    {
        windDirectionChangeTimer += Time.deltaTime;

        if (windDirectionChangeTimer >= nextWindDirectionChange)
        {
            int ChangeAmount;
            int ChangeVar = UnityEngine.Random.Range(0, 100);
            int DirectionOfChange = UnityEngine.Random.Range(0, 2);

            if (ChangeVar < 60){
                ChangeAmount = 1;
            }
            else if (ChangeVar < 90){
                ChangeAmount = 2;
            }
            else{
                ChangeAmount = 3;
            }

            int FinalChange = ChangeAmount * (DirectionOfChange == 0 ? 1 : -1);
            WindController.Instance.ChangeState(FinalChange);

            RandomizeNextWindChange();
            windDirectionChangeTimer = 0f;
        }
    }

    private void RandomizeNextWindChange()
    {
        nextWindDirectionChange = UnityEngine.Random.Range(GlobalGameplayVariables.Instance.WindChangeMin, GlobalGameplayVariables.Instance.WindChangeMax);
    }

    internal void SetTutorialMode()
    {
        WindController.Instance.ChangeState(0);
    }
}
