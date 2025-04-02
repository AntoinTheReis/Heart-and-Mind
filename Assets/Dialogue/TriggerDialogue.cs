using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TriggerDialogue : MonoBehaviour
{
    [Tooltip("Name of the yarn file you want to run when player enters this box")] public string startNode;
    private bool triggered = false;
    private DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && !dialogueRunner.IsDialogueRunning && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Starting Dialogue @: " + startNode);
            triggered = true;
            dialogueRunner.StartDialogue(startNode);
        }
    }

}
