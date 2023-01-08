using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleParticlesMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public ParticleSystem ParticleSystem;
    void Start()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float factorSpeed = Mathf.Clamp01(_rigidbody.velocity.magnitude / 100.0f);
        ParticleSystem.startSpeed = Mathf.Lerp(5.0f, 15.0f, factorSpeed);
        ParticleSystem.startColor = new Color(ParticleSystem.startColor.r, ParticleSystem.startColor.g, ParticleSystem.startColor.b
            , factorSpeed); // We want these only at high speed
        //ParticleSystem.simulationSpace = (_rigidbody.velocity.magnitude < 1.0f ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local);
    }
}
