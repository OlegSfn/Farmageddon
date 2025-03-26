using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quests
{
    [System.Serializable]
    public struct QuestUIPanel
    {
        public Image questPanelImage;
        public Image questItemImage;
        public GameObject strikethrough;
        public TextMeshProUGUI questCompletenessText;
        public TextMeshProUGUI questDeadlineText;
        public TextMeshProUGUI questRewardText;
    }
    
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questMenuTitle;
        [SerializeField] private GameObject questMenuDropdown;
        [SerializeField] private QuestUIPanel[] questUIPanels;
        
        [SerializeField] private Color incompleteQuestColor;
        [SerializeField] private Color completeQuestColor;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HandleQuestButtonClick();
            }
        }

        public void UpdateQuestMenuUI()
        {
            QuestManager questManager = Managers.GameManager.Instance.questManager;
            var quests = questManager.ActiveQuests;
            questMenuTitle.text = $"Quests ({quests.Count(x => x.isCompleted)}/{questManager.maxActiveQuests})";
            
            for (int i = 0; i < questUIPanels.Length; i++)
            {
                UpdateQuestPanelUI(questUIPanels[i], quests[i]);
            }
        }
        
        public void HandleQuestButtonClick()
        {
            if (questMenuDropdown.activeInHierarchy)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void UpdateQuestPanelUI(QuestUIPanel panelToUpdate, ActiveQuest quest)
        {
            panelToUpdate.questItemImage.sprite = quest.questData.itemSprite;
            panelToUpdate.questPanelImage.color = quest.isCompleted ? completeQuestColor : incompleteQuestColor;
            panelToUpdate.strikethrough.SetActive(quest.isCompleted);
            panelToUpdate.questCompletenessText.text = $"{quest.currentAmount}/{quest.questData.requiredAmount}";
            panelToUpdate.questDeadlineText.text =
                quest.remainingDays == 1
                    ? $"{Mathf.CeilToInt(quest.remainingDays)} day left"
                    : $"{Mathf.CeilToInt(quest.remainingDays)} days left";
            panelToUpdate.questRewardText.text = $"{quest.questData.rewardAmount} $";
        }
        
        private void Show()
        {
            questMenuDropdown.SetActive(true);
            UpdateQuestMenuUI();
        }
    
        private void Hide()
        {
            questMenuDropdown.SetActive(false);
        }
    }
}
