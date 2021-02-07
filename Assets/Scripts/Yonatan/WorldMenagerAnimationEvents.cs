using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldMenagerAnimationEvents : MonoBehaviour
{
    [Range(0, 0.1f)]
    public float fogDensitySlider = 0.023f;


    //for hilights fix
    [ColorUsageAttribute(true, true)]
    public Color highlight01;

    [ColorUsageAttribute(true, true)]
    public Color highlight02; 

    public Renderer highlight;
    public Renderer flagHighlight;

    
    void Update()
    {
        RenderSettings.fogDensity = fogDensitySlider;
    }

    private void Awake()
    {
        //set highlights to first color for start scene
        highlight.sharedMaterial.SetColor("_EMISSION", highlight01);

        flagHighlight.sharedMaterial.SetColor("_EMISSION", highlight01);
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

    void ChangeHighlights() // fix highlights in post process change
    {
        highlight.sharedMaterial.SetColor("_EMISSION", highlight02);

        flagHighlight.sharedMaterial.SetColor("_EMISSION", highlight02);
    }
}
