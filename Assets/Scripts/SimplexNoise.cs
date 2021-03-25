using System;
using UnityEngine;

[Serializable]
public class SimplexNoise : INoise
{
    static readonly Simplex simplex = new Simplex();

    [Serializable]
    public class Settings : ISettings
    {
        [Range(0, 2)]
        public float minValue;
        [Range(0, 2)]
        public float strength;
        [Range(0, 10)]
        public int nbOctaves;
        [Range(0, 5)]
        public float baseRoughness;
        [Range(0, 5)]
        public float roughness;
        [Range(0, 5)]
        public float persistence;
        public Vector3 centre;
    };
    public Settings settings;

    public SimplexNoise() : this(null) {
    }

    public SimplexNoise(Settings settings) {
        this.settings = settings;
    }

    public float GetElevation(Vector3 point) {
        float elevation = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.nbOctaves; i++) {
            float v = simplex.Evaluate(point * frequency + settings.centre);
            elevation += (v + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        elevation = Mathf.Max(0, elevation - settings.minValue);
        return elevation * settings.strength;
    }
}