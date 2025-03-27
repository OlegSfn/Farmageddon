using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Manages all audio in the game including music, ambience, and sound effects
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Static singleton instance accessible throughout the game
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        /// Reference to the audio data asset containing all audio clips and settings
        /// </summary>
        [SerializeField] private AudioData audioData;
        
        /// <summary>
        /// Audio source dedicated to background music
        /// </summary>
        [SerializeField] private AudioSource musicSource;
        
        /// <summary>
        /// Audio source dedicated to ambient sounds
        /// </summary>
        [SerializeField] private AudioSource ambienceSource;
        
        /// <summary>
        /// Number of audio sources to create for sound effects
        /// </summary>
        [SerializeField] private int sfxSourcesCount = 10;
        
        /// <summary>
        /// Pool of audio sources for playing sound effects
        /// </summary>
        private List<AudioSource> _sfxSources = new();
        
        /// <summary>
        /// Dictionary of currently playing looping sounds
        /// </summary>
        private Dictionary<string, AudioSource> _loopingSounds = new();
        
        /// <summary>
        /// Name of the currently playing music group
        /// </summary>
        private string _currentMusicGroup = string.Empty;

        /// <summary>
        /// Initializes the singleton instance and audio sources
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                
                // Subscribe to scene loading to change music
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Unsubscribe from events when the audio manager is destroyed
        /// </summary>
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Creates and configures all audio sources needed for the game
        /// </summary>
        private void InitializeAudioSources()
        {
            // Create pool of sound effect sources
            for (int i = 0; i < sfxSourcesCount; i++)
            {
                GameObject sfxSource = new GameObject($"SFX_Source_{i}");
                sfxSource.transform.SetParent(transform);
                
                AudioSource source = sfxSource.AddComponent<AudioSource>();
                source.playOnAwake = false;
                
                _sfxSources.Add(source);
            }

            // Create music source if not assigned in inspector
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("Music_Source");
                musicObj.transform.SetParent(transform);
                
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Create ambience source if not assigned in inspector
            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("Ambience_Source");
                ambienceObj.transform.SetParent(transform);
                
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// Event handler for when a new scene is loaded
        /// Automatically plays the appropriate music for the scene
        /// </summary>
        /// <param name="scene">The scene that was loaded</param>
        /// <param name="mode">The scene loading mode</param>
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

        /// <summary>
        /// Plays background music from the specified audio group
        /// </summary>
        /// <param name="groupName">Name of the audio group containing music clips</param>
        public void PlayMusic(string groupName)
        {
            // Don't restart if the same music is already playing
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

        /// <summary>
        /// Stops the currently playing music
        /// </summary>
        public void StopMusic()
        {
            _currentMusicGroup = string.Empty;
            musicSource.Stop();
        }

        /// <summary>
        /// Pauses the currently playing music without stopping it
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// Resumes previously paused music
        /// </summary>
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.Play();
            }
        }

        /// <summary>
        /// Plays a one-shot sound effect from the specified audio group
        /// </summary>
        /// <param name="groupName">Name of the audio group to play from</param>
        /// <param name="position">Optional 3D position for spatial audio</param>
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

        /// <summary>
        /// Plays a looping sound effect that continues until explicitly stopped
        /// </summary>
        /// <param name="groupName">Name of the audio group to play from</param>
        /// <param name="position">Optional 3D position for spatial audio</param>
        public void PlayLoopingSound(string groupName, Vector3? position = null)
        {
            // Don't start if already playing
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

        /// <summary>
        /// Stops a specific looping sound by group name
        /// </summary>
        /// <param name="groupName">Name of the audio group to stop</param>
        public void StopLoopingSound(string groupName)
        {
            if (_loopingSounds.TryGetValue(groupName, out AudioSource source))
            {
                source.Stop();
                _loopingSounds.Remove(groupName);
            }
        }

        /// <summary>
        /// Stops all currently playing looping sounds
        /// </summary>
        public void StopAllLoopingSounds()
        {
            foreach (var source in _loopingSounds.Values)
            {
                source.Stop();
            }
            _loopingSounds.Clear();
        }

        /// <summary>
        /// Gets an available audio source from the pool for playing sound effects
        /// </summary>
        /// <returns>An audio source that isn't currently playing, or the oldest source if all are in use</returns>
        private AudioSource GetAvailableSfxSource()
        {
            foreach (var source in _sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // If all sources are playing, return the oldest one
            // This will interrupt the oldest sound to play the new one
            return _sfxSources[0];
        }

        /// <summary>
        /// Plays the footstep sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlayFootstepSound(Vector3 position)
        {
            PlaySound("Footstep", position);
        }

        /// <summary>
        /// Plays the player hurt sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlayPlayerHurtSound(Vector3 position)
        {
            PlaySound("PlayerHurt", position);
        }

        /// <summary>
        /// Plays the sword swing sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlaySwordSwingSound(Vector3 position)
        {
            PlaySound("SwordSwing", position);
        }

        /// <summary>
        /// Plays the watering can sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlayWateringCanSound(Vector3 position)
        {
            PlaySound("WateringCan", position);
        }

        /// <summary>
        /// Plays the filling watering can sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlayFillWateringCanSound(Vector3 position)
        {
            PlaySound("FillWateringCan", position);
        }

        /// <summary>
        /// Plays the dirt plugging/digging sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlayPluggingDirtSound(Vector3 position)
        {
            PlaySound("PluggingDirt", position);
        }

        /// <summary>
        /// Plays the slime jump sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlaySlimeJumpSound(Vector3 position)
        {
            PlaySound("SlimeJump", position);
        }

        /// <summary>
        /// Plays the slime attack sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlaySlimeAttackSound(Vector3 position)
        {
            PlaySound("SlimeAttack", position);
        }

        /// <summary>
        /// Plays the slime death sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlaySlimeDeathSound(Vector3 position)
        {
            PlaySound("SlimeDeath", position);
        }

        /// <summary>
        /// Plays the slime taking damage sound at the specified position
        /// </summary>
        /// <param name="position">3D position where the sound should originate</param>
        public void PlaySlimeTakingDamageSound(Vector3 position)
        {
            PlaySound("SlimeTakingDamage", position);
        }

        /// <summary>
        /// Starts playing the rain ambient sound
        /// </summary>
        public void StartRainSound()
        {
            PlayLoopingSound("Raining");
        }

        /// <summary>
        /// Stops the rain ambient sound
        /// </summary>
        public void StopRainSound()
        {
            StopLoopingSound("Raining");
        }
    }
}
