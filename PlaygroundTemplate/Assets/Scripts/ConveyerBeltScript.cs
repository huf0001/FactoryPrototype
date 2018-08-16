using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltScript : MonoBehaviour
{
    public GameObject belt;
    public Transform endpoint;
    public float speed;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "PlayerMoving")
        {
            other.transform.position = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);
        }
    }
}
