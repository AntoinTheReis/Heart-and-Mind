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
    private bool creditsFinished = false;
    // Start is called before the first frame update

    public static List <int> levelsVisited = new List <int>();

    private bool isCredits = false;
    private Animator creditsAnimator;

    private static bool firstGo = true;

    static bool comingFromLevelSelect = false;

    void Start()
    {
        FadeIn();
        input.DeactivateInput();
        changer = GameObject.FindGameObjectWithTag("SceneChanger").GetComponentInChildren<SceneChanger>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        volume = cam.GetComponentInChildren<Volume>();
        cameraSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraSystem>();

        Time.timeScale = 1.0f;

        if (firstGo)
        {
            PlayerPrefs.SetInt("CurrentScene", 1);
            firstGo = false;
        }

        if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            isCredits = true;
            creditsAnimator = GameObject.FindGameObjectWithTag("Credits").GetComponent<Animator>();
            PlayerPrefs.SetInt("CurrentScene", 1);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 10)
        {
            comingFromLevelSelect = false;
        }


        if (!levelsVisited.Contains(SceneManager.GetActiveScene().buildIndex))
        {
            levelsVisited.Add(SceneManager.GetActiveScene().buildIndex);
        }

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

        if(isCredits && creditsAnimator.GetCurrentAnimatorStateInfo(0).IsName("credit end") && !creditsFinished)
        {
            creditsFinished = true;
            FadeOut();
            StartCoroutine(returnToMenu());
        }
        
    }

    public void GoToScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            //this function is only called right now by the main menu button in the pause menu; thus we should save our current scene in player perfs
            //SceneManager.LoadScene(lastActualScene);
            PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);
        }
        SceneManager.LoadScene(sceneName);

    }


    [YarnCommand("NextScene")]
    public void NextScene()
    {
        //lastActualScene = SceneManager.GetActiveScene().buildIndex;
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

        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (!comingFromLevelSelect)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(activeSceneIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentScene"));
            }
        }
        else if (activeSceneIndex == 1 || activeSceneIndex == 2 || activeSceneIndex == 6 || activeSceneIndex == 7) SceneManager.LoadScene(10);
        else SceneManager.LoadScene(activeSceneIndex + 1);
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

    IEnumerator returnToMenu()
    {
        yield return new WaitForSecondsRealtime(curtainOutTime);
        SceneManager.LoadScene(0);
    }

    public void ComingFromLevelSelect()
    {
        comingFromLevelSelect = true;
    }

}
