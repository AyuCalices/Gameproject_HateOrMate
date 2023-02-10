using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBehaviour : MonoBehaviour
{
    [SerializeField] private List<AudioSource> soundTracks;
    [SerializeField] private float musicFadeTime;

    public float MusicFadeTime => musicFadeTime;
    
    private int _currentTrackIndex;
    
    public void Disable()
    {
        Debug.Log("Disable");
        StartCoroutine(FadeOutTrack(soundTracks[_currentTrackIndex], 0, musicFadeTime));
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
        StartCoroutine(PlayNextTrack());
        StartCoroutine(FadeInTrack(soundTracks[_currentTrackIndex], soundTracks[_currentTrackIndex].volume, musicFadeTime));
    }

    private void Awake()
    {
        _currentTrackIndex = Random.Range(0, soundTracks.Count);
    }

    private IEnumerator PlayNextTrack()
    {
        while (true)
        {
            if (_currentTrackIndex >= soundTracks.Count - 1)
            {
                _currentTrackIndex = 0;
            }
            else
            {
                _currentTrackIndex++;
            }
            soundTracks[_currentTrackIndex].Play();
            yield return new WaitForSeconds(soundTracks[_currentTrackIndex].clip.length);
        }
    }
    
    private IEnumerator FadeOutTrack(AudioSource audioSource, float toVal, float duration)
    {
        float counter = 0f;
        float startVolume = audioSource.volume;

        while (counter < duration)
        {
            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;
            
            audioSource.volume = Mathf.Lerp(startVolume, toVal, counter / duration);
            Debug.Log(audioSource.volume);
            yield return null;
        }
        
        gameObject.SetActive(false);
        audioSource.volume = startVolume;
        StopAllCoroutines();
    }
    
    private IEnumerator FadeInTrack(AudioSource audioSource, float toVal, float duration)
    {
        audioSource.volume = 0f;
            
        float counter = 0f;
        float startVolume = audioSource.volume;

        while (counter < duration)
        {
            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;
            
            audioSource.volume = Mathf.Lerp(startVolume, toVal, counter / duration);
            yield return null;
        }
    }
}
