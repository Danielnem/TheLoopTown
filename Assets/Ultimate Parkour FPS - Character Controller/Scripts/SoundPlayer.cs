using UnityEngine;

/*
    Script to handle playing sound effects.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        public bool isPlaying { get { return audioSource.isPlaying; } } // if a sound is currently being played

        public AudioClip clip { get { return audioSource.clip; } } // current audio clip being played

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // play a selected audio clip with parameters
        public void PlaySound(AudioClip clip, bool loop = false, float volume = 1, float pitch = 1)
        {
            // set parameters
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;

            // play audio clip
            if (loop)
                audioSource.Play();
            else
                audioSource.PlayOneShot(clip);
        }

        // stop playing the current audio clip
        public void Stop() => audioSource.Stop();
    }
}