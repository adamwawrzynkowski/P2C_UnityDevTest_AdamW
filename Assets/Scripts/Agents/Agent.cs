using UnityEngine;

namespace Agents {
    public class Agent : MonoBehaviour, IAgentService {
        private double GUID;

        public double GetGUID() => GUID;
        public void SetGUID(double guid) => GUID = guid;
        
        public Vector3 FindPoint(BoxCollider area) {
            var randomBoundsX = Random.Range(area.bounds.min.x, area.bounds.max.x);
            var randomBoundsZ = Random.Range(area.bounds.min.z, area.bounds.max.z);
            return new Vector3(randomBoundsX, 0.0f, randomBoundsZ);
        }

        public void GoToDestination() {
            
        }

        public void GetDestinationRange(Vector3 destinationPosition) {
            
        }

        public void Unregister() {
            Destroy(gameObject);
        }
    }
}