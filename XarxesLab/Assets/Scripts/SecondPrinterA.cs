using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SecondPrinterA : MonoBehaviour
{
    private bool exit = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("coroutineInfiniteSecondPrinter");
        Debug.LogWarning("Coroutine Started!!");
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            exit = true;
        }

    }



    IEnumerator coroutineInfiniteSecondPrinter()
    {
        DateTime myTime;
        while (!exit)
        {
            myTime = System.DateTime.UtcNow;
            while ((System.DateTime.UtcNow - myTime).Seconds < 10f)
            {
                yield return null;

            }
            Debug.Log("10 more seconds have passed!");
        }
        Debug.LogWarning("Coroutine Stopped!!");
    }
}
