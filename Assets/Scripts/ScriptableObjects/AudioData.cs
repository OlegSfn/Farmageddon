using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/Audio/AudioData")]
    public class AudioData : ScriptableObject
    {
        [System.Serializable]
        public class AudioGroup
        {
            public string groupName;
            public List<AudioClip> clips = new();
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.1f, 3f)] public float minPitch = 0.9f;
            [Range(0.1f, 3f)] public float maxPitch = 1.1f;
            [Range(0f, 1f)] public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
            public bool loop = false;
            public AudioMixerGroup mixerGroup;
        }

        public List<AudioGroup> musicGroups = new();

        public List<AudioGroup> sfxGroups = new();

        private Dictionary<string, AudioGroup> _groupCache;

        private void OnEnable()
        {
            BuildCache();
        }

        public void BuildCache()
        {
            _groupCache = new Dictionary<string, AudioGroup>();
        
            foreach (var group in musicGroups)
            {
                if (!string.IsNullOrEmpty(group.groupName))
                {
                    _groupCache[group.groupName] = group;
                }
            }

            foreach (var group in sfxGroups)
            {
                if (!string.IsNullOrEmpty(group.groupName))
                {
                    _groupCache[group.groupName] = group;
                }
            }
        }

        public AudioGroup GetAudioGroup(string groupName)
        {
            if (_groupCache == null)
            {
                BuildCache();
            }

            if (_groupCache.TryGetValue(groupName, out AudioGroup group))
            {
                return group;
            }

            Debug.LogWarning($"Audiogroup '{groupName}' is not found!");
            return null;
        }

        public AudioClip GetRandomClip(string groupName)
        {
            var group = GetAudioGroup(groupName);
            if (group == null || group.clips.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, group.clips.Count);
            return group.clips[randomIndex];
        }
    }
}
