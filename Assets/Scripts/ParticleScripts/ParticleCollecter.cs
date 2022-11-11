using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ParticleCollecter : MonoBehaviour {

    ParticleSystem ps;
    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();


    private void Start() {

        ps = GetComponent<ParticleSystem>();
    }


    private void OnParticleTrigger() {

        int triggeredParticles = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for (int i = 0; i < triggeredParticles; i++) {

            ParticleSystem.Particle p = particles[i];
            p.remainingLifetime = 0;
            Debug.Log("Collected 1 Particle");
            particles[i] = p;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }
}
