using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyParticles : MonoBehaviour
{
    public ParticleSystem psSource;
    private ParticleSystem ps;

    ParticleSystem.MainModule main_;

    void Start()
    {
       
            ps = GetComponent<ParticleSystem>();
            main_ = ps.main;
                  
    }

    // Update is called once per frame
    void Update()
    {
        if (psSource != null)
        {
            main_.startLifetime = psSource.main.startLifetime;
        }
        if (main_.startLifetime.constant < 0.0005f)
        {
            ps.Stop();
        }
        else
        {
            ps.Play();
        }

    }

    
}
