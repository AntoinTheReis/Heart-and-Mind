using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switcher : MonoBehaviour
{
    Controls input;

    [Tooltip("1 is heart and 2 is mind")]
    public int activeCharacter; 

    public GameObject mindObject;
    public GameObject heartObject;

    private Movement heartMovement;
    private MindMovement mindMovement;

    private MindBlockTelekinesis mindBlockMechanic;

    private Transform cam;
    private bool movingAndLooking = false;
    Vector2 mindpos;

    public float telekinesisWaitTime = 0.55f;

    private void Awake()
    {
        input = GetComponent<Controls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mindBlockMechanic = mindObject.GetComponent<MindBlockTelekinesis>();

        heartMovement = heartObject.GetComponent<Movement>();
        mindMovement = mindObject.GetComponent<MindMovement>();

        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        if (heartMovement.turnedOn)  //Checking what character is active and making sure one is active and one is inactive
        {
            activeCharacter = 1;    //Mind character takes priority
            //heartMovement.turnedOn = false;
        }
        else
        {
            activeCharacter = 1;
            //heartMovement.turnedOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (input.OnInteractPressed())
        {
            if(activeCharacter == 1)
            {
                mindpos = mindObject.transform.position;

                activeCharacter = 2;
                //mindBlockMechanic.enabled = true;
                mindMovement.turnedOn = true;
                heartMovement.turnedOn = false;

                //movingAndLooking = true;
                StartCoroutine(MoveAndLook());
            }
            else
            {
                activeCharacter = 1;
                movingAndLooking= false;

                //mindBlockMechanic.enabled = false;
                mindMovement.turnedOn = false;
                heartMovement.turnedOn = true;
            }
        }

        /*if(movingAndLooking && input.OnJumpPressed())
        {
            movingAndLooking = false;
        }
        else if (movingAndLooking && !mindObject.GetComponent<MindBlockTelekinesis>().active && !mindObject.GetComponent<MindTeleporting>().movementMode)
        {
            Debug.Log("Checking for cube");
            mindObject.GetComponent<MindBlockTelekinesis>().ActivateTelekinesis();
        }*/
    }

    IEnumerator MoveAndLook()
    {
        Debug.Log("Moving and looking");
        yield return new WaitForSecondsRealtime(telekinesisWaitTime);
        Debug.Log(!mindObject.GetComponent<MindBlockTelekinesis>().active);
        if(!mindObject.GetComponent<MindBlockTelekinesis>().active) mindObject.GetComponent<MindBlockTelekinesis>().ActivateTelekinesis();
    }

}
