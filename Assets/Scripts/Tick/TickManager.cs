using Interface;
using UI;
using UnityEngine;

namespace Tick {
    public class TickManager : MonoBehaviour, ITickService {
        public static TickManager Instance;
        private void Awake() {
            Instance = this;
        }
        
        // Default speed (x1)
        private int currentTick = 3;
        private float currentTickSpeed = 1.0f;

        public int GetCurrentTick() => currentTick;
        public float GetCurrentTickSpeed() => currentTickSpeed;

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
            
            // Set tick speed for Agents based on the tick ID
            currentTickSpeed = GetCurrentTick() switch {
                4 => 2.0f,
                3 => 1.0f,
                2 => 0.5f,
                1 => 0.25f,
                0 => 0.0f,
                _ => 0.0f
            };
            
            // Refresh tick UI
            RefreshTickUI();
        }
        
        // Compare current tick with new tick to detect the game is already paused
        private bool CheckPause(int tickToCheck) {
            return tickToCheck == 0 && GetCurrentTick() == 0;
        }
        
        // Compare tick values to detect the value is not above the range
        private static int CheckTickRange(int tickToCheck) {
            var newTick = tickToCheck;
            if (newTick < 0) newTick = 0;
            if (newTick > 4) newTick = 4;
            return newTick;
        }

        public void RefreshTickUI() {
            UIManager.Instance.RefreshTickUI();
        }
    }
}