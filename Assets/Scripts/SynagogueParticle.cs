using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynagogueParticle : MonoBehaviour
{
    // Start is called before the first frame update
    private ParticleSystem myParticleSystem;


    void Start()
    {
        myParticleSystem = GetComponentInChildren<ParticleSystem>();
        myParticleSystem.Play();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Jew"))
        {
            if (myParticleSystem.isPlaying)
            {
                myParticleSystem.Stop();
            }

            myParticleSystem.Play();
        }
    }
}