using TMPro;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages the player's cash/currency system
    /// Handles currency display and modification
    /// </summary>
    public class CashManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the UI text element that displays the cash amount
        /// </summary>
        [SerializeField] private TextMeshProUGUI cashText;

        /// <summary>
        /// The player's current cash amount
        /// Automatically updates the UI when modified
        /// </summary>
        public int Cash
        {
            set
            {
                _cash = value;
                UpdateCashUI();
            }

            get => _cash;
        }
        
        private int _cash = 100;
        
        /// <summary>
        /// Initialize the cash display
        /// </summary>
        private void Awake()
        {
            UpdateCashUI();
        }

        /// <summary>
        /// Updates the UI text to display the current cash amount
        /// </summary>
        private void UpdateCashUI()
        {
            cashText.text = $"{_cash} $";
        }
    }
}