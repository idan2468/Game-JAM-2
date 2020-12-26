using UnityEngine;
using PathCreation;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class PathFollow : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject pathObj;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 10;
    float distanceTravelled;
    
    void Start()
    {
        endOfPathInstruction = EndOfPathInstruction.Stop;
    }

    void Update()
    {
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            if (distanceTravelled >= pathCreator.path.length)
            {
                Debug.Log("Distance traveled : " + distanceTravelled);
                Debug.Log("Disconnected from path... \n Removing path");
                pathCreator = null;
                gameObject.GetComponent<TempJewControler>().EnterFreeState();
                distanceTravelled = 0;
                Destroy(pathObj);
            }
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}