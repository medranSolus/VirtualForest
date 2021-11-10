using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateList : MonoBehaviour
{
    [SerializeField]
    uint maxVisible = 4;
    [SerializeField]
    List<GameObject> availableObjects;
    [SerializeField]
    GameObject list;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in availableObjects)
            Instantiate(obj.transform.Find("Image").gameObject).transform.SetParent(list.transform, false);
    }
}