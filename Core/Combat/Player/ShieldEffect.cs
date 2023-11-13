using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    
    void TriggerShieldParticle()
    {
        foreach (Transform item in transform)
        {
            var com = item.GetComponent<ParticleSystem>();
            if (com)
            {
                com.Play();
            }
        }
    }
    void fixScale(bool onOff)
    {
        if (onOff)
        {
            GetComponent<ParticleSystem>().Play();
        }
        else
        {
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
