using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;

    [SerializeField]
    private PhysicMaterial[] frictions;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int friction = Random.Range(0, frictions.Length - 1);
            int rnd_X = Random.Range(-50, 50);
            int rnd_Z = Random.Range(-50, 50);
            GameObject createdCube = Instantiate(cube, new Vector3(rnd_X, 20, rnd_Z), Quaternion.identity, transform);
            createdCube.GetComponent<BoxCollider>().material = frictions[friction];
        }
    }
}
