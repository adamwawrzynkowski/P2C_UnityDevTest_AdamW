using Tick;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : MonoBehaviour, Interface.ITickService {
        public static UIManager Instance;
        
        [Header("Tick Panel")]
        [SerializeField] private TMP_Text currentTimeLabel;
        [SerializeField] private Button tickUpButton;
        [SerializeField] private Button tickDownButton;
        [SerializeField] private Button tickPauseButton;
        
        private void Awake() {
            Instance = this;
            
            // Set listeners for buttons
            tickUpButton.onClick.AddListener(() => { SetTickRate(1); });
            tickDownButton.onClick.AddListener(() => { SetTickRate(-1); });
            tickPauseButton.onClick.AddListener(() => { SetTickRate(0); });
            
            RefreshTickUI();
        }

        public void SetTickRate(int tickValue) {
            TickManager.Instance.SetTickRate(tickValue);
        }

        // Get current tick and refresh UI
        public void RefreshTickUI() {
            // Set the visual representation to something more readable than range between 0-4
            var visualSpeedMark = TickManager.Instance.GetCurrentTick() switch {
                4 => "x2",
                3 => "x1",
                2 => "x1/2",
                1 => "x1/4",
                0 => "Paused",
                _ => ""
            };

            currentTimeLabel.text = "Speed: " + visualSpeedMark;
        }
    }
}