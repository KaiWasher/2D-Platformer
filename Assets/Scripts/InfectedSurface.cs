using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectedSurface : MonoBehaviour
{
    [SerializeField] private float mTimeRemaining;
    [SerializeField] private ParticleSystem mDisinfectParticleEmitter;
    [SerializeField] private ParticleSystem mInfectParticleEmitter;

    private bool mTimerRunning;
    private float mInitialTime;

    // Start is called before the first frame update
    void Start()
    {
        mTimerRunning = true;
        mInitialTime = mTimeRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        if (mTimerRunning)
        {
            if(mTimeRemaining > 0)
            {
                mTimeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Infected surface timer is done.");
                mTimeRemaining = mInitialTime;
                mTimerRunning = false;
                Player.points--;
                Instantiate(mInfectParticleEmitter, transform.position, Quaternion.identity);
            }
        }
    }

    public void Disinfect()
    {
        Instantiate(mDisinfectParticleEmitter, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Unmasked" || collision.tag == "Masked" || collision.tag == "Susceptible")
        {
            NPC npc = collision.GetComponent<NPC>();
            npc.SetTouchedInfectedSurface(true);
            npc.SetInfectedSurface(gameObject.GetComponent<InfectedSurface>());
        }
    }

    public bool GetTimerRunning()
    {
        return mTimerRunning;
    }
}
