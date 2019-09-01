﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;

public class PlantsController : SerializedMonoBehaviour
{
    private static PlantsController _instance;
    public static PlantsController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    [SerializeField] private List<Plant> _plants;

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
    }

    public void CollectPlant(){
        int uncollected = _plants.Count(p => p.Collected);
        if (uncollected > 0){
            Plant plant = _plants.Where(p => p.Collected).ToArray()[UnityEngine.Random.Range(0, uncollected)];
            plant.Collected = true;
            plant.gameObject.SetActive(true);
        }
    }

    public void ResetState(){
        _plants.ForEach(plant => plant.Collected = false);
    }

    public void Serialize(){
        _plants.ForEach(plant => PlayerPrefs.SetInt(String.Format("Plant{0}", plant.PlantId), plant.Collected ? 1 : 0));
    }

}