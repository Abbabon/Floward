using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextBox : MonoBehaviour
{
    [SerializeField] private TutorialPhase currentPhase;
    [SerializeField] private TextMeshProUGUI TMPUpper;
    [SerializeField] private TextMeshProUGUI TMPLower;

    public void SetTutorialMessage(TutorialPhase newMessage)
    {
        currentPhase = newMessage;
        //escape characters (for example \n) are serialized as actual characters (\\n) and that's why we need an excluded character. 
        TMPUpper.text = newMessage.UpperText.Replace('$', '\n');
        TMPLower.text = newMessage.LowerText.Replace('$', '\n');
    }    
}