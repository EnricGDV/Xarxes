using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject kartPrefab;
    public List<GameObject> allKarts;

    private void Start()
    {
        AddKart(1);
    }
    public void AddKart(int id)
    {
        GameObject newPlayer = GameObject.Find("Client");
        GameObject newKart = Instantiate(kartPrefab, newPlayer.transform);
        newKart.name = "Kart " + 1;
    }
}
