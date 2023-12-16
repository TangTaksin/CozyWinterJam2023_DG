using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FireLight : MonoBehaviour
{
    Light fire_light;
    public float intensitySpeed = 1.0f;
    public float minIntensityRange = 14f;
    public float maxIntensityRange = 22f;
    // Start is called before the first frame update
    void Start()
    {
        fire_light = GetComponent<Light>();

    }

    // Update is called once per frame
    void Update()
    {
        // Use Mathf.Sin to create a smooth oscillation between -1 and 1
        float intensityValue = Mathf.Sin(Time.time * intensitySpeed);

        // Map the intensityValue to the desired range and use SmoothStep for smooth interpolation
        float smoothIntensity = Mathf.SmoothStep(0f, 1f, (intensityValue + 1f) * 0.5f);

        // Map the smoothIntensity to the desired range and clamp it
        float clampedIntensity = Mathf.Clamp(smoothIntensity, 0f, 1f);

        // Map the clampedIntensity to the desired intensity range
        fire_light.intensity = Mathf.Lerp(minIntensityRange, maxIntensityRange, clampedIntensity);

    }
}
