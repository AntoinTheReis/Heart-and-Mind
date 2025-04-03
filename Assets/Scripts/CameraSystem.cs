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
using Yarn.Unity;

/*
 * MAURICIO: Camera script to go on main camera - game should not have multiple cameras 
 */

public class CameraSystem : MonoBehaviour
{
    public Switcher characterSwitcher;
    [SerializeField] private GameObject target;

    [Header("Camera Movement Settings")]
    [SerializeField] float followSpeed = 5f;

    [SerializeField] private float zoomSpeed = 2f;
    //[SerializeField] float maxDistance = 8f;
    [SerializeField] float timeToSetFollowSpawn = 0.2f;
    [SerializeField] float characterMaterialization = 0.4f;
    
    [Header("Screen Shake Settings")]
    [Tooltip("Number of seconds until a call of screenShake() ends")] [SerializeField] float shakeDuration = 0.2f;
    [Tooltip("The magnitude of each shake")] [SerializeField] float shakeAmount = 0.2f;
    [Tooltip("Repeat shake every n seconds. (The lower, the faster)")] [SerializeField] float shakeRate = 0.2f;
    
    
    private SpriteRenderer followerSprite;

    [Tooltip("Determines if the camera is bounded to the current room or moves to the target freely")]
    bool bounded = true;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI roomNameUI;

    Camera cam;
    float half_height;
    float half_width;
    private Room previousRoom;
    private Room lastActualRoom;
    
    DialogueRunner dialogueRunner;

    private void Start()
    {
        cam = GetComponent<Camera>();
        half_height = cam.orthographicSize;
        half_width = cam.aspect * half_height;
        lastActualRoom = RoomTracker.current_room;
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        lastActualRoom = RoomTracker.current_room;
    }
    
    #region Yarnspinner Commands
    /*
     * Custom yarnspinner command, written in yarn files as so:
     * <<pan Camera GameObject>>
     * This command for exmaple will change the camera's target to any given GameObject, panning the camera over to it
     */
    [YarnCommand("pan")]
    public void changeTarget(GameObject new_target)
    {
        if(new_target == null) Debug.LogError("Chosen target is null!");
        target = new_target;
    }
    
    /*
     * Custom yarnspinner command, written in yarn files as so:
     * <<shake Camera>>
     * Will shake the camera based on variables determined in CameraSystem
     */
    [YarnCommand("shake")]
    public void screenShake()
    {
        startShaking();
        StartCoroutine("StopShaking");
    }

    [YarnCommand("startShaking")]
    public void startShaking()
    {
        InvokeRepeating("InduceRandomOffset", 0f, shakeRate);
    }
    
    [YarnCommand("stopShaking")]
    public IEnumerator StopShaking()
    {
        yield return new WaitForSeconds(shakeDuration);
        CancelInvoke("InduceRandomOffset");
        yield break;
    }
    private void InduceRandomOffset()
    {
        transform.position += (Vector3)Random.insideUnitCircle * shakeAmount;
    }
    
    /*
     * Custom yarnspinner command, written in yarn files as so:
     * <<size Camera 7>>
     * Will change the camera's size to given size, camera size gets reset when dialogue ends
     */
    [YarnCommand("size")]
    public void ChangeCameraSize(float new_size)
    {
        StartCoroutine(LerpCamSize(new_size));
    }
    #endregion
    
    private void Update()
    {
        //Determining Target:
        //If not in dialogue
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            //if character switcher exists (relevant for acts 1 and 3)
            if(characterSwitcher != null)
            {
                changeTarget(characterSwitcher.activeCharacter == 2 ? characterSwitcher.mindObject : characterSwitcher.heartObject);
            }
            else
            {
                //Default to first found player obj
                changeTarget(GameObject.FindGameObjectWithTag("Player"));
            }
        }
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

            //On room chance
            if (RoomTracker.current_room != null && RoomTracker.current_room != lastActualRoom)
            {
                lastActualRoom = RoomTracker.current_room;
            }
            
            if (RoomTracker.current_room != null)
            {
                room_pos = RoomTracker.current_room.transform.position;
                room_size = new Vector2(RoomTracker.current_room.room_width / 2, RoomTracker.current_room.room_height / 2);
            }
            else if (lastActualRoom != null)
            {
                room_pos = lastActualRoom.transform.position;
                room_size = new Vector2(lastActualRoom.room_width / 2, lastActualRoom.room_height/ 2);
            }
            else
            {
                room_pos = transform.position;
                room_size = new Vector2(40, 40);
            }
            
            
            //camera resizing
            //if(camera gets resized){}
                half_height = cam.orthographicSize;
                half_width = cam.aspect * half_height;
            
            
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

    IEnumerator LerpCamSize(float new_size)
    {
        float elapsedTime = 0f;
        while (Mathf.Abs(cam.orthographicSize - new_size) > 0.01f) //Camera can only take a second to lerp, otherwise fucntion gets cancelled (without this, quickly switching rooms will cause opposing lerps)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, new_size, zoomSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 5)
            {
                cam.orthographicSize = new_size;
                yield break;
            }
            yield return null;
        }
        cam.orthographicSize = new_size;
    }

    public void resetCamSize()
    {
        ChangeCameraSize(RoomTracker.current_room.CamSizeInRoom);
    }

    public void changeRoomText()
    {
        if(roomNameUI != null && RoomTracker.current_room != null) roomNameUI.text = RoomTracker.current_room.name;
    }


    IEnumerator IdleFollow()  //Transporting other character into new Room
    {
        GameObject otherCharacter;

        /*if (characterSwitcher.activeCharacter == 1) otherCharacter = characterSwitcher.mindObject;
        else otherCharacter = characterSwitcher.heartObject;*/

        otherCharacter = characterSwitcher.mindObject;
        otherCharacter.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
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
}

