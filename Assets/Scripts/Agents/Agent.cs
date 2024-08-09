using System;
using DG.Tweening;
using DG.Tweening.Core;
using Tick;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents {
    public class Agent : MonoBehaviour, IAgentService {
        private double GUID = 0;

        public double GetGUID() => GUID;
        public void SetGUID(double guid) => GUID = guid;

        private Vector3 destinationPosition;
        private Tweener tweener;

        public Vector3 FindPoint(BoxCollider area) {
            var randomBoundsX = Random.Range(area.bounds.min.x, area.bounds.max.x);
            var randomBoundsZ = Random.Range(area.bounds.min.z, area.bounds.max.z);
            return new Vector3(randomBoundsX, 0.0f, randomBoundsZ);
        }

        public void GoToDestination() {
            destinationPosition = FindPoint(AgentsManager.Instance.GetArea());
            tweener = transform.DOMove(
                destinationPosition, 
                GetDistance() / IAgentService.DefaultAgentMoveDuration
                ).SetEase(Ease.Linear).OnComplete(GoalCompleted);
            
            UpdateTweener();
        }

        private void GoalCompleted() {
            GoToDestination();
            
            
        }

        public void Unregister() {
            tweener.Kill();
            Destroy(gameObject);
        }

        public void UpdateTweener() {
            tweener.timeScale = TickManager.Instance.GetCurrentTickSpeed();
        }

        private float GetDistance() {
            return Vector3.Distance(transform.position, destinationPosition);
        }
    }
}