using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// if the room is currently not the "selected" one (the one the player is in), draw a rectangle to cover it.
// addition: we could make this juicy and appear / disappear in fun ways


public class Room : MonoBehaviour
{
    // room width and height
    public int room_width;
    public int room_height;
    public bool room_is_selected;
    public Transform checkpoint;
    bool room_is_selected_last_state = false;
    [HideInInspector] public Transform cover;
    public List<Transform> mindBusStops;

    // Adding a trigger collider for checking if target is inside
    BoxCollider2D room_bounds;

    // draw an outline at these sizes, for debug reasons:
    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireCube(transform.position, new Vector3(room_width, room_height, 0));
    }
    

    void Start()
    {
        cover = transform.Find("cover");
        cover.localScale = new Vector3(room_width, room_height, 0);

        // Set bounds of collider
        room_bounds = GetComponent<BoxCollider2D>();
        room_bounds.size = new Vector2(room_width-1, room_height-1);
    }

    void Update()
    {
        if (room_is_selected != room_is_selected_last_state)
        {
            room_is_selected_last_state = room_is_selected;
            if (RoomTracker.current_room == null)
            {
                Debug.Log("There is no current room");
                coverFadeOut();
            }
            else if (!room_is_selected)
            {
                coverFadeIn();
            }
            else
            {
                coverFadeOut();
            }
        }
    }
    void coverFadeIn()
    {
        print("fade in started");
        cover.gameObject.SetActive(true);
    }
    void coverFadeOut()
    {
        print("fade out started");
        cover.gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform == RoomTracker.target)
        {
            room_is_selected = true;
            RoomTracker.current_room = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.transform == RoomTracker.target )
        {
            room_is_selected = false;
            RoomTracker.current_room = null;
            StartCoroutine(NullRoomTimer(0.1f));
        }
    }

    IEnumerator NullRoomTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (RoomTracker.current_room == this && !room_is_selected) RoomTracker.current_room = null;
    }

}
