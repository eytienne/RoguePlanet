using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioParameters : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetAudio(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
}

