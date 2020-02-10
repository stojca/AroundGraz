using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheelskid : MonoBehaviour
{

    [SerializeField] float intensityModifer = 1.5f;
    Skidmarks skidMarkController;
    PlayerCar playerCar;

    ParticleSystem particleSystem;

    int lastSkidId = -1;

    // Start is called before the first frame update
    void Start()
    {
        skidMarkController = FindObjectOfType<Skidmarks>();
        playerCar = GetComponentInParent<PlayerCar>();
        particleSystem = GetComponentInParent<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float intensity = playerCar.SlideSlipAmount();
        if (intensity < 0)
            intensity = -intensity;

        if(intensity > 0.2f)
        {
            lastSkidId = skidMarkController.AddSkidMark(transform.position, transform.up, intensity * intensityModifer, lastSkidId);
            if(particleSystem != null && !particleSystem.isPlaying)
            {
            	particleSystem.Play();
            }

        }
        else
        {
            lastSkidId = -1;
            if(particleSystem != null && particleSystem.isPlaying)
            {
            	particleSystem.Stop();
            }
        }
    }
}
