using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MindBlockTelekinesis : MonoBehaviour
{
    [SerializeField] private GameObject SelectionOverlay;

    LinkedListNode<GameObject> selectedBlockNode = null;
    GameObject selectedBlock = null;

    public Controls input;
    public bool active = false;

    private bool firstFrameOfTelekinesis = false;

    private void Start()
    {
        SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
        input = GetComponent<Controls>();
    }

    private void Update()
    {
        //GetComponent<MindMovement>().canMove = !active;
        
        if (input.OnPrimaryPressed() && !active)
        {
            ActivateTelekinesis();
        }

        if (active)
        {
            SelectionOverlay.GetComponent<SpriteRenderer>().enabled = true;
            
            if (selectedBlockNode == null) selectedBlockNode = BlockTracker.BlocksOnScreen.First; //remember, the head of the list here CANNOT be null, so now we know that we have something not-null selected
            if (firstFrameOfTelekinesis) //Cycling through blocks on screen:
            {
                firstFrameOfTelekinesis = false;
                if(selectedBlock != null) selectedBlock.GetComponent<Block>().DeselectBlock(); //turn back on gravity for the old selected block
                if (selectedBlockNode.Next != null) 
                    selectedBlockNode = selectedBlockNode.Next; //goes to next node on the linked list of blocks on screen (next here can be null, so we check)
                else selectedBlockNode = BlockTracker.BlocksOnScreen.First;
            }
            
            //Moving selected block
            selectedBlock = selectedBlockNode.Value;
            selectedBlock.GetComponent<Block>().SelectBlock();
            Rigidbody2D blockRb = selectedBlock.GetComponent<Rigidbody2D>();
            Vector3 newBlockPos = selectedBlock.transform.position + (Vector3)input.MoveInput() * (4 * Time.deltaTime);
            //make the selected block's velocity approach zero (to push back against any external forces to give the feeling of catching the block)
            blockRb.velocity = Vector2.Lerp(blockRb.velocity, Vector2.zero,  Time.deltaTime);
            //Fully sets it to zero if the player starts moving it in the opposite direction
            if (Vector2.Dot(blockRb.velocity, input.MoveInput()) < 0) blockRb.velocity = Vector2.zero;
            //probably need some checks here for bugs
            selectedBlock.transform.position = newBlockPos;
            
            SelectionOverlay.transform.position = selectedBlock.transform.position;
            
            //Leaving the active state with the jump key:
            if (input.OnJumpPressed() || selectedBlock.GetComponent<Block>().IsOffScreen() || GameObject.FindWithTag("Switcher").GetComponent<Switcher>().activeCharacter != 2)
            {
                Debug.Log("Make active = false");
                selectedBlock.GetComponent<Block>().DeselectBlock();
                SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
                active = false;
            }
        }
    }


    public void ActivateTelekinesis()
    {
        Debug.Log("Activate Telekinesis called");
        firstFrameOfTelekinesis = true;

        if (BlockTracker.BlocksOnScreen.First == null)
        {
            //TODO: play some informative sound effect that there are no blocks on screen to select
            Debug.Log("No blocks on screen");
        }
        else
        {
            //Sorting the linked list based on distance from the player
            if (BlockTracker.BlocksOnScreen.Count > 1)
            {
                BlockTracker.SortByDistance(ref BlockTracker.BlocksOnScreen, transform.position);
            }
            active = true; //therefore we can assume that we are only active when at least one block is on screen
            Debug.Log("There was a block on screen");
        }
    }
}
