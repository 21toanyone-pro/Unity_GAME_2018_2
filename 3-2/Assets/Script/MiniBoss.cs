using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour {

    public bool MovePos;
    bool moveCheck;
    Rigidbody2D rd;
    public Collider2D coll;
    Player player;
    Boss boss;

	// Use this for initialization
	void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        rd = GetComponent<Rigidbody2D>();
        if(boss.BabyNum == 1 || boss.BabyNum == 3 || boss.BabyNum == 4)
        {
            Angel_move(rd.velocity = new Vector3(10, 10, 0));
            MovePos = false;
        }
        else
        {
            Angel_move(rd.velocity = new Vector3(-10, 10, 0));
            MovePos = true;
        }

        player = GameObject.Find("Player").GetComponent<Player>();
        coll.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

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

            transform.position += moveVec * 3f * Time.deltaTime;
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
            coll.enabled = true;
            moveCheck = true;
        }

        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }


}
