using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mSpriteRenderer;
    [SerializeField] private float mTimeOfBoost;
    [SerializeField] private GameObject mParticleEmitter;

    private bool mTimerRunning;

    private AudioSource mPowerUpSound;

    // Start is called before the first frame update
    void Start()
    {
        mTimerRunning = false;
        mPowerUpSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        RunTimer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.speedMultiplier = 2;
            mSpriteRenderer.forceRenderingOff = true;
            mTimerRunning = true;

            if(mParticleEmitter != null)
            {
                Instantiate(mParticleEmitter, transform.position, Quaternion.identity);
                mPowerUpSound.Play();
            }

            mParticleEmitter = null;
        }
    }

    private void RunTimer()
    {
        if (mTimerRunning)
        {
            if (mTimeOfBoost > 0)
            {
                mTimeOfBoost -= Time.deltaTime;
            }
            else
            {
                mTimeOfBoost = 0;
                mTimerRunning = false;
                Player.speedMultiplier = 1;
                Destroy(gameObject);
            }
        }
    }
}
