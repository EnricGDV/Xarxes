//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GoalScript : MonoBehaviour
//{
//    public GameObject manager;
//    private int laps1 = 0;
//    private int laps2 = 0;
//    private int laps3 = 0;
//    private int laps4 = 0;

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.gameObject.name == "Kart 1")
//        {
//            laps1++;
//            if (laps2 < laps1 && laps3 < laps1 && laps4 < laps1)
//                manager.GetComponent<GameManager>().AddLap(1);
//        }
//        else if (other.gameObject.name == "Kart 2")
//        {
//            laps2++;
//            if (laps1 < laps2 && laps3 < laps2 && laps4 < laps2)
//                manager.GetComponent<GameManager>().AddLap(2);
//        }
//        else if (other.gameObject.name == "Kart 3")
//        {
//            laps3++;
//            if (laps1 < laps3 && laps2 < laps3 && laps4 < laps3)
//                manager.GetComponent<GameManager>().AddLap(3);
//        }
//        else if (other.gameObject.name == "Kart 4")
//        {
//            laps4++;
//            if (laps1 < laps4 && laps2 < laps4 && laps3 < laps4)
//                manager.GetComponent<GameManager>().AddLap(4);
//        }
//    }
//}
