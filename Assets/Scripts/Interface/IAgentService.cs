using UnityEngine;

namespace Agents {
    public interface IAgentService {
        public static float DefaultAgentMoveDuration = 2.0f;
        public static float DefaultAgentStoppingDistance = 1.0f;

        public Vector3 FindPoint(BoxCollider area);
        public void GoToDestination();
    }
}