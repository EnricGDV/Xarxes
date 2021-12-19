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
        GameObject newPlayer = new GameObject("Player " + 1);
        GameObject newKart = Instantiate(kartPrefab, newPlayer.transform);
        newKart.name = "Kart " + 1;
    }
}
