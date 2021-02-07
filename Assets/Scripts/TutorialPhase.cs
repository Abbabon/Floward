using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Message", menuName = "Floward Tutorial Message", order = 51)]
public class TutorialPhase : ScriptableObject
{
    [SerializeField] public int PhaseID;
    [SerializeField] public string UpperText;
    [SerializeField] public string LowerText;
    [SerializeField] public float SecondsToAppear;
}
