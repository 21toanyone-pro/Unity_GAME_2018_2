using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBoss : MonoBehaviour {


    public AudioSource BossSound;
    public AudioClip Death;
    public AudioClip Idle;
    public bool MovePos;
    bool moveCheck;
    Rigidbody2D rd;
    public Collider2D coll;
    Player player;
    Boss boss;
    Animator ani;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("WalkSound", 0.0f, 2f);
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        ani = GetComponent<Animator>();
        rd = GetComponent<Rigidbody2D>();
        if (boss.BabyNum == 1 || boss.BabyNum == 3 || boss.BabyNum == 4)
        {
            Angel_move(rd.velocity = new Vector3(10, 10, 0));
            transform.localScale = new Vector3(-1f, 1f, 0f);
            MovePos = false;
        }
        else
        {
            Angel_move(rd.velocity = new Vector3(-10, 10, 0));
            transform.localScale = new Vector3(1f, 1f, 0f);
            MovePos = true;
        }

        player = GameObject.Find("Player").GetComponent<Player>();
        coll.enabled = false;
    }
    void WalkSound()
    {
        BossSound.PlayOneShot(Idle);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveVec = Vector3.zero;
        if (moveCheck)
        {
            if (MovePos) // true면 왼쪽으로
            {
                moveVec = Vector3.left;
                transform.localScale = new Vector3(1f, 1f, 0f);
            }

            else // false 면 오른쪽으로
            {
                moveVec = Vector3.right;
                transform.localScale = new Vector3(-1f, 1f, 0f);
            }

            transform.position += moveVec * 1f * Time.deltaTime;
        }
    }

    void Angel_move(Vector3 velocity)
    {
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            ani.SetBool("Ground", true);
            coll.enabled = true;
            moveCheck = true;
        }

        if (other.gameObject.tag == "Player")
        {
            ani.SetBool("Death", true);
            BossSound.clip = Death;
            BossSound.Play();
            Destroy(gameObject, 0.5f);
        }

        if (other.gameObject.tag == "Wall")
        {
            ani.SetBool("Death", true);
            BossSound.clip = Death;
            BossSound.Play();
            Destroy(gameObject, 0.5f);
        }

        if (other.gameObject.tag == "Attackcoll")
        {
            BossSound.clip = Death;
            BossSound.Play();
            ani.SetBool("Death", true);
            Destroy(gameObject, 0.5f);
        }
    }
}
