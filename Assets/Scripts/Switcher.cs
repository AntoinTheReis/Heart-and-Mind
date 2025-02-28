using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switcher : MonoBehaviour
{
    Controls input;

    //1 is heart and 2 is mind
    public int activeCharacter = 1; 

    public GameObject mindObject;
    public GameObject heartObject;

    private Movement heartMovement;
    private MindMovement mindMovement;

    private MindBlockTelekinesis mindBlockMechanic;


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

        if (mindMovement.turnedOn)  //Checking what character is active and making sure one is active and one is inactive
        {
            activeCharacter = 2;    //Mind character takes priority
            heartMovement.turnedOn = false;
        }
        else
        {
            activeCharacter = 1;
            heartMovement.turnedOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (input.OnInteractPressed())
        {
            if(activeCharacter == 1)
            {
                activeCharacter = 2;
                mindBlockMechanic.enabled = true;
                mindMovement.turnedOn = true;
                heartMovement.turnedOn = false;
            }
            else
            {
                activeCharacter = 1;
                mindBlockMechanic.enabled = false;
                mindMovement.turnedOn = false;
                heartMovement.turnedOn = true;
            }
        }
    }
}
