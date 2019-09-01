using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldMenagerAnimationEvents : MonoBehaviour
{
    [Range(0, 0.1f)]
    public float fogDensitySlider = 0.023f;

    void Update()
    {
        RenderSettings.fogDensity = fogDensitySlider;
    }

    private void Start(){
        HideShip();
    }

    void Startgame()
    {
        //animation event, start the game logic here
        GameManager.Instance.StartGame();
        ShowShip();
    }

    void HideShip(){
        MaterialsController.Instance.SetMaterialsBaseMapColor(Color.black);
    }

    void SetShipColor(Color color){
        MaterialsController.Instance.SetMaterialsBaseMapColor(color);
    }

    void ShowShip(){
        MaterialsController.Instance.TweenMaterialsBaseMapColor(Color.white, 1f);
    }
}
