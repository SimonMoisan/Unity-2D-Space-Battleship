using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsGenerator : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips; //0 is the default sound effects
    public bool isLooping;

    public void playDefaultSoundEffect()
    {
        if(audioClips != null && audioClips.Length > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();

            if(isLooping)
            {
                audioSource.loop = true;
            }
        }
    }

    public void playRandomSoundEffect()
    {
        if (audioClips != null && audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[randomIndex];
            audioSource.Play();
        }
    }

    public void stopSoundEffect()
    {
        audioSource.Stop();
    }
}
