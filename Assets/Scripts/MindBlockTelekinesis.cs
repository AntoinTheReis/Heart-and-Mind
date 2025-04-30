using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MindBlockTelekinesis : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriterenderer;

    [SerializeField] private GameObject SelectionOverlay;
    [SerializeField] private float blockSpeed;

    LinkedListNode<GameObject> selectedBlockNode = null;
    public Block selectedBlock = null;
    private Switcher switcher;

    public Controls input;
    public bool active = false;
    public float blockLingerTime;

    private void Start()
    {
        SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
        input = GetComponent<Controls>();
        anim = GetComponentInChildren<Animator>();
        switcher = GameObject.FindGameObjectWithTag("Switcher").GetComponent<Switcher>();
    }

    private void Update()
    {        
        if (switcher.activeCharacter == 2 && input.OnPrimaryPressed() && !active)
        {
            ActivateTelekinesis();
        }

        if (active)
        {
            SelectionOverlay.GetComponent<SpriteRenderer>().enabled = true;

            if (selectedBlockNode == null) selectedBlockNode = BlockTracker.BlocksOnScreen.First; //remember, the head of the list here CANNOT be null, so now we know that we have something not-null selected

            if (input.OnPrimaryPressed()) //Cycle through block list
            {
                if (selectedBlock != null) selectedBlock.DropBlock(); //Deselect currently selected block
                if (selectedBlockNode.Next != null)
                    selectedBlockNode = selectedBlockNode.Next; //goes to next node on the linked list of blocks on screen (next here can be null, so we check)
                else selectedBlockNode = BlockTracker.BlocksOnScreen.First;

            }

            //Moving selected block
            selectedBlock = selectedBlockNode.Value.GetComponent<Block>();
            selectedBlock.SelectBlock();
            Rigidbody2D blockRb = selectedBlock.GetComponent<Rigidbody2D>();
            Vector3 newBlockPos = selectedBlock.transform.position + (Vector3)input.MoveInput() * (blockSpeed * Time.deltaTime);
            if(input.MoveInput() != Vector2.zero) selectedBlock.OnMove();
            
            //make the selected block's velocity approach zero (to push back against any external forces to give the feeling of catching the block)
            blockRb.velocity = Vector2.Lerp(blockRb.velocity, Vector2.zero,  Time.deltaTime);
            //Fully sets it to zero if the player starts moving it in the opposite direction
            if (Vector2.Dot(blockRb.velocity, input.MoveInput()) < 0) blockRb.velocity = Vector2.zero;

            selectedBlock.transform.position = IsPositionOnScreen(newBlockPos) ? newBlockPos : selectedBlock.transform.position;
            SelectionOverlay.transform.position = selectedBlock.transform.position;

            
            //Leaving the active state
            if ((input.OnJumpPressed() && RoomTracker.current_room.mindBusStops.Count > 1) || selectedBlock.IsOffScreen() || GameObject.FindWithTag("Switcher").GetComponent<Switcher>().activeCharacter != 2)
            {
                Debug.Log("Make active = false");
                anim.SetTrigger("AbilityStop");
                selectedBlock.DropBlock();
                SelectionOverlay.GetComponent<SpriteRenderer>().enabled = false;
                active = false;
            }
        }
        anim = GetComponentInChildren<Animator>();
    }


    public void ActivateTelekinesis()
    {
        Debug.Log("Activate Telekinesis called");
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

            //Select the first block on activation
            if (BlockTracker.lastBlockSelected == null || BlockTracker.lastBlockSelected.IsOffScreen())
            {
                Debug.Log("Last block selected was: " + BlockTracker.lastBlockSelected);
                if(BlockTracker.lastBlockSelected != null) Debug.Log("Last block was offscreen is: " + BlockTracker.lastBlockSelected.IsOffScreen());

                selectedBlockNode = BlockTracker.BlocksOnScreen.First;
                BlockTracker.lastBlockSelected = selectedBlockNode.Value.GetComponent<Block>();
            }
            else
            {
                LinkedList<GameObject> newList = new LinkedList<GameObject>();
                newList.AddLast(BlockTracker.lastBlockSelected.gameObject);

                selectedBlockNode = newList.First;

            }

            anim.SetTrigger("Ability");
        }
    }

    

    public bool IsPositionOnScreen(Vector3 position)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    void ReplaceLinkedListNodeValue(LinkedList<GameObject> linkedList, LinkedListNode<GameObject> node, GameObject newValue)
    {
        if (node == null) return;

        // Create a new node and insert it after the current node
        LinkedListNode<GameObject> newNode = new LinkedListNode<GameObject>(newValue);
        linkedList.AddAfter(node, newNode);

        // Remove the old node
        linkedList.Remove(node);
    }


}
