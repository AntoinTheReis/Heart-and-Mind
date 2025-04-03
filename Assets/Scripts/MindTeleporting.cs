using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MindTeleporting : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriterenderer;

    Controls input;

    public Switcher switcher;
    public List<Transform> busStops;

    public float orbitRadius = 5f; 
    public float orbitSpeed = 2f;

    [Header("Arrows")]
    public GameObject northArrow;
    public GameObject southArrow;
    public GameObject eastArrow;
    public GameObject westArrow;

    private Transform optionNorth;
    private Transform optionSouth;
    private Transform optionEast;
    private Transform optionWest;

    private bool lateStart = false;

    private bool positionUpdated = false;
    public bool movementMode = false;

    private MindBlockTelekinesis telekinesis;

    private Transform currentBusStop;

    #region Audio
    public FMODUnity.EventReference sfx_teleport;
    FMOD.Studio.EventInstance sfx_teleportInstance;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        #region Audio EventInstances
        sfx_teleportInstance = FMODUnity.RuntimeManager.CreateInstance(sfx_teleport);
        #endregion

        switcher = GameObject.FindGameObjectWithTag("Switcher").GetComponent<Switcher>();
        telekinesis = GetComponent<MindBlockTelekinesis>();

        input = GetComponent<Controls>();

        spriterenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!positionUpdated && movementMode && ((input.MoveInput().x > 0.5 || input.MoveInput().y > 0.5) || (input.MoveInput().x < -0.5 || input.MoveInput().y < -0.5)))
        {
            positionUpdated = true;

            /*Vector2 direction = new Vector2(input.MoveInput().x, input.MoveInput().y).normalized;
            Transform goTo = FindNearestTransformInDirection(direction);
            if (goTo != null) transform.position = goTo.position;
            else Debug.Log("No Transform available");*/

            Transform goTo =  UseOptions(input.MoveInput().x, input.MoveInput().y);
            if (goTo != null)
            {
                #region Teleport Audio
                if (sfx_teleportInstance.isValid())
                {
                    sfx_teleportInstance.start();
                }
                #endregion

                transform.position = goTo.position;
                currentBusStop = goTo;
                PickOptions();
            }
            else Debug.Log("No Transform available");
        }
        else if (positionUpdated && (input.MoveInput().x < 0.4 && input.MoveInput().y < 0.4) && (input.MoveInput().x > -0.4 && input.MoveInput().y > -0.4))
        {
            positionUpdated = false;
        }

        if (optionNorth != null && movementMode)
        {
            AdjustPosition(northArrow.transform, optionNorth.transform.position);
            PointAt(northArrow.transform, optionNorth.transform.position);
            northArrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else northArrow.GetComponent<SpriteRenderer>().enabled = false;
        if (optionEast != null && movementMode)
        {
            AdjustPosition(eastArrow.transform, optionEast.transform.position);
            PointAt(eastArrow.transform, optionEast.transform.position);
            eastArrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else eastArrow.GetComponent<SpriteRenderer>().enabled = false;
        if (optionSouth != null && movementMode)
        {
            AdjustPosition(southArrow.transform, optionSouth.transform.position);
            PointAt(southArrow.transform, optionSouth.transform.position);
            southArrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else southArrow.GetComponent<SpriteRenderer>().enabled = false;
        if (optionWest != null && movementMode)
        {
            AdjustPosition(westArrow.transform, optionWest.transform.position);
            PointAt(westArrow.transform, optionWest.transform.position);
            westArrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else westArrow.GetComponent<SpriteRenderer>().enabled = false;

    }

    private void LateUpdate()
    {
        if (!lateStart && RoomTracker.current_room != null)
        {
            Debug.Log("First bus stops assigned");
            lateStart = true;

            busStops = RoomTracker.current_room.mindBusStops;
            transform.position = busStops[0].position;

            currentBusStop = busStops[0];
        }

        if (switcher.activeCharacter == 1 || telekinesis.active)
        {
            movementMode = false;
        }
        else if (input.OnJumpPressed() && RoomTracker.current_room.mindBusStops.Count > 1)
        {
            Debug.Log("More than one bus stop");
            movementMode = !movementMode;
            if(!movementMode)
                telekinesis.ActivateTelekinesis();

            PickOptions();
        }
        else if (input.OnJumpPressed())
        {
            Debug.Log("Only one bus stop");
        }

        anim = GetComponentInChildren<Animator>();
    }

    private Transform FindNearestTransformInDirection(Vector2 direction)
    {
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;
        Vector3 startPosition = transform.position;

        foreach (Transform target in busStops)
        {
            Vector3 toTarget = target.position - startPosition;
            Vector2 toTarget2D = new Vector2(toTarget.x, toTarget.z); // Assuming movement in XZ plane

            // Check if the target is in the same direction
            if (Vector2.Dot(toTarget2D, direction) > 0.9f) // Threshold to account for slight angle deviations
            {
                float distance = toTarget.sqrMagnitude; // Using squared magnitude for performance
                if (distance < nearestDistance && target != currentBusStop)
                {
                    nearestDistance = distance;
                    nearestTarget = target;
                }
            }
        }

        if(nearestTarget != null) currentBusStop = nearestTarget;
        return nearestTarget;
    }

    private void PickOptions()
    {
        optionNorth = null;
        optionEast = null;
        optionWest = null;
        optionSouth = null;

        float northDistance = float.MaxValue;
        float sotuthDistance = float.MaxValue;
        float westDistacnce = float.MaxValue;
        float eastDistacnce = float.MaxValue;

        foreach (Transform target in busStops)
        {
            if(target != currentBusStop)
            {
                if (IsTransformBetween(transform, target.position, 0, 45f))
                {
                    float distanceTesting = (target.position - transform.position).sqrMagnitude;
                    if (distanceTesting < northDistance)
                    {
                        northDistance = distanceTesting;
                        optionNorth = target;
                        anim.SetTrigger("Teleporting");
                    }
                }
                else if (IsTransformBetween(transform, target.position, 45f, 135f) && transform.position.x > target.position.x)
                {
                    float distanceTesting = (target.position - transform.position).sqrMagnitude;
                    if (distanceTesting < westDistacnce)
                    {
                        westDistacnce = distanceTesting;
                        optionWest = target;

                        spriterenderer.flipX = true;
                        anim.SetTrigger("Teleporting");
                    }
                }
                else if (IsTransformBetween(transform, target.position, 45f, 135f) && transform.position.x < target.position.x)
                {
                    float distanceTesting = (target.position - transform.position).sqrMagnitude;
                    if (distanceTesting < eastDistacnce)
                    {
                        eastDistacnce = distanceTesting;
                        optionEast = target;

                        spriterenderer.flipX = false;
                        anim.SetTrigger("Teleporting");
                    }
                }
                else if(IsTransformBetween(transform, target.position, 135f, 180))
                {
                    float distanceTesting = (target.position - transform.position).sqrMagnitude;
                    if (distanceTesting < sotuthDistance)
                    {
                        sotuthDistance = distanceTesting;
                        optionSouth = target;
                        anim.SetTrigger("Teleporting");
                    }
                } 
                
            }
        }
    }

    private Transform UseOptions(float x, float y)
    {
        if (x > 0.5) return optionEast;
        else if (x < -0.5) return optionWest;
        else if (y > 0.5) return optionNorth;
        else return optionSouth;
    }

    private bool IsTransformBetween(Transform reference, Vector3 targetPosition, float minAngle, float maxAngle)
    {
        Vector3 toTarget = (targetPosition - reference.position).normalized; // Direction vector to the target
        float angle = Vector3.Angle(Vector3.up, toTarget); // Angle between upward vector and direction
        Debug.Log(angle);

        return angle >= minAngle && angle <= maxAngle;
    }

    public void UpdateCurrentStop()
    {
        currentBusStop = busStops[0];
    }

    void AdjustPosition(Transform arrowTransform, Vector2 targetPosition)
    {
        // Get direction from axisPoint to targetPosition
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Calculate the correct position at fixed radius
        Vector2 desiredPosition = (Vector2)transform.position + direction * orbitRadius;

        // Move the GameObject to the correct position if needed
        if ((Vector2)transform.position != desiredPosition)
        {
            arrowTransform.position = desiredPosition;
        }
    }

    void PointAt(Transform arrowTransform, Vector2 target)
    {
        Vector2 direction = (target - (Vector2)arrowTransform.position).normalized;
        float angleToTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowTransform.rotation = Quaternion.Euler(0, 0, angleToTarget);
    }

}
