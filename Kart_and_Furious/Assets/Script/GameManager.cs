using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject kartPrefab;
    public Transform initialCoords;
    public GameObject camera;

    private void Start()
    {
        AddKart(1);
    }
    public void AddKart(int id)
    {
        GameObject newPlayer = GameObject.Find("Client");
        GameObject newKart = Instantiate(kartPrefab, newPlayer.transform);
        newKart.name = "Kart " + id;
        newKart.transform.position = new Vector3(initialCoords.position.x - 0.8f * id, initialCoords.position.y, initialCoords.position.z - 0.6f * (id%2-1));
        camera.GetComponent<CameraScript>().target = newKart;

    }
}
