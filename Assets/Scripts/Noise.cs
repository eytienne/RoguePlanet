using UnityEngine;

[System.Serializable]
public struct NoiseOctave
{
    [Range(0, 2)]
    public float depth;
    [Range(0, 20)]
    public float frequency;
    [Range(0, 10)]
    public float offset;

    public NoiseOctave(float depth = 1, float frequency = 1, float offset = 0) {
        this.depth = depth;
        this.frequency = frequency;
        this.offset = offset;

    }
}

[System.Serializable]
public class Noise
{
    public delegate float NoiseFunction(Vector3 v);

    public bool maskAgainstFirstOctave = false;
    [Range(0, 2)]
    public float min = 0;

    public NoiseOctave[] noiseOctaves;

    Simplex simplex = new Simplex();

    public Noise() { }

    public Noise(NoiseOctave baseLayer, int numLayers, float depthDivider, float frequencyIncrease, float offsetIncrease) {
        noiseOctaves = new NoiseOctave[numLayers];
        noiseOctaves[0] = baseLayer;
        for (int i = 1; i < numLayers; i++) {
            NoiseOctave previous = noiseOctaves[i - 1];
            NoiseOctave current = new NoiseOctave(
                previous.depth / depthDivider,
                previous.frequency + frequencyIncrease,
                previous.offset + offsetIncrease
            );
            noiseOctaves[i] = current;
        }
    }

    public float GetElevation(Vector3 v) {
        float elevation = 0;

        if (noiseOctaves != null) {
            float maxElevation = maskAgainstFirstOctave ? min : Mathf.Infinity;
            for (int i = 0; i < noiseOctaves.Length; i++) {
                NoiseOctave no = noiseOctaves[i];
                float currentElevation = Mathf.Max(no.depth * (simplex.Evaluate(no.frequency * 0.5f * v + no.offset * Vector3.one) + 1) / 2, min);
                if (maskAgainstFirstOctave) {
                    if (i == 0) {
                        maxElevation = Mathf.Max(currentElevation, maxElevation);
                        elevation += currentElevation;
                    } else {
                        elevation += (elevation - min) * currentElevation;
                    }
                } else {
                    elevation += currentElevation;
                }
            }
        }

        return elevation;
    }
}