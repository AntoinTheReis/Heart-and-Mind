using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitCoin : MonoBehaviour
{

    static List<Vector3> allCoins = new List<Vector3>();
    //x is the scene build number of the coin
    //y is the number of the coin in the scene 
    //z is whetgher the coin has been obtained or not (0 is no, 1 is yes)

    List<GameObject> coinsInLevel;
    List<GameObject> coinsInLevelPre;

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


        if (coinsInLevel.Count > 1 && !coinsInLevel[1].name.Contains("1"))
        {
            Debug.Log("Coin in second position is" + coinsInLevel[1].name);
            Debug.Log("Swapped coin positions");

            Swap(coinsInLevel[1], coinsInLevel[0]);
        }


        if (!levelsVisited.Contains(currentScene))
        {
            levelsVisited.Add(currentScene);
            Debug.Log("Visiting New Scene");

            for (int i = 0; i < coinsInLevel.Count; i++)
            {
                allCoins.Add(new Vector3(currentScene, i, 0));
                Debug.Log("Add coin to list: " + currentScene + "," + i + "," + 0);
            }
        }
        else
        {
            Debug.Log("Visiting Old Scene");
            for (int i = 0; i < allCoins.Count; i++)
            {
                if (allCoins[i].x == currentScene && allCoins[i].z == 1)
                {
                    coinsInLevel[(int)(allCoins[i].y)].GetComponent<Coin>().TurnOff();
                    Debug.Log("Turning off coin: " + allCoins[i].x + "," + allCoins[i].y + "," + allCoins[i].z);
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
                Debug.Log("Obtained coin: " + allCoins[i].x + "," + allCoins[i].y + "," + allCoins[i].z);
            }
        }
    }

    public int GetAcmountOfCoinsFoundButNotNecessarilyGotten()
    {
        Debug.Log(allCoins.Count() + " coins found so far");
        return allCoins.Count();
    }

    public bool AllCoinsGotten()
    {
        Debug.Log("Have all the coins");
        for(int i = 0; i < allCoins.Count; i++)
        {
            if (allCoins[i].z == 0) return false;
        }
        return true;
    }

    public int AmountOfCoinsLeft()
    {
        int numba = 0;
        for (int i = 0; i < allCoins.Count; i++)
        {
            if (allCoins[i].z == 0) numba++;
        }
        return numba;
    }

    void Swap(GameObject shouldGoFirst, GameObject shouldGoSecond)
    {
        GameObject temp = coinsInLevel[1];
        coinsInLevel[1] = coinsInLevel[0];
        coinsInLevel[0] = temp;
    }

}
