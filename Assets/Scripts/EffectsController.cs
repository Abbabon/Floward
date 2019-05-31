using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    private static EffectsController _instance;
    public static EffectsController Instance { get { return _instance; } }
    private static readonly object padlock = new object();


    public Transform tornadoA;
    public Transform tornadoB;
    public ParticleSystem tornadoParticleSystem;
    public float tornadoMovementDuration = 30.0f; 

    private bool tornadoIsRunning;

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
                tornadoIsRunning = false;
                tornadoParticleSystem.Stop();
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {

    }

    public void RunTornado()
    {
        if (!tornadoIsRunning) {
            tornadoIsRunning = true;
            tornadoParticleSystem.transform.position = tornadoA.position;
            tornadoParticleSystem.Play();

            Sequence tornadoSequence = DOTween.Sequence();
            tornadoSequence.AppendInterval(5f);
            tornadoSequence.Append(tornadoParticleSystem.transform.DOMove(tornadoB.position, tornadoMovementDuration));
            tornadoSequence.AppendCallback(() => tornadoParticleSystem.Stop());
            tornadoSequence.AppendCallback(() => tornadoIsRunning = false);
        }
    }
}
