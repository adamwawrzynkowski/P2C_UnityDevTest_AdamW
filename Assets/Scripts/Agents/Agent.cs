using System;
using System.Linq;
using DG.Tweening;
using Pathfinding;
using Tick;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents {
    public class Agent : MonoBehaviour, IAgentService {
        private Guid GUID;

        public Guid GetGUID() => GUID;
        public void SetGUID(Guid guid) => GUID = guid;

        private Vector3 destinationPosition;
        private Tweener tweener;
        private LineRenderer lr;
        private Animator animator;
        
        private Seeker seeker;
        private Vector3[] path;
        private int currentPathIndex = 0;

        private void Awake() {
            lr = GetComponent<LineRenderer>();
            animator = GetComponentInChildren<Animator>();
            seeker = GetComponent<Seeker>();
        }

        private void Update() {
            transform.DOLookAt(destinationPosition, 0.1f, AxisConstraint.Y);
            
            if (lr == null || path == null) return;
            if (UIManager.Instance.GetTabPressed()) {
                if (!lr.enabled) lr.enabled = true;
                lr.SetPositions(path.Reverse().ToArray());
                return;
            }
            
            if (lr.enabled) lr.enabled = false;
        }

        public Vector3 FindPoint(BoxCollider area) {
            var randomBoundsX = Random.Range(area.bounds.min.x, area.bounds.max.x);
            var randomBoundsZ = Random.Range(area.bounds.min.z, area.bounds.max.z);
            return new Vector3(randomBoundsX, 0.0f, randomBoundsZ);
        }

        private void FindPath(BoxCollider area) {
            seeker.StartPath(transform.position, FindPoint(area), PathFound);
        }

        private void PathFound(Path _path) {
            if (_path.error) {
                Debug.LogError("Path failed: " + _path.errorLog);
                return;
            }
            
            path = _path.vectorPath.ToArray();
            
            currentPathIndex = 0;
            GoToDestination();
        }

        public void GoToDestination() {
            if (path == null) {
                FindPath(AgentsManager.Instance.GetArea());
                return;
            }
            
            if (currentPathIndex >= path.Length || currentPathIndex == -1) {
                path = null;
                
                FindPath(AgentsManager.Instance.GetArea());
                return;
            }
            
            destinationPosition = path[currentPathIndex];
            currentPathIndex++;
            
            lr.positionCount = path.Length + 1 - currentPathIndex;
            
            tweener = transform.DOMove(
                destinationPosition, 
                GetDistance() / IAgentService.DefaultAgentMoveDuration
                ).SetEase(Ease.Linear).OnComplete(GoalCompleted);
            
            UpdateTweener();
        }

        private void GoalCompleted() {
            GoToDestination();
            
            if (currentPathIndex != path.Length) return;
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