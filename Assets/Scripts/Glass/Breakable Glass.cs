using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour
{

    [SerializeField] GameObject brokenPrefab;
    [SerializeField] Collider2D thisCollider;
    public GameObject heart;
    public float dashGlassWait = 0.1f;

    private bool dashingIn = false;
    private float originalDistance;
    private GameObject playerTarget;
    private bool breaking = false;

    #region Audio
    public FMODUnity.EventReference glassBreak;
    FMOD.Studio.EventInstance sfx_glassBreakInstance;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Audio EventInstances
        sfx_glassBreakInstance = FMODUnity.RuntimeManager.CreateInstance(glassBreak);
        #endregion

        for (int i = 0; i < 2; i++)
        {
            if(GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Movement>() != null)
            {
                heart = GameObject.FindGameObjectsWithTag("Player")[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(heart.GetComponent<Movement>().isDashing) thisCollider.enabled = false;
        else thisCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*thisCollider.enabled = false;
        if((collision.gameObject.tag != "Player" || !collision.gameObject.GetComponent<Movement>().isDashing)) thisCollider.enabled = true;*/
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<Movement>().isDashing)
        {
            breaking = true;
            Instantiate(brokenPrefab, transform.position, transform.rotation);
            Debug.Log("Glass break 1");
            sfx_glassBreakInstance.start();
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<Movement>().isDashing && !dashingIn)
        {
            Debug.Log("Dashing While Inside");
            dashingIn = true;
            originalDistance = Vector2.Distance(gameObject.transform.position, collision.gameObject.transform.position);
            playerTarget = collision.gameObject;

            Instantiate(brokenPrefab, transform.position, transform.rotation);
            Debug.Log("Glass break 2");
            sfx_glassBreakInstance.start();
            Destroy(gameObject);
        }
    }

    IEnumerator CheckForDashInGlass()
    {
        yield return new WaitForSecondsRealtime(dashGlassWait);
        if (dashingIn)
        {
            
        }
    }

}
