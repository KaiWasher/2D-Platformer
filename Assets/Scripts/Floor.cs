using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private GameObject mInfectedSurface;

    private float mStopWatch;
    private float mRandomNumber;

    // Start is called before the first frame update
    void Start()
    {
        mStopWatch = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mStopWatch += Time.deltaTime;

        if(Mathf.RoundToInt(mStopWatch) == 4)
        {
            Debug.Log("4 seconds. Might spawn more infected surfaces.");
            mRandomNumber = Random.Range(1, 6);
            if(mRandomNumber == 1)
            {
                Debug.Log("Spawning an infected surface");
                Instantiate(mInfectedSurface, transform.position, Quaternion.identity);
            }
            mStopWatch = 0;
        }
    }
}
