using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quests
{
    /// <summary>
    /// Structure containing all UI elements for a single quest display panel
    /// </summary>
    [System.Serializable]
    public struct QuestUIPanel
    {
        /// <summary>
        /// Background panel image that changes color based on quest completion status
        /// </summary>
        public Image questPanelImage;
        
        /// <summary>
        /// Image displaying the quest's target item icon
        /// </summary>
        public Image questItemImage;
        
        /// <summary>
        /// Visual indicator that appears when a quest is completed
        /// </summary>
        public GameObject strikethrough;
        
        /// <summary>
        /// Text showing current progress toward quest completion
        /// </summary>
        public TextMeshProUGUI questCompletenessText;
        
        /// <summary>
        /// Text showing how many days remain before the quest expires
        /// </summary>
        public TextMeshProUGUI questDeadlineText;
        
        /// <summary>
        /// Text showing the cash reward for completing the quest
        /// </summary>
        public TextMeshProUGUI questRewardText;
    }
    
    /// <summary>
    /// Manages the quest UI display and interaction
    /// Handles showing/hiding the quest panel and updating quest information
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        /// <summary>
        /// Title text element showing current quest completion status
        /// </summary>
        [SerializeField] private TextMeshProUGUI questMenuTitle;
        
        /// <summary>
        /// The dropdown panel containing all quest information
        /// </summary>
        [SerializeField] private GameObject questMenuDropdown;
        
        /// <summary>
        /// Array of UI panels, one for each active quest
        /// </summary>
        [SerializeField] private QuestUIPanel[] questUIPanels;
        
        /// <summary>
        /// Color for quests that are not yet completed
        /// </summary>
        [SerializeField] private Color incompleteQuestColor;
        
        /// <summary>
        /// Color for quests that have been completed
        /// </summary>
        [SerializeField] private Color completeQuestColor;

        /// <summary>
        /// Checks for quest menu toggle key press
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HandleQuestButtonClick();
            }
        }

        /// <summary>
        /// Updates all quest UI elements with current quest data
        /// </summary>
        public void UpdateQuestMenuUI()
        {
            QuestManager questManager = GameManager.Instance.questManager;
            var quests = questManager.ActiveQuests;
            
            questMenuTitle.text = $"Quests ({quests.Count(x => x.isCompleted)}/{questManager.maxActiveQuests})";
            
            for (int i = 0; i < questUIPanels.Length; i++)
            {
                UpdateQuestPanelUI(questUIPanels[i], quests[i]);
            }
        }
        
        /// <summary>
        /// Toggles the quest menu visibility when the quest button is clicked
        /// </summary>
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

        /// <summary>
        /// Updates a single quest panel with the specified quest's information
        /// </summary>
        /// <param name="panelToUpdate">The UI panel to update</param>
        /// <param name="quest">The quest data to display</param>
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
        
        /// <summary>
        /// Shows the quest menu dropdown and updates quest information
        /// </summary>
        private void Show()
        {
            questMenuDropdown.SetActive(true);
            UpdateQuestMenuUI();
        }
    
        /// <summary>
        /// Hides the quest menu dropdown
        /// </summary>
        private void Hide()
        {
            questMenuDropdown.SetActive(false);
        }
    }
}
