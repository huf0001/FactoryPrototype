using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawnItemScript : MonoBehaviour
{
    [System.Serializable]
    public class ItemWeightPair
    {
        [SerializeField] private GameObject item;
        [SerializeField] private int itemWeighting;

        public GameObject Item
        {
            get
            {
                return item;
            }
        }

        public int ItemWeighting
        {
            get
            {
                return itemWeighting;
            }
        }
    }

    [SerializeField] private float secBetweenSpawns = 2f;
    [SerializeField] ItemWeightPair[] itemWeightPairs;

    private List<GameObject> weightedObjects = new List<GameObject>();
    private float time = 0f;
    
    // Use this for initialization
	void Start ()
    {
        foreach (ItemWeightPair p in itemWeightPairs)
        {
            for (int i = 0; i < p.ItemWeighting; i++)
            {
                weightedObjects.Add(p.Item);
            }
        }

        // Debug.Log("weightedObjects.Count is :" + weightedObjects.Count);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (time > secBetweenSpawns)
        {
            Spawn();
            time = Time.deltaTime;
        }
        else
        {
            time += Time.deltaTime;
        }
	}

    private void Spawn()
    {
        int i = Random.Range(0, weightedObjects.Count);
        GameObject spawning = Instantiate(weightedObjects[i]);
        spawning.transform.position = this.gameObject.transform.position;
        spawning.transform.rotation = this.gameObject.transform.rotation;
    }
}
