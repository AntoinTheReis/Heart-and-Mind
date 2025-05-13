using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitCoin : MonoBehaviour
{

    static List<Vector3> allCoins;
    //x is the scene build number of the coin
    //y is the number of the coin in the scene 
    //z is whetgher the coin has been obtained or not (0 is no, 1 is yes)

    List<GameObject> coinsInLevel;

    static List<int> levelsVisited = new List<int>();

    SceneChanger sceneManager;

    void Start()
    {

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (GetComponent<SceneChanger>() != null)
        {
            sceneManager = GetComponent<SceneChanger>();
        }

        coinsInLevel = GameObject.FindGameObjectsWithTag("Coin").ToList();

        if (!levelsVisited.Contains(currentScene))
        {
            levelsVisited.Add(currentScene);

            for (int i = 0; i < coinsInLevel.Count; i++)
            {
                allCoins.Add(new Vector3(currentScene, i, 0));
            }
        }
        else
        {
            for (int i = 0; i < allCoins.Count; i++)
            {
                if (allCoins[i].x == currentScene && allCoins[i].y == 1)
                {
                    coinsInLevel[(int)allCoins[i].y].GetComponent<Coin>().TurnOff();
                }
            }
        }
    }


    public void CoinGot(GameObject coin)
    {
        int coinYvalue = -1;

        for(int i = 0; i < coinsInLevel.Count; i++)
        {
            if(coinsInLevel[i].gameObject == coin)
            {
                coinYvalue = i;
            }
        }

        for(int i = 0; i < allCoins.Count; i++)
        {
            Vector3 coinBeingChecked = allCoins[i];
            if(SceneManager.GetActiveScene().buildIndex == coinBeingChecked.x && coinYvalue == allCoins[i].y)
            {
                allCoins[i] = new Vector3(coinBeingChecked.x, coinBeingChecked.y, 1);
            }
        }
    }

}
