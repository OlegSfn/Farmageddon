using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    /// <summary>
    /// Manages the game settings functionality and persistence
    /// Handles audio, graphics, and display settings
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SettingsManager Instance { get; private set; }
        
        /// <summary>
        /// Audio mixer for controlling game volume settings
        /// </summary>
        [SerializeField] private AudioMixer audioMixer;

        /// <summary>
        /// Path to settings file, relative to persistent data path
        /// </summary>
        private const string SettingsFileName = "game_settings.json";

        /// <summary>
        /// Settings data container for serialization
        /// </summary>
        [Serializable]
        public class SettingsData
        {
            public float masterVolume;
            public float musicVolume;
            public float sfxVolume;
            public float uiVolume;
            public int qualityLevel;
            public int resolutionIndex;
            public bool isFullscreen;
            public bool isVSync;
        }

        /// <summary>
        /// Current settings instance
        /// </summary>

        public SettingsData CurrentSettings { get; private set; } = new();

        /// <summary>
        /// Initializes the settings manager and loads saved settings
        /// </summary>
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                InitializeSettings();
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initialize settings with current values
        /// </summary>
        private void InitializeSettings()
        {
            CurrentSettings = new SettingsData();
            
            audioMixer.GetFloat("MasterVolume", out float masterVolume);
            audioMixer.GetFloat("MusicVolume", out float musicVolume);
            audioMixer.GetFloat("SfxVolume", out float sfxVolume);
            audioMixer.GetFloat("UIVolume", out float uiVolume);
            
            CurrentSettings.masterVolume = masterVolume;
            CurrentSettings.musicVolume = musicVolume;
            CurrentSettings.sfxVolume = sfxVolume;
            CurrentSettings.uiVolume = uiVolume;
            CurrentSettings.qualityLevel = QualitySettings.GetQualityLevel();
            CurrentSettings.isFullscreen = Screen.fullScreen;
            CurrentSettings.isVSync = QualitySettings.vSyncCount > 0;
            
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Screen.resolutions[i].width == Screen.currentResolution.width && 
                    Screen.resolutions[i].height == Screen.currentResolution.height)
                {
                    CurrentSettings.resolutionIndex = i;
                    break;
                }
            }
        }
        
        /// <summary>
        /// Sets the master volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level (typically between -80 and 0)</param>
        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
            CurrentSettings.masterVolume = volume;
            SaveSettings();
        }
        
        /// <summary>
        /// Sets the music volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level</param>
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
            CurrentSettings.musicVolume = volume;
            SaveSettings();
        }
        
        /// <summary>
        /// Sets the sound effects volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level</param>
        public void SetSfxVolume(float volume)
        {
            audioMixer.SetFloat("SfxVolume", volume);
            CurrentSettings.sfxVolume = volume;
            SaveSettings();
        }
        
        /// <summary>
        /// Sets the UI sound effects volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level</param>
        public void SetUIVolume(float volume)
        {
            audioMixer.SetFloat("UIVolume", volume);
            CurrentSettings.uiVolume = volume;
            SaveSettings();
        }

        /// <summary>
        /// Sets the graphics quality level
        /// </summary>
        /// <param name="qualityIndex">Index of quality preset to use</param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            CurrentSettings.qualityLevel = qualityIndex;
            SaveSettings();
        }

        /// <summary>
        /// Apply fullscreen settings
        /// </summary>
        /// <param name="isFullscreen">Whether fullscreen should be enabled</param>
        public void ApplyFullscreenSettings(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            CurrentSettings.isFullscreen = isFullscreen;
            SaveSettings();
        }

        /// <summary>
        /// Apply resolution settings
        /// </summary>
        /// <param name="resolutionIndex">Index of the resolution in Screen.resolutions array</param>
        public void ApplyResolutionSettings(int resolutionIndex)
        {
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            CurrentSettings.resolutionIndex = resolutionIndex;
            SaveSettings();
        }

        /// <summary>
        /// Toggles vertical sync
        /// </summary>
        /// <param name="isVSync">Whether VSync should be enabled</param>
        public void SetVSync(bool isVSync)
        {
            QualitySettings.vSyncCount = isVSync ? 1 : 0;
            CurrentSettings.isVSync = isVSync;
            SaveSettings();
        }

        /// <summary>
        /// Saves current settings to JSON file
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                string settingsJson = JsonUtility.ToJson(CurrentSettings, true);
                string filePath = Path.Combine(Application.persistentDataPath, SettingsFileName);
                File.WriteAllText(filePath, settingsJson);
            }
            catch (Exception)
            {
                Debug.LogError("Failed to save settings.");
            }
        }

        /// <summary>
        /// Loads settings from JSON file and applies them
        /// </summary>
        private void LoadSettings()
        {
            string filePath = Path.Combine(Application.persistentDataPath, SettingsFileName);
            
            if (File.Exists(filePath))
            {
                try
                {
                    string settingsJson = File.ReadAllText(filePath);
                    CurrentSettings = JsonUtility.FromJson<SettingsData>(settingsJson);

                    ApplyLoadedSettings();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load settings: {e.Message}");
                }
            }
            else
            {
                Debug.Log("Default settings");
                // Using default settings if file doesn't exist and creating a new settings file.
                SaveSettings();
            }
        }

        /// <summary>
        /// Applies loaded settings to the game
        /// </summary>
        private void ApplyLoadedSettings()
        {
            audioMixer.SetFloat("MasterVolume", CurrentSettings.masterVolume);
            audioMixer.SetFloat("MusicVolume", CurrentSettings.musicVolume);
            audioMixer.SetFloat("SfxVolume", CurrentSettings.sfxVolume);
            audioMixer.SetFloat("UIVolume", CurrentSettings.uiVolume);
            
            QualitySettings.SetQualityLevel(CurrentSettings.qualityLevel);
            
            QualitySettings.vSyncCount = CurrentSettings.isVSync ? 1 : 0;
            
            if (CurrentSettings.resolutionIndex >= 0 && CurrentSettings.resolutionIndex < Screen.resolutions.Length)
            {
                Resolution resolution = Screen.resolutions[CurrentSettings.resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, CurrentSettings.isFullscreen);
            }
            
            Screen.fullScreen = CurrentSettings.isFullscreen;
            Screen.fullScreenMode = CurrentSettings.isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }
    }
}