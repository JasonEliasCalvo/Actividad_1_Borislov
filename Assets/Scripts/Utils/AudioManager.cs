using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    public AudioSource sfx, music, voice;

    public AudioClip interactSound;
    public AudioClip failInteractSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlayFailSound()
    {
        Debug.Log("Playing fail sound");
        PlaySound(failInteractSound);
    }

    public void PLayInteractSound()
    {
        Debug.Log("Playing interact sound");
        PlaySound(interactSound);
    }
    public void PlayLoopSound(AudioClip sound)
    {
        sfx.clip = sound;
        sfx.loop = true;
        sfx.Play();
    }

    public void StopLoopSound()
    {
        sfx.Stop();
        sfx.clip = null;
        sfx.loop = false;
    }
}
