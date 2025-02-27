using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public static class BlockTracker
{
    public static LinkedList<GameObject> BlocksOnScreen = new LinkedList<GameObject>();
    
    public static void SortByDistance(ref LinkedList<GameObject> gameObjects, Vector3 position)
    {
        // Create a new list to use LINQ's OrderBy method, compute each element's distance from given position, and order by that distance (crazy line of code ik)
        List<GameObject> sortedList = gameObjects.OrderBy(go => Vector3.Distance(go.transform.position, position)).ToList();

        // Clear the original LinkedList
        gameObjects.Clear();

        // Add sorted elements back to the LinkedList
        foreach (var go in sortedList)
        {
            gameObjects.AddLast(go);
        }
    }
}
