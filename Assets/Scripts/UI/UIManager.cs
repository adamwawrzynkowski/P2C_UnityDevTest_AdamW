using System;
using System.Collections;
using System.Collections.Generic;
using Agents;
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

        [Header("Agents Panel")]
        [SerializeField] private TMP_Text currentAgentsCount;
        [SerializeField] private Button addAgentButton;
        [SerializeField] private Button removeRandomAgentButton;
        [SerializeField] private Button removeAllAgentsButton;

        [Header("Console")]
        [SerializeField] private TMP_Text console;

        private List<string> consoleStrings = new List<string>();

        private bool tabPressed;
        public bool GetTabPressed() => tabPressed;
        
        private void Awake() {
            Instance = this;
            
            // Set listeners for buttons
            tickUpButton.onClick.AddListener(() => { SetTickRate(1); });
            tickDownButton.onClick.AddListener(() => { SetTickRate(-1); });
            tickPauseButton.onClick.AddListener(() => { SetTickRate(0); });
            
            addAgentButton.onClick.AddListener(() => { AgentsManager.Instance.RequestAgentSpawn(); });
            removeRandomAgentButton.onClick.AddListener(() => { AgentsManager.Instance.RequestAgentRemove(AgentRemoveType.Random); });
            removeAllAgentsButton.onClick.AddListener(() => { AgentsManager.Instance.RequestAgentRemove(AgentRemoveType.All); });
        }

        private void Start() {
            RefreshTickUI();
            RefreshAgentsUI();
        }

        private void Update() {
            tabPressed = Input.GetKey(KeyCode.Tab);
        }

        public void SetTickRate(int tickValue) {
            TickManager.Instance.SetTickRate(tickValue);
        }

        // Get current tick and refresh UI
        public void RefreshTickUI() {
            // Set the visual representation to something more readable than range between 0-4
            var visualSpeedMark = TickManager.Instance.GetCurrentTick() switch {
                5 => "x4",
                4 => "x2",
                3 => "x1",
                2 => "x1/2",
                1 => "x1/4",
                0 => "Paused",
                _ => ""
            };

            currentTimeLabel.text = "Speed: " + visualSpeedMark;
        }

        public void RefreshAgentsUI() {
            currentAgentsCount.text = "Agents: " + AgentsManager.Instance.GetAgentsCount();
        }

        private void RefreshConsole() {
            console.text = "";
            foreach (var consoleString in consoleStrings) {
                console.text += consoleString;
            }
        }
        
        public void AddToConsole(string newString) {
            consoleStrings.Add(newString + "\n");
            StartCoroutine(ConsoleLoop());
            RefreshConsole();
        }

        private IEnumerator ConsoleLoop() {
            yield return new WaitForSeconds(3.0f);
            if (consoleStrings.Count > 0) consoleStrings.RemoveAt(0);
            RefreshConsole();
        }
    }
}