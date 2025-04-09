using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels.Settings
{
    /// <summary>
    /// Manages the UI for game settings
    /// Handles user interface elements and events for settings configuration
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        /// <summary>
        /// Slider for adjusting master volume
        /// </summary>
        [SerializeField] private Slider masterVolumeSlider;
        
        /// <summary>
        /// Slider for adjusting music volume
        /// </summary>
        [SerializeField] private Slider musicVolumeSlider;
        
        /// <summary>
        /// Slider for adjusting sound effects volume
        /// </summary>
        [SerializeField] private Slider sfxVolumeSlider;
        
        /// <summary>
        /// Slider for adjusting UI sound effects volume
        /// </summary>
        [SerializeField] private Slider uiVolumeSlider;
        
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
        /// Toggle for vertical sync
        /// </summary>
        [SerializeField] private Toggle vSyncToggle;

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
        /// Reference to the settings manager
        /// </summary>
        private SettingsManager _settingsManager;
        
        /// <summary>
        /// Sets up UI event listeners when the menu is enabled
        /// </summary>
        private void OnEnable()
        {
            if (_settingsManager == null)
            {
                _settingsManager = SettingsManager.Instance;
                if (_settingsManager == null)
                {
                    Debug.LogError("SettingsManager not found!");
                    return;
                }
            }
            
            RefreshUIWithSettings();
            
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            masterVolumeSlider.onValueChanged.AddListener(_settingsManager.SetMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(_settingsManager.SetMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(_settingsManager.SetSfxVolume);
            uiVolumeSlider.onValueChanged.AddListener(_settingsManager.SetUIVolume);
            qualityDropdown.onValueChanged.AddListener(_settingsManager.SetQuality);
            vSyncToggle.onValueChanged.AddListener(_settingsManager.SetVSync);
        }

        /// <summary>
        /// Removes UI event listeners when the menu is disabled
        /// </summary>
        private void OnDisable()
        {
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
            masterVolumeSlider.onValueChanged.RemoveListener(_settingsManager.SetMasterVolume);
            musicVolumeSlider.onValueChanged.RemoveListener(_settingsManager.SetMusicVolume);
            sfxVolumeSlider.onValueChanged.RemoveListener(_settingsManager.SetSfxVolume);
            uiVolumeSlider.onValueChanged.RemoveListener(_settingsManager.SetUIVolume);
            qualityDropdown.onValueChanged.RemoveListener(_settingsManager.SetQuality);
            vSyncToggle.onValueChanged.RemoveListener(_settingsManager.SetVSync);
        }

        /// <summary>
        /// Toggles fullscreen mode with confirmation dialog
        /// </summary>
        /// <param name="isFullscreen">Whether fullscreen should be enabled</param>
        private void SetFullscreen(bool isFullscreen)
        {
            _previousFullScreenMode = Screen.fullScreenMode;
            _wasFullScreen = Screen.fullScreen;

            Screen.fullScreen = isFullscreen;
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

            applyChangesMenu.Open(() => 
            {
                _settingsManager.ApplyFullscreenSettings(isFullscreen);
            }, RevertFullscreen);
        }

        /// <summary>
        /// Changes screen resolution with confirmation dialog
        /// </summary>
        /// <param name="resolutionIndex">Index of the resolution in Screen.resolutions array</param>
        private void SetResolution(int resolutionIndex)
        {
            _previousResolution = Screen.currentResolution;
            
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            applyChangesMenu.Open(() => 
            {
                _settingsManager.ApplyResolutionSettings(resolutionIndex);
            }, RevertResolution);
        }

        /// <summary>
        /// Updates the UI elements to reflect current system settings
        /// </summary>
        private void RefreshUIWithSettings()
        {
            var settings = _settingsManager.CurrentSettings;
            
            masterVolumeSlider.value = settings.masterVolume;
            musicVolumeSlider.value = settings.musicVolume;
            sfxVolumeSlider.value = settings.sfxVolume;
            uiVolumeSlider.value = settings.uiVolume;
            
            qualityDropdown.value = settings.qualityLevel;
            qualityDropdown.RefreshShownValue();

            UpdateResolutionDropdown();
            resolutionsDropdown.RefreshShownValue();
            
            fullscreenToggle.isOn = settings.isFullscreen;
            vSyncToggle.isOn = settings.isVSync;
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
        /// Reverts fullscreen settings to previous state
        /// </summary>
        private void RevertFullscreen()
        {
            Screen.fullScreen = _wasFullScreen;
            Screen.fullScreenMode = _previousFullScreenMode;
            
            // Update UI to match reverted settings without triggering events.
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
            
            // Update UI to match reverted resolution without triggering events.
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            resolutionsDropdown.value = resolutionsDropdown.options.FindIndex(option =>
                option.text == _previousResolution.ToString());
            resolutionsDropdown.RefreshShownValue();
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
        }
    }
}