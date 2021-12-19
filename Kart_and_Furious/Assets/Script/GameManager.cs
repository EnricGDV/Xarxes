﻿using System.Collections;
using System.Collections.Generic;
using Complete;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject kartPrefab;
    public Transform initialCoords;
    public GameObject camera;
    public int maxLaps = 1;
    private int laps = 0;
    public List<KartMovement> kartsList;
    public int kartID = 1;

    public bool kartNeeded = false;

    private List<GameObject> kartObjects;

    private void Start()
    {

    }

    private void Update()
    {
        if(kartNeeded)
        {
            kartNeeded = false;
            AddKart(kartID);
        } 

        if(laps == maxLaps+1)
        {
            Debug.Log("Race Finished!!");
        }
    }


    public void AddKart(int id)
    {
        GameObject newPlayer = GameObject.Find("Client");
        GameObject newKart = Instantiate(kartPrefab, newPlayer.transform);
        newKart.name = "Kart " + id;
        newKart.transform.position = new Vector3(initialCoords.position.x - 0.8f * id, initialCoords.position.y, initialCoords.position.z - 0.6f * (id%2-1));
        camera.GetComponent<CameraScript>().target = newKart;
        kartsList.Add(newKart.GetComponent<KartMovement>());
        kartObjects.Add(newKart);     
    }

    public void AddLap(int kartnum)
    {
        laps++;
        Debug.Log("Kart " + kartnum + " has completed " + (laps-1) + "/" + maxLaps + " laps!");
    }
}
