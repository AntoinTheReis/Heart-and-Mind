using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * MAURICIO: Camera script to go on main camera - game should not have multiple cameras 
 */

public class CameraSystem : MonoBehaviour
{
    public Switcher characterSwitcher;
    private GameObject target;

    [Header("Camera Movement Settings")]
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float maxDistance = 8f;
    [SerializeField] float timeToSetFollowSpawn = 0.2f;
    [SerializeField] float characterMaterialization = 0.4f;
    private SpriteRenderer followerSprite;

    [Tooltip("Determines if the camera is bounded to the current room or moves to the target freely")]
    [SerializeField] bool bounded = true;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI roomNameUI;

    Camera cam;
    float half_height;
    float half_width;
    private Room previousRoom;
    private Room lastActualRoom;

    private void Start()
    {
        cam = GetComponent<Camera>();
        half_height = cam.orthographicSize;
        half_width = cam.aspect * half_height;
        lastActualRoom = RoomTracker.current_room;
    }
    private void Update()
    {
        if (characterSwitcher.activeCharacter == 1) target = characterSwitcher.heartObject;
        else target = characterSwitcher.mindObject;

        if (target == null)
        {
            Debug.LogError("No Camera target found");
            return;
        }
        Vector3 target_position = target.transform.position;

        #region Clamping Camera's Target to Room Bounds
        if (bounded)
        {
            RoomTracker.target = target.transform;
            //if (RoomTracker.current_room == null) goto BoundFailed;
            if (RoomTracker.current_room != previousRoom && RoomTracker.current_room != null && RoomTracker.current_room != lastActualRoom) StartCoroutine("IdleFollow");  //Moves the other character when the room changes

            Vector3 room_pos;
            Vector2 room_size;

            if (RoomTracker.current_room != null && RoomTracker.current_room != lastActualRoom)
            {
                lastActualRoom = RoomTracker.current_room;
            }


            if (RoomTracker.current_room != null)
            {
                room_pos = RoomTracker.current_room.transform.position;
                room_size = new Vector2(RoomTracker.current_room.room_width / 2, RoomTracker.current_room.room_height / 2);
            }
            else
            {
                room_pos = lastActualRoom.transform.position;
                room_size = new Vector2(lastActualRoom.room_width / 2, lastActualRoom.room_height/ 2);
            }

            //  camera clamping
            //horizontal max
            if (target_position.x + half_width > room_pos.x + room_size.x) { target_position.x = room_pos.x + room_size.x - half_width; }
            //horizontal min
            if (target_position.x - half_width < room_pos.x - room_size.x) { target_position.x = room_pos.x - room_size.x + half_width; }
            //vertical max
            if(target_position.y + half_height > room_pos.y + room_size.y) { target_position.y = room_pos.y + room_size.y - half_height; }
            //vertical min
            if(target_position.y - half_height < room_pos.y - room_size.y) { target_position.y = room_pos.y - room_size.y + half_height; }


            previousRoom = RoomTracker.current_room;
        }
    //BoundFailed:  //goto lable for when bound fails, unbinding the camera
        //Debug.Log("Bound failed");
        #endregion

        #region Moving Camera To Target 

        //  Move toward target
        //transform.position = Vector3.SmoothDamp(transform.position, target_position, ref velocity, 1f, followSpeed);
        transform.position = Vector3.Lerp(transform.position, target_position, followSpeed * Time.deltaTime);

        /* TODO: super buggy needs fixing
        //Enforce max distance
        if(Vector3.Distance(transform.position, target_position) > maxDistance)
        {
            Vector3 difference = transform.position - target_position;
            Vector3 difference = transform.position - target_position;
            difference.Normalize();
            difference *= maxDistance;
            transform.position = target_position + difference;
        }
        */
        
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        
        #endregion
        


    }

    private void LateUpdate()
    {
        #region UI
        if(roomNameUI != null) roomNameUI.text = RoomTracker.current_room != null ? RoomTracker.current_room.name : "unknown";
        #endregion

    }


    IEnumerator IdleFollow()  //Transporting other character into new Room
    {
        GameObject otherCharacter;

        /*if (characterSwitcher.activeCharacter == 1) otherCharacter = characterSwitcher.mindObject;
        else otherCharacter = characterSwitcher.heartObject;*/

        otherCharacter = characterSwitcher.mindObject;
        followerSprite = otherCharacter.GetComponentInChildren<SpriteRenderer>();

        Color tmp = followerSprite.color;   //Setting color to transparent
        tmp.a = 0f;
        followerSprite.color = tmp;

        yield return new WaitForSecondsRealtime(timeToSetFollowSpawn);
        Vector2 spawnPos = target.transform.position;

        otherCharacter.transform.position = RoomTracker.current_room.mindBusStops[0].position;
        otherCharacter.GetComponent<MindTeleporting>().busStops = RoomTracker.current_room.mindBusStops;
        otherCharacter.GetComponent<MindTeleporting>();
        DOVirtual.Float(0, 1, characterMaterialization, SpriteAlpha);

    }

    private void SpriteAlpha(float alpha)
    {
        Color tmp = followerSprite.color;
        tmp.a = alpha;
        followerSprite.color = tmp;
    }

    //TODO: CAMERA SHAKE
    public void ShakeCamera() { Debug.Log("Camera Shake (unimplemented)"); }
}

