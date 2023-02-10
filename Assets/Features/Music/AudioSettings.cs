using System;
using DataStructures.Variables;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Features.Audio_Namespace.Logic
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private AudioMixer volumeMixer;
        [SerializeField] private FloatVariable volume;
        [SerializeField] private Slider musicSlider;

        private bool _valueIsChangeable;
    
        private void Awake()
        {
            if (PlayerPrefs.HasKey("Volume"))
            {
                volume.Set(PlayerPrefs.GetFloat("Volume"));
            }
            
            _valueIsChangeable = false;
            
            musicSlider.onValueChanged.AddListener(SetVolume);
        }

        private void Start()
        {
            UpdateVolume();
        }

        private void OnDestroy()
        {
            musicSlider.onValueChanged.RemoveListener(SetVolume);
        }

        private void OnEnable()
        {
            _valueIsChangeable = true;
            UpdateVolume();
        }

        private void OnDisable()
        {
            _valueIsChangeable = false;
        }

        private void UpdateVolume()
        {
            //use the float of the scriptable object to override the current volume
            float dbMusic = volume.Get() != 0 ? Mathf.Log10(volume.Get()) * 20 : -80f;
        
            volumeMixer.SetFloat("Vol", dbMusic);
        
            musicSlider.value = volume.Get();
            Debug.Log(musicSlider.value);
        }

        private void SetVolume(float value)
        {
            if (!_valueIsChangeable)
            {
                return;
            }
        
            volume.Set(value);
        
            //use the float of the slider to override the current volume
            float dbMusic = volume.Get() != 0 ? Mathf.Log10(volume.Get()) * 20 : -80f;
            volumeMixer.SetFloat("Vol", dbMusic);
        
            PlayerPrefs.SetFloat("Volume", volume.Get());
        }
    }
}
