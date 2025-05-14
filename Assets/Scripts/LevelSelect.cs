using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

    TransitCoin coinManager;

    public List<RawImage> minis = new List<RawImage>();

    public TextMeshProUGUI numberLeft;
    public TextMeshProUGUI numberLeftBackground;
    public TextMeshProUGUI left;
    public TextMeshProUGUI leftBackground;
    public TextMeshProUGUI allObtained;
    public TextMeshProUGUI allObtainedBackground;

    private bool completed = true;
    private int amountLeft = 0;
    SceneChanger sceneChanger;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("SceneChanger") != null) coinManager = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<TransitCoin>();
        if (GameObject.FindGameObjectWithTag("SceneChanger") != null) sceneChanger = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<SceneChanger>();


        if (gameObject.name == "Level Select" && coinManager.GetAcmountOfCoinsFoundButNotNecessarilyGotten() != 7)
        {
            gameObject.SetActive(false);
        }

        if (coinManager.GetAcmountOfCoinsFoundButNotNecessarilyGotten() == 7)
        {
            for (int i = 0; i < minis.Count; i++)
            {
                if (!coinManager.CheckIfObtained(i))
                {
                    amountLeft++;
                    completed = false;
                    minis[i].color = Color.gray;
                }
            }
        }
        else
        {
            completed = false;
        }

        if(SceneManager.GetActiveScene().buildIndex == 10)
        {
            if (completed)
            {
                numberLeft.enabled = false;
                numberLeftBackground.enabled = false;
                left.enabled = false;
                leftBackground.enabled = false;

                allObtained.enabled = true;
                allObtainedBackground.enabled = true;
            }
            else
            {
                numberLeft.enabled = true;
                numberLeftBackground.enabled = true;
                numberLeft.text = amountLeft.ToString();
                numberLeftBackground.text = amountLeft.ToString();
                left.enabled = true;
                leftBackground.enabled = true;

                allObtained.enabled = false;
                allObtainedBackground.enabled = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToLevelSelect()
    {
        SceneManager.LoadScene(10);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(1);
        sceneChanger.ComingFromLevelSelect();
    }

    public void LoadAct1()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadAct2()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadAct3()
    {
        SceneManager.LoadScene(7);
    }
}
