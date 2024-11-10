using UnityEngine;
using System;
using UnityEngine;

   public class AudioManager : MonoBehaviour
   {
        private int volumePercent = 1;
        private bool varee = true;
       [SerializeField] private AudioSource audioSource;   // Reference to the AudioSource component
       [SerializeField] private AudioClip[] audioClips;    // Array to hold different AudioClips
       private System.Random random;

       void Start()
       {
            random = new System.Random();
           if (audioSource == null)
           {
               audioSource = GetComponent<AudioSource>();
           }
       }

       // Method to play a specific sound by index
       public void PlayVareedSound(int index)
       {
            audioSource.volume = 1;
           if (index >= 0 && index < audioClips.Length)
           {
                if (varee)
                {
                    audioSource.pitch = 1;
                    float offset = (float)random.NextDouble()/8;
                    offset *= random.Next(2) * 2 - 1;
                    audioSource.pitch += offset;
                }
                audioSource.volume = 100 / volumePercent;
                audioSource.clip = audioClips[index];
                audioSource.Play();
           }
           else
           {
               Debug.LogWarning("Index out of range. Cannot play sound.");
           }
       }

        public void PlaySound(int index)
        {
            audioSource.volume = 1;
            if (index >= 0 && index < audioClips.Length)
            {
                audioSource.clip = audioClips[index];
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Index out of range. Cannot play sound.");
            }
        }
   }

