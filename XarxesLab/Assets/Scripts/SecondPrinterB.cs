using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class SecondPrinterB : MonoBehaviour
{

    private bool exit = false;

    // Start is called before the first frame update
    void Start()
    {
        Thread myThread = new Thread(threadInfiniteSecondPrinter);
        myThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            exit = true;
        }
    }



    void threadInfiniteSecondPrinter()
    {
        Debug.LogWarning("Starting Thread!!");
        DateTime myTime;

        while (!exit)
        {
            myTime = System.DateTime.UtcNow;
            while ((System.DateTime.UtcNow - myTime).Seconds < 5f)
            {
            }
            Debug.Log("5 more seconds have passed!");
        }
        Debug.LogWarning("Thread Stopping!!");
    }
}
