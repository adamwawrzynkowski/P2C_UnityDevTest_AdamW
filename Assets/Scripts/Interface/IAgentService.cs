using UnityEngine;

public interface IAgentService {
    public static float DefaultAgentSpeed;
    public static float DefaultAgentStoppingDistance;
    
    public Vector3 FindPoint(BoxCollider area);
    public void GoToDestination();
    public void GetDestinationRange(Vector3 destinationPosition);
}
