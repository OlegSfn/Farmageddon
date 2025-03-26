using System;
using System.Collections.Generic;
using Managers;
using ScriptableObjects.Quests;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<QuestData> availableQuests = new();
        [SerializeField] private UnityEvent onQuestsUpdated;
    
        public int maxActiveQuests => 3;
        public List<ActiveQuest> ActiveQuests { get; private set; } = new();

        private void Start()
        {
            RefreshQuests();
        }
    
        public void OnDayStart()
        {
            UpdateQuestTimers();
            RefreshQuests();
        }
    
        private void UpdateQuestTimers()
        {
            List<ActiveQuest> expiredQuests = new List<ActiveQuest>();
        
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
    
        public void CheckQuestProgress(string itemName, int amount)
        {
            bool isAnyQuestUpdated = false;
            foreach (var quest in ActiveQuests)
            {
                if (quest.isCompleted)
                {
                    continue;
                }
            
                if (quest.questData.itemToSell.itemName == itemName)
                {
                    quest.currentAmount += amount;
                    isAnyQuestUpdated = true;
                
                    if (quest.currentAmount >= quest.questData.requiredAmount && !quest.isCompleted)
                    {
                        CompleteQuest(quest);
                    }
                }
            }
        
            if (isAnyQuestUpdated)
            {
                onQuestsUpdated?.Invoke();
            }
        }
    
        private void CompleteQuest(ActiveQuest quest)
        {
            quest.isCompleted = true;
            GameManager.Instance.cashManager.Cash += quest.questData.rewardAmount;
        }
    }

    [System.Serializable]
    public class ActiveQuest
    {
        public QuestData questData;
        public int currentAmount;
        public float remainingDays;
        public bool isCompleted;
    
        public ActiveQuest(QuestData data)
        {
            questData = data;
            currentAmount = 0;
            remainingDays = data.daysLimit;
            isCompleted = false;
        }
    }
}