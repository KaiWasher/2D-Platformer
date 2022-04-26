using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private float mWalkSpeed;
    [SerializeField] private int mWalkingDirection;
    [SerializeField] private ParticleSystem mIsolateParticleEmitter;

    private int mDirection;

    private bool mMoving;
    private bool mTurnAround;

    private InfectedSurface mInfectedSurface;
    private bool mTouchedInfectedSurface;
    private bool mTouchedInfectedNPC;

    private Vector2 flipScale;

    public GameObject mMaskedPrefab;
    public GameObject mInfectedPrefab; 

    private Rigidbody2D mRigidBody2D;
    private SpriteRenderer mSpriteRenderer;
    private Collider2D mCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        mRigidBody2D = GetComponent<Rigidbody2D>();
        mDirection = mWalkingDirection;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mCollider2D = GetComponent<Collider2D>();
        flipScale = gameObject.transform.localScale;
        mInfectedSurface = null;

        if (mDirection > 0)
        {
            flipScale.x *= -1;
            gameObject.transform.localScale = flipScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveNPC();
        InfectIfTouchedSurface();
        InfectIfTouchedInfectedNPC();

        if(Player.newLevel)
        {
            if(gameObject.tag == "Masked")
            {
                Isolate();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall" || collision.tag == "Player")
        {
            mTurnAround = true;
        }
    }

    private void MoveNPC()
    { 
        transform.Translate(mWalkSpeed * mDirection * Time.deltaTime, 0, 0);

        if (mTurnAround == true)
        {
            mDirection *= -1;
            mTurnAround = false;
            flipScale.x *= -1;
            gameObject.transform.localScale = flipScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((gameObject.tag == "Unmasked" || gameObject.tag == "Susceptible") && collision.gameObject.tag == "Infected")
        {
            mTouchedInfectedNPC = true;
        }

        if (collision.gameObject.layer == 3 || collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.collider, mCollider2D);
        }
    }

    public void AddMaskToNPC()
    {
        GameObject newNPC = Instantiate(mMaskedPrefab) as GameObject;
        newNPC.transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }

    public void Isolate()
    {
        Instantiate(mIsolateParticleEmitter, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void InfectIfTouchedSurface()
    {
        if (mInfectedSurface != null && gameObject.tag == "Susceptible")
        {
            GameObject newNPC = Instantiate(mInfectedPrefab) as GameObject;
            newNPC.transform.position = gameObject.transform.position;
            Player.points -= 2;
            Destroy(gameObject);
        }
        else if (mInfectedSurface != null && mInfectedSurface.GetTimerRunning() == false)
        {
            GameObject newNPC = Instantiate(mInfectedPrefab) as GameObject;
            newNPC.transform.position = gameObject.transform.position;
            Player.points--;
            Destroy(mInfectedSurface.gameObject);
            Destroy(gameObject);
        }
    }

    private void InfectIfTouchedInfectedNPC()
    {
        if(mTouchedInfectedNPC)
        {
            GameObject newNPC = Instantiate(mInfectedPrefab) as GameObject;
            newNPC.transform.position = gameObject.transform.position;
            Player.points--;
            Destroy(gameObject);
        }
    }

    public bool GetTouchedInfectedSurface()
    {
        return mTouchedInfectedSurface;
    }

    public void SetTouchedInfectedSurface(bool touchedInfectedSurface)
    {
        mTouchedInfectedSurface = touchedInfectedSurface;
    }

    public void SetInfectedSurface(InfectedSurface infectedSurface)
    {
        mInfectedSurface = infectedSurface;
    }
}
