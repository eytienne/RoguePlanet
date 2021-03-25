using System;

[Serializable]
public class NoiseLayer
{
    public enum Type
    {
        SIMPLEX,
        RIGID
    };
    public Type type;

    public bool enabled = true;
    public bool useFirstLayerAsMask = false;

    [ConditionalHide("type", (int)Type.SIMPLEX)]
    public SimplexNoise.Settings simplexNoiseSettings;
    [ConditionalHide("type", (int)Type.RIGID)]
    public RigidNoise.Settings rigidNoiseSettings;

    public INoise GetNoise() {
        switch (type) {
            case Type.SIMPLEX:
                return new SimplexNoise(simplexNoiseSettings);
            case Type.RIGID:
                return new RigidNoise(rigidNoiseSettings);
            default:
                return null;
        }
    }
}