using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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

    private GameObject cam;
    private Volume volume;
    private float shake = 0;
    public float shakeAmount;
    public float decreaseFactor;
    public float initialShakeAmount;
    public float blurInTime;
    private CameraSystem cameraSystem;
    // Start is called before the first frame update
    void Start()
    {
        FadeIn();
        input.DeactivateInput();
        changer = GameObject.FindGameObjectWithTag("SceneChanger").GetComponentInChildren<SceneChanger>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        volume = cam.GetComponentInChildren<Volume>();
        cameraSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shake > 0) 
        {
            cam.transform.localPosition = Random.insideUnitSphere * shakeAmount + cameraSystem.target_position;
            shake -= Time.deltaTime * decreaseFactor;

        } else
        {
            shake = 0.0f;
        }
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

    [YarnCommand("FadeIn")]
    public void FadeIn()
    {
        DOVirtual.Float(1, 0, curtainInTime, AdjustCurtainAlpha);
        //StartCoroutine(MoveCurtainAway());
    }


    [YarnCommand("FadeOut")]
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

    [YarnCommand("CutToBlack")]
    public void CutToBlack()
    {
        Color tmp = curtain.color;
        tmp.a = 1;
        curtain.color = tmp;

        curtain.rectTransform.position = new Vector2(960, 540);
    }

    [YarnCommand("CutFromBlack")]
    public void CutToLevel()
    {
        Color tmp = curtain.color;
        tmp.a = 0;
        curtain.color = tmp;

        curtain.rectTransform.position = new Vector2(10000, 0); ;
    }

    [YarnCommand("CamShake")]
    public void CamShake()
    {
        shake = initialShakeAmount;
    }

    [YarnCommand("AddBlur")]
    public void BlurIn()
    {
        DOVirtual.Float(0.8f, 1, blurInTime, AdjustBlur);
    }

    [YarnCommand("RemoveBlur")]
    public void BlurOut()
    {
        DOVirtual.Float(1, 0.8f, blurInTime, AdjustBlur);
    }

    public void AdjustBlur(float fd)
    {
        volume.weight = fd;
    }


}
