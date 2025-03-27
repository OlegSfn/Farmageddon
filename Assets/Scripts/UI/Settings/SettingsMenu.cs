using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Settings
{
    /// <summary>
    /// Manages the game settings UI and functionality
    /// Handles audio, graphics, and display settings with confirmation system
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        /// <summary>
        /// Audio mixer for controlling game volume settings
        /// </summary>
        [SerializeField] private AudioMixer audioMixer;
        
        /// <summary>
        /// Dropdown for selecting screen resolution
        /// </summary>
        [SerializeField] private TMP_Dropdown resolutionsDropdown;
        
        /// <summary>
        /// Dropdown for selecting graphics quality level
        /// </summary>
        [SerializeField] private TMP_Dropdown qualityDropdown;
        
        /// <summary>
        /// Toggle for fullscreen mode
        /// </summary>
        [SerializeField] private Toggle fullscreenToggle;

        /// <summary>
        /// Reference to the confirmation dialog for applying settings changes
        /// </summary>
        [SerializeField] private ApplyChangesMenu applyChangesMenu;
        
        /// <summary>
        /// Stores the previous resolution to enable reverting changes
        /// </summary>
        private Resolution _previousResolution;
        
        /// <summary>
        /// Stores the previous fullscreen mode to enable reverting changes
        /// </summary>
        private FullScreenMode _previousFullScreenMode;
        
        /// <summary>
        /// Stores whether the game was in fullscreen before changes
        /// </summary>
        private bool _wasFullScreen;
        
        /// <summary>
        /// Sets the master volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level (typically between -80 and 0)</param>
        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }
        
        /// <summary>
        /// Sets the music volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level (typically between -80 and 0)</param>
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }
        
        /// <summary>
        /// Sets the sound effects volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level (typically between -80 and 0)</param>
        public void SetSfxVolume(float volume)
        {
            audioMixer.SetFloat("SfxVolume", volume);
        }
        
        /// <summary>
        /// Sets the UI sound effects volume level in the audio mixer
        /// </summary>
        /// <param name="volume">Volume level (typically between -80 and 0)</param>
        public void SetUIVolume(float volume)
        {
            audioMixer.SetFloat("UIVolume", volume);
        }

        /// <summary>
        /// Sets the graphics quality level
        /// </summary>
        /// <param name="qualityIndex">Index of quality preset to use</param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        /// <summary>
        /// Toggles fullscreen mode with confirmation dialog
        /// </summary>
        /// <param name="isFullscreen">Whether fullscreen should be enabled</param>
        public void SetFullscreen(bool isFullscreen)
        {
            _previousFullScreenMode = Screen.fullScreenMode;
            _wasFullScreen = Screen.fullScreen;

            Screen.fullScreen = isFullscreen;
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

            applyChangesMenu.Open(() => { }, RevertFullscreen);
        }

        /// <summary>
        /// Changes screen resolution with confirmation dialog
        /// </summary>
        /// <param name="resolutionIndex">Index of the resolution in Screen.resolutions array</param>
        public void SetResolution(int resolutionIndex)
        {
            _previousResolution = Screen.currentResolution;
            
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            applyChangesMenu.Open(() => { }, RevertResolution);
        }

        /// <summary>
        /// Toggles vertical sync
        /// </summary>
        /// <param name="isVSync">Whether VSync should be enabled</param>
        public void SetVSync(bool isVSync)
        {
            QualitySettings.vSyncCount = isVSync ? 1 : 0;
        }

        /// <summary>
        /// Updates the UI elements to reflect current system settings
        /// </summary>
        private void RefreshUIWithSettings()
        {
            UpdateResolutionDropdown();

            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
            
            fullscreenToggle.isOn = Screen.fullScreen;
        }
        
        /// <summary>
        /// Populates the resolution dropdown with available screen resolutions
        /// </summary>
        private void UpdateResolutionDropdown()
        {
            resolutionsDropdown.ClearOptions();
            
            for (int i = 0; i < Screen.resolutions.Length; ++i)
            {
                resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(Screen.resolutions[i].ToString()));
                
                if (Screen.resolutions[i].width == Screen.currentResolution.width &&
                    Screen.resolutions[i].height == Screen.currentResolution.height)
                {
                    resolutionsDropdown.value = i;
                }
            }

            resolutionsDropdown.RefreshShownValue();
        }
        
        /// <summary>
        /// Sets up UI event listeners when the menu is enabled
        /// </summary>
        private void OnEnable()
        {
            RefreshUIWithSettings();
            
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        /// <summary>
        /// Removes UI event listeners when the menu is disabled
        /// </summary>
        private void OnDisable()
        {
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }
        
        /// <summary>
        /// Reverts fullscreen settings to previous state
        /// </summary>
        private void RevertFullscreen()
        {
            Screen.fullScreen = _wasFullScreen;
            Screen.fullScreenMode = _previousFullScreenMode;
            
            // Update UI to match reverted settings without triggering events
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
            fullscreenToggle.isOn = _wasFullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        /// <summary>
        /// Reverts resolution settings to previous state
        /// </summary>
        private void RevertResolution()
        {
            Screen.SetResolution(_previousResolution.width, _previousResolution.height, Screen.fullScreen);
            
            // Update UI to match reverted resolution without triggering events
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            resolutionsDropdown.value = resolutionsDropdown.options.FindIndex(option =>
                option.text == _previousResolution.ToString());
            resolutionsDropdown.RefreshShownValue();
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
        }
    }
}
