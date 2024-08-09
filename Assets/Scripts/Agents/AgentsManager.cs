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

        public void RequestAgentSpawn() {
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
        
        private void RegisterAgent(Agent agent) {
            var newGUID = Random.Range(1000000, 9999999);
            while (CheckGUIDExists(newGUID)) {
                newGUID = Random.Range(1000000, 9999999);
            }
            
            agent.SetGUID(newGUID);
            registeredAgents.Add(agent);
            
            UIManager.Instance.RefreshAgentsUI();
        }

        private void UnregisterAgent(Agent agent, bool modifyCollection = true) {
            if (modifyCollection) registeredAgents.RemoveAt(registeredAgents.FindIndex(a => agent == a));
            agent.Unregister();
            
            UIManager.Instance.RefreshAgentsUI();
        }

        private void RemoveAllAgents() {
            foreach (var agent in registeredAgents.Where(agent => agent != null)) {
                UnregisterAgent(agent, false);
            }

            registeredAgents.Clear();
            UIManager.Instance.RefreshAgentsUI();
        }

        private bool CheckGUIDExists(double guid) {
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
