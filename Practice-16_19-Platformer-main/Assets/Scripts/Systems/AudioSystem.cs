using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;

    [SerializeField] private AudioSource sfxSource;

    private void Awake() => Instance = this;

    public void PlayOneShot(string soundName)
    {
        // Реализация поиска звука по имени
        // AudioClip clip = GetClipByName(soundName);
        // if (clip) sfxSource.PlayOneShot(clip);
    }
}