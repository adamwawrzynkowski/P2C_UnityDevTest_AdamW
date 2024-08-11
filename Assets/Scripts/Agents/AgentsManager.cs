using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AgentRemoveType {
    All,
    Random
}

namespace Agents {
    public class AgentsManager : MonoBehaviour {
        public static AgentsManager Instance;
        private void Awake() {
            Instance = this;
        }

        [Header("Agent")]
        [SerializeField] private Transform agentsParent;
        [SerializeField] private GameObject agentPrefab;

        [Header("Area")]
        [SerializeField] private BoxCollider area;

        public BoxCollider GetArea() => area;

        private List<Agent> registeredAgents = new List<Agent>();
        public int GetAgentsCount() => registeredAgents.Count;

        // Spawn new agent and register him
        public void RequestAgentSpawn() {
            var iterations = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;
            for (var i = 0; i < iterations; i++) {
                var newAgent = Instantiate(agentPrefab, agentsParent, true);
                var agentComponent = newAgent.GetComponent<Agent>();
                if (agentComponent == null) {
                    Debug.Log("Unable to find Agent component in the spawned object. Adding one...");
                    agentComponent = newAgent.AddComponent<Agent>();
                }
            
                RegisterAgent(agentComponent);
                newAgent.transform.position = agentComponent.FindPoint(area);
            
                agentComponent.GoToDestination();
            }
        }

        // Remove agent based on the given settings
        public void RequestAgentRemove(AgentRemoveType type) {
            if (registeredAgents == null || registeredAgents.Count <= 0) return;
            switch (type) {
                case AgentRemoveType.All:
                    RemoveAllAgents();
                    break;
                
                case AgentRemoveType.Random:
                    UnregisterAgent(registeredAgents[Random.Range(0, GetAgentsCount())]);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        // Register new agent and assign an GUID to him
        private void RegisterAgent(Agent agent) {
            var newGUID = Guid.NewGuid();
            while (CheckGUIDExists(newGUID)) {
                newGUID = Guid.NewGuid();
            }
            
            agent.SetGUID(newGUID);
            registeredAgents.Add(agent);
            
            UIManager.Instance.RefreshAgentsUI();
        }

        // Unregister agent and remove him from the list
        private void UnregisterAgent(Agent agent, bool modifyCollection = true) {
            if (modifyCollection) registeredAgents.RemoveAt(registeredAgents.FindIndex(a => agent == a));
            agent.Unregister();
            
            UIManager.Instance.RefreshAgentsUI();
        }

        // Unregister all agents on the scene
        private void RemoveAllAgents() {
            foreach (var agent in registeredAgents.Where(agent => agent != null)) {
                UnregisterAgent(agent, false);
            }

            registeredAgents.Clear();
            UIManager.Instance.RefreshAgentsUI();
        }

        // Check if this GUID already exists on the list
        private bool CheckGUIDExists(Guid guid) {
            foreach (var _ in registeredAgents.Where(agent => agent.GetGUID() == guid)) {
                return true;
            }

            return false;
        }

        public void UpdateAgents() {
            foreach (var agent in registeredAgents) {
                agent.UpdateTweener();
            }
        }
    }
}
