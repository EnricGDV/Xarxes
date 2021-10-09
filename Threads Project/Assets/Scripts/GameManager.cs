using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public GameObject prefab;
    private GameObject ball;
    private Thread myThread;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ball == null)
        {

            ball = Instantiate(prefab, new Vector3(-8, 0, 0), Quaternion.identity);
            myThread = new Thread(threadBallTimer);
            ball.GetComponent<Rigidbody>().velocity = new Vector3(3, 0, 0);
            myThread.Start();
            
        }
        if (!myThread.IsAlive)
        {
            Destroy(ball);
        }
    }


    void threadBallTimer()
    {
        Debug.LogWarning("Starting BallTimer Thread!!");
        DateTime myTime;
        myTime = System.DateTime.UtcNow;

        while ((System.DateTime.UtcNow - myTime).Seconds < 5f)
        {
        }
        Debug.LogWarning("BallTimer Thread Stopping!!");
    }
}
