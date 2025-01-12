using UnityEngine;
using System.Collections;

public class FindInactiveByTag : MonoBehaviour
{
    public static GameObject FindInactiveGameObjectByTag(string tag)
    {
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allGameObjects)
        {
            if (obj.CompareTag(tag) && obj.hideFlags == HideFlags.None)
            {
                return obj;
            }
        }

        return null; // Return null if no matching GameObject is found
    }
}