using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitMenuing : MonoBehaviour
{

    private bool menuMementosButton;
    public TextMeshProUGUI whiteMementos;
    public TextMeshProUGUI blackMementos;

    public TextMeshProUGUI foregroundNumber;
    public TextMeshProUGUI backgroundNumber;

    public Scrollbar scrollbar;

    public RectTransform chess;
    public RectTransform rose;
    public RectTransform sword;

    public float chessEndPoint;
    public float roseEndPoint;
    public float swordEndPoint;

    public float chessAdditionValue;
    public float roseAdditionValue;
    public float swordAdditionValue;

    public Animator denyPopUp;

    public Animator mementoButtons;

    private bool allcoinsGotten;

    TransitCoin coinManager;

    private bool showingText = false;
    public TextMeshProUGUI returnButton;

    private float mementoOnScreen = 0;
    // 0 is none
    // 1 is chess
    // 2 is rose
    // 3 is sword

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("SceneChanger") != null) coinManager = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<TransitCoin>();

        if (gameObject.name == "Mementos Button")
        {
            menuMementosButton = true;

            if (coinManager.GetAcmountOfCoinsFoundButNotNecessarilyGotten() == 7)
            {
                if (!coinManager.AllCoinsGotten())
                {
                    int numba = coinManager.AmountOfCoinsLeft();

                    foregroundNumber.text = numba.ToString();
                    backgroundNumber.text = numba.ToString();

                    whiteMementos.color = Color.gray;
                    allcoinsGotten = false;
                }
                else allcoinsGotten = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MementosMenu()
    {
        if (allcoinsGotten)
        {
            SceneManager.LoadScene(9);
        }
        else
        {
            denyPopUp.SetTrigger("Deny");
        }
    }

    public void MementosSceneMenu()
    {
        if (!showingText)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            showingText = false;
            if(mementoOnScreen == 2) DOVirtual.Float(0, -957, 0.84f, MoveRoseX);
            else if(mementoOnScreen == 3) DOVirtual.Float(0, -957, 0.84f, MoveSwordX);
            else if (mementoOnScreen == 1) DOVirtual.Float(0, -957, 0.84f, MoveChessX);
            mementoButtons.SetTrigger("GoIn");
            returnButton.text = "Main Menu";
        }
    }

    public void AdjustTextPositions()
    {
        chess.anchoredPosition = new Vector2(chess.anchoredPosition.x, 177 + (chessAdditionValue * scrollbar.value));
        sword.anchoredPosition = new Vector2(sword.anchoredPosition.x, 177 + (swordAdditionValue * scrollbar.value));
        rose.anchoredPosition = new Vector2(rose.anchoredPosition.x, 177 + (roseAdditionValue * scrollbar.value));
    }

    public void MoveInRose()
    {
        mementoButtons.SetTrigger("GoOut");
        DOVirtual.Float(-957, 0, 1.1f, MoveRoseX);
        mementoOnScreen = 2;
        showingText = true;

        returnButton.text = "Return";
    }

    public void MoveInSword()
    {
        mementoButtons.SetTrigger("GoOut");
        DOVirtual.Float(-957, 0, 1.1f, MoveSwordX);
        mementoOnScreen = 3;
        showingText = true;

        returnButton.text = "Return";
    }

    public void MoveInChess()
    {
        mementoButtons.SetTrigger("GoOut");
        DOVirtual.Float(-957, 0, 1.1f, MoveChessX);
        mementoOnScreen = 1;
        showingText = true;

        returnButton.text = "Return";
    }

    public void MoveRoseX(float x)
    {
        rose.anchoredPosition = new Vector2(x, rose.anchoredPosition.y);
    }
    public void MoveSwordX(float x)
    {
        sword.anchoredPosition = new Vector2(x, sword.anchoredPosition.y);
    }
    public void MoveChessX(float x)
    {
        chess.anchoredPosition = new Vector2(x, chess.anchoredPosition.y);
    }
}
