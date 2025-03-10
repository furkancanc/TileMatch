using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource audioSource;

    [Header("Data")]
    [SerializeField] private AudioClip[] sfxList;
    [SerializeField] private AudioClip[] musicList;
    [SerializeField] private AudioClip menuMusic;

    public void PlaySfx(int index)
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(.9f, 1.1f);
        audioSource.clip = sfxList[index];
        audioSource.Play();
    }

    public void PlayRandomMusic()
    {
        musicSource.Stop();
        musicSource.clip = musicList[Random.Range(0, musicList.Length)];
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        musicSource.Stop();
        musicSource.clip = menuMusic;
        musicSource.Play();
    }
}
