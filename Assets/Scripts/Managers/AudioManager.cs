using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioData audioData;
        
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private int sfxSourcesCount = 10;
        
        private List<AudioSource> _sfxSources = new();
        private Dictionary<string, AudioSource> _loopingSounds = new();
        
        private string _currentMusicGroup = string.Empty;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void InitializeAudioSources()
        {
            for (int i = 0; i < sfxSourcesCount; i++)
            {
                GameObject sfxSource = new GameObject($"SFX_Source_{i}");
                sfxSource.transform.SetParent(transform);
                
                AudioSource source = sfxSource.AddComponent<AudioSource>();
                source.playOnAwake = false;
                
                _sfxSources.Add(source);
            }

            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("Music_Source");
                musicObj.transform.SetParent(transform);
                
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("Ambience_Source");
                ambienceObj.transform.SetParent(transform);
                
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Contains("MainMenu"))
            {
                PlayMusic("MainMenu");
            }
            else if (scene.name.Contains("Game"))
            {
                PlayMusic("Game");
            }
        }

        public void PlayMusic(string groupName)
        {
            if (_currentMusicGroup == groupName && musicSource.isPlaying)
            {
                return;
            }

            _currentMusicGroup = groupName;
            
            AudioData.AudioGroup group = audioData.GetAudioGroup(groupName);
            if (group == null || group.clips.Count == 0)
            {
                return;
            }
            musicSource.outputAudioMixerGroup = group.mixerGroup;

            AudioClip clip = audioData.GetRandomClip(groupName);
            if (clip == null)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = group.volume;
            musicSource.pitch = 1f;
            musicSource.spatialBlend = group.spatialBlend;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            _currentMusicGroup = string.Empty;
            musicSource.Stop();
        }

        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.Play();
            }
        }

        public void PlaySound(string groupName, Vector3? position = null)
        {
            AudioData.AudioGroup group = audioData.GetAudioGroup(groupName);
            if (group == null || group.clips.Count == 0)
            {
                return;
            }

            AudioClip clip = audioData.GetRandomClip(groupName);
            if (clip == null)
            {
                return;
            }

            AudioSource source = GetAvailableSfxSource();
            if (source == null)
            {
                return;
            }
            
            source.outputAudioMixerGroup = group.mixerGroup;
            source.clip = clip;
            source.volume = group.volume;
            source.pitch = Random.Range(group.minPitch, group.maxPitch);
            source.spatialBlend = group.spatialBlend;
            source.loop = false;

            if (position.HasValue)
            {
                source.transform.position = position.Value;
            }

            source.Play();
        }

        public void PlayLoopingSound(string groupName, Vector3? position = null)
        {
            if (_loopingSounds.ContainsKey(groupName))
            {
                return;
            }
            
            AudioData.AudioGroup group = audioData.GetAudioGroup(groupName);
            if (group == null || group.clips.Count == 0)
            {
                return;
            }

            AudioClip clip = audioData.GetRandomClip(groupName);
            if (clip == null)
            {
                return;
            }

            AudioSource source = GetAvailableSfxSource();
            if (source == null)
            {
                return;
            }

            source.outputAudioMixerGroup = group.mixerGroup;
            source.clip = clip;
            source.volume = group.volume;
            source.pitch = Random.Range(group.minPitch, group.maxPitch);
            source.spatialBlend = group.spatialBlend;
            source.loop = true;

            if (position.HasValue)
            {
                source.transform.position = position.Value;
            }

            source.Play();
            
            _loopingSounds[groupName] = source;
        }

        public void StopLoopingSound(string groupName)
        {
            if (_loopingSounds.TryGetValue(groupName, out AudioSource source))
            {
                source.Stop();
                _loopingSounds.Remove(groupName);
            }
        }

        public void StopAllLoopingSounds()
        {
            foreach (var source in _loopingSounds.Values)
            {
                source.Stop();
            }
            _loopingSounds.Clear();
        }

        private AudioSource GetAvailableSfxSource()
        {
            foreach (var source in _sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // If all sources are playing, return the old one.
            return _sfxSources[0];
        }


        public void PlayFootstepSound(Vector3 position)
        {
            PlaySound("Footstep", position);
        }

        public void PlayPlayerHurtSound(Vector3 position)
        {
            PlaySound("PlayerHurt", position);
        }

        public void PlaySwordSwingSound(Vector3 position)
        {
            PlaySound("SwordSwing", position);
        }

        public void PlayWateringCanSound(Vector3 position)
        {
            PlaySound("WateringCan", position);
        }

        public void PlayFillWateringCanSound(Vector3 position)
        {
            PlaySound("FillWateringCan", position);
        }

        public void PlayPluggingDirtSound(Vector3 position)
        {
            PlaySound("PluggingDirt", position);
        }

        public void PlaySlimeJumpSound(Vector3 position)
        {
            PlaySound("SlimeJump", position);
        }

        public void PlaySlimeAttackSound(Vector3 position)
        {
            PlaySound("SlimeAttack", position);
        }

        public void PlaySlimeDeathSound(Vector3 position)
        {
            PlaySound("SlimeDeath", position);
        }

        public void PlaySlimeTakingDamageSound(Vector3 position)
        {
            PlaySound("SlimeTakingDamage", position);
        }

        public void StartRainSound()
        {
            PlayLoopingSound("Raining");
        }

        public void StopRainSound()
        {
            StopLoopingSound("Raining");
        }

    }
}
