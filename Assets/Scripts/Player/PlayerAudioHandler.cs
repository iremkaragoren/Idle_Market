using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAudioHandler : MonoBehaviour
{
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private List<AudioClip> audioClips;
  
  public void PlayTakingSound()
  { 
      PlaySound(0); 
  }

  public void PlayDroppingSound()
  {
      PlaySound(1);
  }
  
  private void PlaySound(int clipIndex)
  {
    if (audioSource != null && audioClips != null && clipIndex >= 0 && clipIndex < audioClips.Count)
    {
      audioSource.PlayOneShot(audioClips[clipIndex]);
    }
  }
}
