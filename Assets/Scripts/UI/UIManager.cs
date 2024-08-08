using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : MonoBehaviour, Interface.ITickService {
        [Header("Tick Panel")]
        [SerializeField] private TMP_Text currentTimeLabel;
        [SerializeField] private Button tickUpButton;
        [SerializeField] private Button tickDownButton;
        [SerializeField] private Button tickPauseButton;

        // Default speed (x1)
        private int currentTick = 3;
        
        private void Awake() {
            // Set listeners for buttons
            tickUpButton.onClick.AddListener(() => { SetTickRate(1); });
            tickDownButton.onClick.AddListener(() => { SetTickRate(-1); });
            tickPauseButton.onClick.AddListener(() => { SetTickRate(0); });
            
            RefreshTickUI();
        }

        public void SetTickRate(int tickValue) {
            int newTick;
            
            // Check if game is currently paused
            if (CheckPause(tickValue)) {
                // If the game is currently paused and the user is pressing
                // on the Pause button again - set the default speed to unpause
                newTick = 3;
            } else {
                // If the game is not paused, set the tick based on the 1 or -1 value
                newTick = tickValue == 0 ? 0 : currentTick += tickValue;
            }
            
            // Check tick range to avoid going above limits
            currentTick = CheckTickRange(newTick);
            
            // Refresh tick UI
            RefreshTickUI();
        }

        // Compare current tick with new tick to detect the game is already paused
        private bool CheckPause(int tickToCheck) {
            return tickToCheck == 0 && currentTick == 0;
        }
        
        // Compare tick values to detect the value is not above the range
        private static int CheckTickRange(int tickToCheck) {
            var newTick = tickToCheck;
            if (newTick < 0) newTick = 0;
            if (newTick > 4) newTick = 4;
            return newTick;
        }

        // Get current tick and refresh UI
        public void RefreshTickUI() {
            // Set the visual representation to something more readable than range between 0-4
            var visualSpeedMark = currentTick switch {
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