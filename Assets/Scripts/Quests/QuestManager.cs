using System.Collections.Generic;
using System.Linq;
using Managers;
using ScriptableObjects.Quests;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Quests
{
    /// <summary>
    /// Manages the quest system in the game
    /// Handles quest generation, tracking, and completion
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        /// <summary>
        /// List of quest templates that can be assigned to the player
        /// </summary>
        [SerializeField] private List<QuestData> availableQuests = new();
        
        /// <summary>
        /// Event triggered when quests are updated, added or completed
        /// </summary>
        [SerializeField] private UnityEvent onQuestsUpdated;
    
        /// <summary>
        /// Maximum number of quests that can be active at once
        /// </summary>
        public int maxActiveQuests => 3;
        
        /// <summary>
        /// List of currently active quests
        /// </summary>
        public List<ActiveQuest> ActiveQuests { get; } = new();

        /// <summary>
        /// Initialize the quest system at the start of the game
        /// </summary>
        private void Start()
        {
            RefreshQuests();
        }
    
        /// <summary>
        /// Called at the start of each new day
        /// Updates quest timers and refreshes available quests
        /// </summary>
        public void OnDayStart()
        {
            UpdateQuestTimers();
            RefreshQuests();
        }
    
        /// <summary>
        /// Decrements the remaining days for all active quests
        /// Removes quests that have expired
        /// </summary>
        private void UpdateQuestTimers()
        {
            List<ActiveQuest> expiredQuests = new();
        
            foreach (var quest in ActiveQuests)
            {
                --quest.remainingDays;
                if (quest.remainingDays <= 0 && !quest.isCompleted)
                {
                    expiredQuests.Add(quest);
                }
            }
        
            foreach (var quest in expiredQuests)
            {
                ActiveQuests.Remove(quest);
            }
        }
    
        /// <summary>
        /// Refreshes the list of active quests
        /// Removes completed quests and adds new ones until the maximum is reached
        /// </summary>
        private void RefreshQuests()
        {
            ActiveQuests.RemoveAll(quest => quest.isCompleted);
            
            while (ActiveQuests.Count < maxActiveQuests && availableQuests.Count > 0)
            {
                int randomIndex = Random.Range(0, availableQuests.Count);
                QuestData questData = availableQuests[randomIndex];
            
                ActiveQuest newQuest = new ActiveQuest(questData);
                ActiveQuests.Add(newQuest);
            }
        
            onQuestsUpdated?.Invoke();
        }
    
        /// <summary>
        /// Checks if an item contributes to quest progress
        /// Updates quest progress and completes quests when requirements are met
        /// </summary>
        /// <param name="itemName">Name of the item to check against quest requirements</param>
        /// <param name="amount">Amount of the item to add to quest progress</param>
        public void CheckQuestProgress(string itemName, int amount)
        {
            bool isAnyQuestUpdated = false;
            
            foreach (var quest in ActiveQuests.Where(quest => !quest.isCompleted && 
                                                              quest.questData.itemToSell.itemName == itemName))
            {
                quest.currentAmount += amount;
                isAnyQuestUpdated = true;
                
                if (quest.currentAmount >= quest.questData.requiredAmount && !quest.isCompleted)
                {
                    CompleteQuest(quest);
                }
            }
        
            if (isAnyQuestUpdated)
            {
                onQuestsUpdated?.Invoke();
            }
        }
    
        /// <summary>
        /// Marks a quest as completed and awards the player with the quest reward
        /// </summary>
        /// <param name="quest">The quest to complete</param>
        private void CompleteQuest(ActiveQuest quest)
        {
            quest.isCompleted = true;
            GameManager.Instance.cashManager.Cash += quest.questData.rewardAmount;
        }
    }

    /// <summary>
    /// Represents an active quest instance that the player can complete
    /// Tracks progress, time remaining, and completion status
    /// </summary>
    [System.Serializable]
    public class ActiveQuest
    {
        /// <summary>
        /// Reference to the quest template data
        /// </summary>
        public QuestData questData;
        
        /// <summary>
        /// Current progress toward completing the quest
        /// </summary>
        public int currentAmount;
        
        /// <summary>
        /// Number of in-game days remaining before the quest expires
        /// </summary>
        public int remainingDays;
        
        /// <summary>
        /// Flag indicating whether this quest has been completed
        /// </summary>
        public bool isCompleted;
    
        /// <summary>
        /// Creates a new active quest from a quest template
        /// </summary>
        /// <param name="data">The quest template data</param>
        public ActiveQuest(QuestData data)
        {
            questData = data;
            currentAmount = 0;
            remainingDays = data.daysLimit;
            isCompleted = false;
        }
    }
}