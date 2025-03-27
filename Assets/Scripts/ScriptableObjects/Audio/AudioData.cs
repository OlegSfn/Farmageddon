using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ScriptableObjects.Audio
{
    /// <summary>
    /// ScriptableObject that manages audio data configuration for the game
    /// Contains collections of audio groups for both music and sound effects
    /// </summary>
    [CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/Audio/AudioData")]
    public class AudioData : ScriptableObject
    {
        /// <summary>
        /// Represents a group of related audio clips with shared playback settings
        /// </summary>
        [System.Serializable]
        public class AudioGroup
        {
            /// <summary>
            /// Unique identifier for this audio group
            /// </summary>
            public string groupName;
            
            /// <summary>
            /// Collection of audio clips in this group
            /// </summary>
            public List<AudioClip> clips = new();
            
            /// <summary>
            /// Volume level for all clips in this group (0-1)
            /// </summary>
            [Range(0f, 1f)] public float volume = 1f;
            
            /// <summary>
            /// Minimum pitch variation for randomized playback
            /// </summary>
            [Range(0.1f, 3f)] public float minPitch = 0.9f;
            
            /// <summary>
            /// Maximum pitch variation for randomized playback
            /// </summary>
            [Range(0.1f, 3f)] public float maxPitch = 1.1f;
            
            /// <summary>
            /// Controls whether the sound is 2D (0) or 3D (1) positioned in the world
            /// </summary>
            [Range(0f, 1f)] public float spatialBlend = 0f;
            
            /// <summary>
            /// Whether the audio should loop continuously
            /// </summary>
            public bool loop = false;
            
            /// <summary>
            /// Audio mixer group for routing and effects processing
            /// </summary>
            public AudioMixerGroup mixerGroup;
        }

        /// <summary>
        /// Collection of all background music audio groups
        /// </summary>
        public List<AudioGroup> musicGroups = new();

        /// <summary>
        /// Collection of all sound effect audio groups
        /// </summary>
        public List<AudioGroup> sfxGroups = new();

        /// <summary>
        /// Cached dictionary of audio groups for faster lookup by name
        /// </summary>
        private Dictionary<string, AudioGroup> _groupCache;

        /// <summary>
        /// Initializes the audio data when the ScriptableObject is enabled
        /// </summary>
        private void OnEnable()
        {
            BuildCache();
        }

        /// <summary>
        /// Builds a lookup cache of all audio groups for efficient access
        /// </summary>
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

        /// <summary>
        /// Retrieves an audio group by its name
        /// </summary>
        /// <param name="groupName">The name of the audio group to retrieve</param>
        /// <returns>The requested audio group, or null if not found</returns>
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

        /// <summary>
        /// Gets a random audio clip from the specified group
        /// </summary>
        /// <param name="groupName">The name of the audio group to get a clip from</param>
        /// <returns>A random audio clip from the group, or null if the group is empty or doesn't exist</returns>
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
