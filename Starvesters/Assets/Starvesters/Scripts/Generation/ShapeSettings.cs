using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{

    public float planetSize = 1;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}