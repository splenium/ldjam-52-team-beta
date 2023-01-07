using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseDeformation = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for(int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseDeformation += (v + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseDeformation = Mathf.Max(0, noiseDeformation - settings.minValue);
        return noiseDeformation * settings.strength;
    }
}
