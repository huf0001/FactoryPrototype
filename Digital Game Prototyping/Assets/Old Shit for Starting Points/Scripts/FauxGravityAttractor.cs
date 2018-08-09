using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour
{
    public float gravity = -10;
    public float destDistanceLimit = 10f;
    public float DropOff = 1;
    private bool gravOn = false;
    private bool anyDestructible = false;
    private bool lastPing = true;
    private float destructiblePeriod = 0f;

    public GameObject gameUI = null;

    private List<Transform> fauxGravBodyTransforms = new List<Transform>();

    public void AddFauxGravBody(Transform body)
    {
        fauxGravBodyTransforms.Add(body);
    }

    public void RemoveFauxGravBody(Transform body)
    {
        fauxGravBodyTransforms.Remove(body);
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.Space)) || (Input.GetAxis("Fire1") > 0.1f))
        {
            gravOn = true;
            anyDestructible = false;
            lastPing = false;

            foreach (Transform t in fauxGravBodyTransforms)
            {
                Attract(t);
            }

            if (anyDestructible)
            {
                destructiblePeriod = 0f;
            }
            else
            {
                destructiblePeriod += Time.deltaTime;
            }

            PingGravAudio();
        }
        else if (!lastPing)
        {
            gravOn = false;
            lastPing = true;
            destructiblePeriod += Time.deltaTime;

            PingGravAudio();
        }
    }

    public void Attract(Transform body)
    {

        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        Rigidbody rb = body.GetComponent<Rigidbody>();
        float dist = Vector3.Distance(transform.position, body.position);
        rb.AddForce((gravityUp * gravity) / (Mathf.Pow((dist / DropOff), 2)));

        if (dist < destDistanceLimit)
        {
            //body.gameObject.GetComponent<EnemyScript>().Destructible = true;
            anyDestructible = true;
        }
    }

    private void PingGravAudio()
    {
        if (gameUI == null)
        {
            Debug.Log("<color=orange>" + gameObject.name + ": Game UI assignment missing in Unity GUI.</color>");
        }
        else
        {
            ArenaAudioScript audioScript = gameUI.GetComponent<ArenaAudioScript>();
            //if script = null, error message, else pass along gravOn + anyDestructible
            if (audioScript == null)
            {
                Debug.Log("<color=orange>" + gameObject.name + ": AudioScript assignment to the GameUI object missing in Unity GUI.</color>");
            }
            else
            {
                audioScript.PingGravAudio(gravOn, anyDestructible, destructiblePeriod);
            }
        }
    }
}
