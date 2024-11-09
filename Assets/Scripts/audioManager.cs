using UnityEngine;

using UnityEngine;

   public class audioManager : MonoBehaviour
   {
       [SerializeField] private AudioSource audioSource;   // Reference to the AudioSource component
       [SerializeField] private AudioClip[] audioClips;    // Array to hold different AudioClips

       void Start()
       {
           if (audioSource == null)
           {
               audioSource = GetComponent<AudioSource>();
           }
       }

       // Method to play a specific sound by index
       public void PlaySound(int index)
       {
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

       // Example method to play a random sound
       public void PlayRandomSound()
       {
           int randomIndex = Random.Range(0, audioClips.Length);
           PlaySound(randomIndex);
       }
   }

