
using System.Collections;
using UnityEngine;

public class MindBlockSpawning : MonoBehaviour
{
   [SerializeField] private GameObject BlockPrefab;
   private GameObject spawnedBlock;
   //[SerializeField] private float minSpawnDistance;
   [SerializeField] private float maxBlockSpawnDistance;
   [SerializeField] private float blockSlideStep;
   [Tooltip("How many milliseconds to hold button to activate spawn coroutine")][SerializeField] private int spawnDelay;
   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(transform.position, maxBlockSpawnDistance);
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
      Vector3 newBlockPos = spawnedBlock.transform.position + (Vector3)input.MoveInput() * (4 * Time.deltaTime);
      if (Vector3.Distance(transform.position, newBlockPos) < maxBlockSpawnDistance)
      {
         spawnedBlock.transform.position = newBlockPos;
      }
      else
      {
         //TODO Add some juice for not being able to move the block that far
         Debug.Log("Can't move block");
         spawnedBlock.transform.position = Vector3.MoveTowards(spawnedBlock.transform.position, transform.position, blockSlideStep);
      }
      
      //Releasing block
      if (!input.PrimaryPressed())
      {
         blockRigidbody.gravityScale = 1;
         GetComponent<MindMovement>().canMove = true;
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
      if (input.MoveInput().y < 0) //if player is holding down
      {
         spawnedBlock.transform.position = transform.position - Vector3.up * 1;
      }
      else
      {
         spawnedBlock.transform.position = transform.position + Vector3.up * 1;
      }
      spawnedBlock.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
      spawnedBlock.GetComponent<Rigidbody2D>().gravityScale = 0;
      GetComponent<MindMovement>().canMove = false;
   }


   
}
