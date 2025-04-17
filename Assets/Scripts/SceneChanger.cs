using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;


public class SceneChanger : MonoBehaviour
{

    public RawImage curtain;
    public float curtainInTime;
    public float curtainOutTime;
    PlayerInput input = new PlayerInput();
    private SceneChanger changer;

    // Start is called before the first frame update
    void Start()
    {
        FadeIn();
        input.DeactivateInput();
        changer = GameObject.FindGameObjectWithTag("SceneChanger").GetComponentInChildren<SceneChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [YarnCommand("NextScene")]
    public void NextScene()
    {
        FadeOut();
        StartCoroutine(NextSceneStart());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FadeIn()
    {
        DOVirtual.Float(1, 0, curtainInTime, AdjustCurtainAlpha);
        //StartCoroutine(MoveCurtainAway());
    }

    public void FadeOut()
    {
        curtain.rectTransform.position = new Vector2(960, 540);
        DOVirtual.Float(0, 1, curtainOutTime, AdjustCurtainAlpha);
    }

    private void AdjustCurtainAlpha(float a)
    {
        Color tmp = curtain.color;
        tmp.a = a;
        curtain.color = tmp;

        if(tmp.a == 0)
        {
            curtain.rectTransform.position = new Vector2(10000, 0);
            input.ActivateInput();
        }
    }

    IEnumerator NextSceneStart()
    {
        yield return new WaitForSecondsRealtime(curtainOutTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            changer.NextScene();
        }
    }

    IEnumerator MoveCurtainAway()
    {
        yield return new WaitForSecondsRealtime(curtainInTime);
        Debug.Log("Move curtain away");
        curtain.rectTransform.position = new Vector2(10000, 0);
    }

}
