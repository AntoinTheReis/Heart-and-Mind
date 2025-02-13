using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MindBlockSpawning : MonoBehaviour
{
   [SerializeField] private GameObject BlockPrefab;
   private GameObject spawnedBlock;
   [SerializeField] private float minSpawnDistance;
   [SerializeField] private float maxSpawnDistance;
   [Tooltip("How many milliseconds to hold button to activate spawn coroutine")][SerializeField] private int spawnDelay;
   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position, minSpawnDistance);
      Gizmos.DrawWireSphere(transform.position, maxSpawnDistance);
   }

   private Controls input;
   private void Start()
   {
      input = GetComponent<Controls>();
   }

   private void Update()
   {
      if (input.OnPrimaryPressed())
      {
         StartCoroutine(SpawnDelay());
      }
   }

   IEnumerator SpawnDelay()
   {
      for (int i = 0; i < spawnDelay * 1000 * Time.deltaTime; i++) //multiply spawnDelay by 1000 to get it in seconds, then multiply by deltaTime to get how many frames
      {
         if (!input.PrimaryPressed()) yield break; //checks if input is released each frame, if so, stop (not fully necessary, spawnDelay can be 0)
         yield return null;
      }
      spawnBlockFunc();
      StartCoroutine(MoveBlock());
      yield break;
   }
   IEnumerator MoveBlock()
   {
      Rigidbody2D blockRigidbody = spawnedBlock.GetComponent<Rigidbody2D>();

      keepMovingBlock: //omg i made my own update function inside a coroutine thats crazy
      //Moving block position
      spawnedBlock.transform.position += (Vector3)input.MoveInput() * (4 * Time.deltaTime);
      
      //Releasing block
      if (!input.PrimaryPressed())
      {
         blockRigidbody.gravityScale = 1;
         yield break;
      }
      else
      {
         yield return null;
         goto keepMovingBlock;
      }
   }

   void spawnBlockFunc()
   {
      if(spawnedBlock != null) Destroy(spawnedBlock); //destroy already spawned block
      spawnedBlock = Instantiate(BlockPrefab, transform.position, Quaternion.identity); //makes new one
      //resets position and rigidbody parameters
      spawnedBlock.transform.position = transform.position + Vector3.up * minSpawnDistance;
      spawnedBlock.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
      spawnedBlock.GetComponent<Rigidbody2D>().gravityScale = 0;
   }


   
}
