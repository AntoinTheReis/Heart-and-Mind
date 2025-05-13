using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitMenuing : MonoBehaviour
{

    private bool menuMementosButton;
    public TextMeshProUGUI whiteMementos;
    public TextMeshProUGUI blackMementos;

    public TextMeshProUGUI foregroundNumber;
    public TextMeshProUGUI backgroundNumber;

    public Animator denyPopUp;

    private bool allcoinsGotten;

    TransitCoin coinManager;

    // Start is called before the first frame update
    void Start()
    {
        coinManager = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<TransitCoin>();

        if(gameObject.name == "Mementos Button")
        {
            menuMementosButton = true;

            if (coinManager.GetAcmountOfCoinsFoundButNotNecessarilyGotten() == 7)
            {
                if (!coinManager.AllCoinsGotten())
                {
                    int numba = 7 - coinManager.GetAcmountOfCoinsFoundButNotNecessarilyGotten();

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
}
