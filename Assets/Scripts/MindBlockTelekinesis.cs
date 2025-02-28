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
    private bool active = false;
    private void Start()
    {
        SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
        input = GetComponent<Controls>();
    }

    private void Update()
    {
        GetComponent<MindMovement>().canMove = !active;
        
        if (input.OnPrimaryPressed() && !active)
        {
            if (BlockTracker.BlocksOnScreen.First == null)
            {
                //TODO: play some informative sound effect that there are no blocks on screen to select
            }
            else
            {
                //Sorting the linked list based on distance from the player
                if (BlockTracker.BlocksOnScreen.Count > 1)
                {
                     BlockTracker.SortByDistance(ref BlockTracker.BlocksOnScreen, transform.position);
                }
                active = true; //therefore we can assume that we are only active when at least one block is on screen
            }
        }

        if (active)
        {
            SelectionOverlay.GetComponent<SpriteRenderer>().enabled = true;
            
            if (selectedBlockNode == null) selectedBlockNode = BlockTracker.BlocksOnScreen.First; //remember, the head of the list here CANNOT be null, so now we know that we have something not-null selected
            if (input.OnPrimaryPressed()) //Cycling through blocks on screen:
            {
                if(selectedBlock != null) selectedBlock.GetComponent<Rigidbody2D>().gravityScale = 1; //turn back on gravity for the old selected block
                if (selectedBlockNode.Next != null) 
                    selectedBlockNode = selectedBlockNode.Next; //goes to next node on the linked list of blocks on screen (next here can be null, so we check)
                else selectedBlockNode = BlockTracker.BlocksOnScreen.First;
            }
            
            //Moving selected block
            selectedBlock = selectedBlockNode.Value;
            Rigidbody2D blockRb = selectedBlock.GetComponent<Rigidbody2D>();
            blockRb.gravityScale = 0;
            blockRb.angularDrag = 1;

            Vector3 newBlockPos = selectedBlock.transform.position + (Vector3)input.MoveInput() * (4 * Time.deltaTime);
            //make the selected block's velocity approach zero (to push back against any external forces to give the feeling of catching the block)
            blockRb.velocity = Vector2.Lerp(blockRb.velocity, Vector2.zero,  Time.deltaTime);
            //Fully sets it to zero if the player starts moving it in the opposite direction
            if (Vector2.Dot(blockRb.velocity, input.MoveInput()) < 0) blockRb.velocity = Vector2.zero;
            //probably need some checks here for bugs
            selectedBlock.transform.position = newBlockPos;
            
            SelectionOverlay.transform.position = selectedBlock.transform.position;
            
            //Leaving the active state with the jump key:
            if (input.OnJumpPressed() || selectedBlock.GetComponent<Block>().IsOffScreen())
            {
                selectedBlock.GetComponent<Rigidbody2D>().gravityScale = 1;
                selectedBlock.GetComponent<Rigidbody2D>().angularDrag = 0.05f;
                SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
                active = false;
            }
        }
    }

}
