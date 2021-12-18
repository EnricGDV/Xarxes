using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;
    public Vector3 distance;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(target.transform.position.x - distance.x, target.transform.position.y + distance.y, target.transform.position.z + distance.z);
        gameObject.transform.LookAt(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.transform.position.x - distance.x, target.transform.position.y + distance.y, target.transform.position.z + distance.z);
        Quaternion newRot = Quaternion.LookRotation(target.transform.position - transform.position);

        gameObject.transform.position = Vector3.SlerpUnclamped(gameObject.transform.position, newPos, Time.deltaTime);
        gameObject.transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, newRot, Time.deltaTime);
    }
}
