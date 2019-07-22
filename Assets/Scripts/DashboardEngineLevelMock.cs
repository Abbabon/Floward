using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// the particle system is based on this Youtube video - https://www.youtube.com/watch?v=wjtNGV9GNrw&app=desktop
// abd this article - https://docs.unity3d.com/ScriptReference/ParticleSystem-sizeOverLifetime.html
public class DashboardEngineLevelMock : MonoBehaviour {

    [SerializeField] private ParticleSystem masterParticleSystem;
    [SerializeField] private ParticleSystem scalingParticleSystem;

    [SerializeField] private float minScalingPSSize = 0.2f;
    [SerializeField] private float maxScalingPSSize = 1.2f;
    private float unit;

    private void Awake()
    {
        unit = ((maxScalingPSSize - minScalingPSSize) / 100f);
    }

    // level is between 0 and 100
    public void Set(float level) {
        if (level > 0)
        {
            if (!masterParticleSystem.isPlaying)
                masterParticleSystem.Play();

            float currentSize = level * unit;

            var sz = scalingParticleSystem.sizeOverLifetime;
            sz.size = new ParticleSystem.MinMaxCurve(currentSize);
        }
        else
        {
            if (masterParticleSystem.isPlaying)
                masterParticleSystem.Stop();
        }
    }
}
