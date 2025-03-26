using TMPro;
using UnityEngine;

namespace Managers
{
    public class CashManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cashText;

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
        
        private void Awake()
        {
            UpdateCashUI();
        }

        private void UpdateCashUI()
        {
            cashText.text = $"{_cash} $";
        }
    }
}
