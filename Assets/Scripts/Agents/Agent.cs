using System;
using DG.Tweening;
using Tick;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents {
    public class Agent : MonoBehaviour, IAgentService {
        private double GUID = 0;

        public double GetGUID() => GUID;
        public void SetGUID(double guid) => GUID = guid;

        private Vector3 destinationPosition;
        private Tweener tweener;
        private LineRenderer lr;
        private Animator animator;

        private void Awake() {
            lr = GetComponent<LineRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update() {
            transform.LookAt(new Vector3(destinationPosition.x, 0.0f, destinationPosition.z));
            
            if (lr == null) return;
            if (UIManager.Instance.GetTabPressed()) {
                if (!lr.enabled) lr.enabled = true;
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, destinationPosition);
                
                return;
            }
            
            if (lr.enabled) lr.enabled = false;
        }

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
            UIManager.Instance.AddToConsole("Agent " + GUID + " arrived.");
        }

        public void Unregister() {
            tweener.Kill();
            Destroy(gameObject);
        }

        public void UpdateTweener() {
            tweener.timeScale = TickManager.Instance.GetCurrentTickSpeed();
            animator.speed = IAgentService.DefaultAnimationSpeed * TickManager.Instance.GetCurrentTickSpeed();
        }

        private float GetDistance() {
            return Vector3.Distance(transform.position, destinationPosition);
        }
    }
}