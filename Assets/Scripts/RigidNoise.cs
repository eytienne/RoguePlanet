using System;
using UnityEngine;

[Serializable]
public class RigidNoise : INoise
{
    static readonly Simplex simplex = new Simplex();

    [Serializable]
    public class Settings : SimplexNoise.Settings
    {
        public float weightMultiplier;
    }
    public Settings settings;

    public RigidNoise() : this(null) {
    }

    public RigidNoise(Settings settings) {
        this.settings = settings;
    }

    public float GetElevation(Vector3 point) {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.nbOctaves; i++) {
            float v = 1 - Mathf.Abs(simplex.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }

}