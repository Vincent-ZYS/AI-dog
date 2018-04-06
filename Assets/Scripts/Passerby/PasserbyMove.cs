using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasserbyMove : MonoBehaviour {

    private GameObject[] allPositions;
    private float[] allDistances;
    private int currentPosition = 0;
    private float minDistance;

    public float walkSpeed;


	void Start()
	{
        allPositions = GameObject.FindGameObjectsWithTag("position");
        allDistances = new float[allPositions.Length];
	}

    void Update()
    {
        Invoke("searchDistance",minDistance/(Time.deltaTime * walkSpeed));
        
        PMoveBehaviour(allPositions[currentPosition].transform.position);

        if (Vector3.Distance(transform.position, allPositions[currentPosition].transform.position) <= 2)
        {
            currentPosition++;
        }
        if (currentPosition == allPositions.Length)
        {
            currentPosition = 0;
        } 
	}

    public void PMoveBehaviour(Vector3 nextAim)
    {
        transform.LookAt(nextAim);
        transform.position = Vector3.Lerp(transform.position, nextAim, Time.deltaTime * walkSpeed);
    }

    public void searchDistance()
    {
        minDistance = allDistances[0];
        for (int i = 0; i < allPositions.Length; i++)
        {
            allDistances[i] = Vector3.Distance(transform.position, allPositions[i].transform.position);
            if (minDistance > allDistances[i])
            {
                minDistance = allDistances[i];
                currentPosition = i;
            }
        }
    }
}
