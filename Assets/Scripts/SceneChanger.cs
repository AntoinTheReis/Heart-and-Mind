using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneChanger : MonoBehaviour
{

    public RawImage curtain;
    public float curtainInTime;
    public float curtainOutTime;

    // Start is called before the first frame update
    void Start()
    {
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        if(tmp.a == 0) curtain.rectTransform.position = new Vector2(10000, 0);
    }

    IEnumerator NextSceneStart()
    {
        yield return new WaitForSecondsRealtime(curtainOutTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator MoveCurtainAway()
    {
        yield return new WaitForSecondsRealtime(curtainInTime);
        Debug.Log("Move curtain away");
        curtain.rectTransform.position = new Vector2(10000, 0);
    }

}
