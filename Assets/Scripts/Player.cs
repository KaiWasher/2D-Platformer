using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float mWalkSpeed;
    [SerializeField] private float mRunSpeed;
    [SerializeField] private float mJumpingForce;
    [SerializeField] private LayerMask mWhatIsGround;
    [SerializeField] private float mLevelTime;
    [SerializeField] private GameObject mMob;
    [SerializeField] private int mMobInterval;
    [SerializeField] private GameObject mGameOverScreen;
    [SerializeField] private GameObject mDeathParticle;
    [SerializeField] private GameObject mPowerBoost;
    [SerializeField] private LevelReached levelReached;

    private float mGroundCheckRadius = 0.1f;

    public static bool newLevel;

    private Text[] textFields;
    private Text mPointsText;
    private Text mLevelNumberText;
    private Text mLevelReachedText;

    public static int level;

    private float mMobStopWatch;
    private float mLevelStopWatch;

    public static float speedMultiplier;

    public static int points;

    private bool mRunning;
    private bool mMoving;
    private bool mGrounded;
    private bool mFalling;

    private Rigidbody2D mRigidBody2D;
    private Transform mGroundCheck;
    private Animator mAnimator;

    private AudioSource[] mAudioSources;
    private AudioSource mInteractWithNpcSound;
    private AudioSource mLevelUpSound;
    private AudioSource mDisinfectSound;
    private AudioSource mMobSpawnSound;

    // Start is called before the first frame update
    void Start()
    {
        mRigidBody2D = GetComponent<Rigidbody2D>();
        mGroundCheck = transform.Find("GroundCheck");
        mAnimator = GetComponent<Animator>();
        textFields = GetComponentsInChildren<Text>();
        mPointsText = textFields[0];
        mLevelNumberText = textFields[1];
        points = 0;
        mPointsText.text = "Points: " + points.ToString();
        Time.timeScale = 1;
        speedMultiplier = 1;
        level = 1;
        mMobStopWatch = 0;
        mLevelStopWatch = 0;
        newLevel = false;
        mLevelNumberText.text = "Level 1";
        mAudioSources = GetComponents<AudioSource>();
        mInteractWithNpcSound = mAudioSources[0];
        mLevelUpSound = mAudioSources[1];
        mDisinfectSound = mAudioSources[2];
        mMobSpawnSound = mAudioSources[3];
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        MovePlayer();
        CheckFalling();
        UpdatePoints();
        SpawnMobOverInterval();
        mLevelStopWatch += Time.deltaTime;

        if (Mathf.RoundToInt(mLevelStopWatch) == mLevelTime)
        {
            levelReached.SetLevel(++level);
            mLevelNumberText.text = "Level " + level;
            mLevelStopWatch++;
            newLevel = true;
            mLevelUpSound.Play();
        }
        else if (Mathf.RoundToInt(mLevelStopWatch) == (mLevelTime + 2))
        {
            mLevelNumberText.text = "Level " + level;
            mLevelStopWatch++;
        }
        else if (Mathf.RoundToInt(mLevelStopWatch) == (mLevelTime + 4))
        {
            mLevelNumberText.text = "Level " + level;
            mLevelStopWatch++;
        }
        else if (Mathf.RoundToInt(mLevelStopWatch) == (mLevelTime + 6))
        {
            mLevelNumberText.text = "";
            mLevelStopWatch = 0;
            for (int i = 1; i <= level; i++)
            {
                SpawnMob();
            }
            Instantiate(mPowerBoost, new Vector3(Random.Range(-15f, 15f), Random.Range(-9f, 15f), 0),Quaternion.identity);
        }
        else if (level == 1 && Mathf.RoundToInt(mLevelStopWatch) == 3)
        {
            Instantiate(mPowerBoost, new Vector3(Random.Range(-15f, 15f), Random.Range(-9f, 8f), 0), Quaternion.identity);
            mLevelNumberText.text = "";
            mLevelStopWatch++;
        }
        else
        {
            newLevel = false;
        }

        if (points <= -20)
        {
            Debug.Log("Game Over");
            mLevelNumberText.text = "";
            Instantiate(mGameOverScreen, new Vector3(0, 0, 0), Quaternion.identity);
            levelReached.DisplayLevelReached();
            Time.timeScale = 0;
            Destroy(gameObject);
        }

        mAnimator.SetBool("isWalking", mMoving);
        mAnimator.SetBool("isGrounded", mGrounded);
        mAnimator.SetBool("isFalling", mFalling);
    }

    private void CheckGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mGroundCheck.position, mGroundCheckRadius, mWhatIsGround);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                mGrounded = true;
                return;
            }
        }
        mGrounded = false;
    }

    private void MovePlayer()
    {
        mRunning = Input.GetButton("Run");

        float horizontal = Input.GetAxis("Horizontal");
        mMoving = !Mathf.Approximately(horizontal, 0f);
        if(mMoving)
        {
            transform.Translate(horizontal * (mRunning ? mRunSpeed : mWalkSpeed) * Time.deltaTime * speedMultiplier, 0, 0);
            FaceCorrectDirection(horizontal < 0f ? true : false);
        }

        if (mGrounded && Input.GetButton("Jump") && mRigidBody2D.velocity.y == 0f)
        {
            mRigidBody2D.AddForce(new Vector2(0, mJumpingForce), ForceMode2D.Impulse);
        }
    }

    private void CheckFalling()
    {
        mFalling = mRigidBody2D.velocity.y < 0f;
    }

    private void FaceCorrectDirection(bool left)
    {
        Vector2 localScale = transform.localScale;

        if ((localScale.x < 0f && Input.GetAxis("Horizontal") > 0f) || localScale.x > 0f && Input.GetAxis("Horizontal") < 0f) 
        {
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if ((col.tag == "Unmasked" || col.tag == "Susceptible") && Input.GetButton("Give Mask"))
        {
            NPC npc = col.GetComponent<NPC>();
            npc.AddMaskToNPC();
            mInteractWithNpcSound.Play();
            points++;
        }
        else if (col.tag == "Infected" && Input.GetButton("Isolate"))
        {
            NPC npc = col.GetComponent<NPC>();
            npc.Isolate();
            mInteractWithNpcSound.Play();
        }
        else if (col.tag == "Surface" && Input.GetButton("Disinfect"))
        {
            InfectedSurface infectedSurface = col.GetComponent<InfectedSurface>();
            infectedSurface.Disinfect();
            mDisinfectSound.Play();
            points += 2;
        }
    }

    private void UpdatePoints()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "PointsUI")
            {
                mPointsText.text = "Points: " + points.ToString();
            }
        }
    }

    private void SpawnMobOverInterval()
    {
        mMobStopWatch += Time.deltaTime;

        if (Mathf.RoundToInt(mMobStopWatch) == mMobInterval)
        {
            Debug.Log("Spawning a mob");
            Instantiate(mMob, new Vector3(Random.Range(-18f,18f), Random.Range(-9f,18f), 0), Quaternion.identity);
            mMobSpawnSound.Play();
            mMobStopWatch = 0;
        }
    }

    private void SpawnMob()
    {
        Instantiate(mMob, new Vector3(Random.Range(-18f, 18f), Random.Range(-9f, 18f), 0), Quaternion.identity);
        mMobSpawnSound.Play();
    }
}

