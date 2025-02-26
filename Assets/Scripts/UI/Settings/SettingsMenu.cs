using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private TMP_Dropdown resolutionsDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        [SerializeField] private ApplyChangesMenu applyChangesMenu;
        private Resolution _previousResolution;
        private FullScreenMode _previousFullScreenMode;
        private bool _wasFullScreen;
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            _previousFullScreenMode = Screen.fullScreenMode;
            _wasFullScreen = Screen.fullScreen;

            Screen.fullScreen = isFullscreen;
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

            applyChangesMenu.Open(() => { }, RevertFullscreen);
        }

        public void SetResolution(int resolutionIndex)
        {
            _previousResolution = Screen.currentResolution;
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            applyChangesMenu.Open(() => { }, RevertResolution);
        }

        public void SetVSync(bool isVSync)
        {
            QualitySettings.vSyncCount = isVSync ? 1 : 0;
        }

        private void RefreshUIWithSettings()
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

            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
            
            fullscreenToggle.isOn = Screen.fullScreen;
        }
        
        private void OnEnable()
        {
            RefreshUIWithSettings();
            
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        private void OnDisable()
        {
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }
        
        private void RevertFullscreen()
        {
            Screen.fullScreen = _wasFullScreen;
            Screen.fullScreenMode = _previousFullScreenMode;
            
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
            fullscreenToggle.isOn = _wasFullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        private void RevertResolution()
        {
            Screen.SetResolution(_previousResolution.width, _previousResolution.height, Screen.fullScreen);
            
            resolutionsDropdown.onValueChanged.RemoveListener(SetResolution);
            resolutionsDropdown.value = resolutionsDropdown.options.FindIndex(option =>
                option.text == _previousResolution.ToString());
            resolutionsDropdown.RefreshShownValue();
            resolutionsDropdown.onValueChanged.AddListener(SetResolution);
        }
    }
}
