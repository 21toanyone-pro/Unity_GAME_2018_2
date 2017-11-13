using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public GameObject PlayerPos;

    Animator ani;
    Player player;
    public Collider2D coll;

    public float Boss_HP = 2000f;
    int RandPattern;

    bool HozCheck;
    bool RushCheck;

    public enum BOSSSTATE { SLEEP, IDLE, UPRISING, RUSH, SHOUT, SHOUT2, WAIT, MOVE, DEATH }
    BOSSSTATE bossstate = BOSSSTATE.IDLE;

    // Use this for initialization
    void Awake () {
        StartCoroutine(FSM());
        //StartCoroutine(Pattern());
        player = GameObject.Find("Player").GetComponent<Player>();
        ani = GetComponent<Animator>();
	}

    IEnumerator FSM()
    {
        while (true)
        {
            switch (bossstate)
            {
                case BOSSSTATE.SLEEP:
                    break;

                case BOSSSTATE.IDLE:
                    StartCoroutine(BossMove());
                    break;

                case BOSSSTATE.UPRISING: // 솟아오르기 (땅속으로 사라진 뒤 1초 후 플레이어가 있던 위치에 솟아오름)
                   
                    break;

                case BOSSSTATE.RUSH: // 덮치기 (플레이어가 있는 방향으로 빠르게 이동)
                    ani.SetBool("Rushing", true);
                    break;

                case BOSSSTATE.SHOUT: // 포효1 (플레이어가 서있는 위치에 0.8초마다 가시가 솟아오름)
                    
                    break;

                case BOSSSTATE.SHOUT2: // 포효2 (포효하는 입에서 가시가 나와 플레이어에게 날아감)
                   
                    break;

                case BOSSSTATE.DEATH:

                    break;

                case BOSSSTATE.WAIT:

                    break;
            }
            yield return null;
        }
    }

    IEnumerator Pattern()
    {
        if (bossstate == BOSSSTATE.IDLE)
        {
            RandPattern = Random.Range(0, 4);
            if (RandPattern == 0) // 덮치기 
            {
                bossstate = BOSSSTATE.RUSH;
            }

            else if (RandPattern == 1) //솟아오르기 
            {
                bossstate = BOSSSTATE.UPRISING;
            }

            else if (RandPattern == 2) //포효 1 
            {
                bossstate = BOSSSTATE.SHOUT;
            }

            else if (RandPattern == 3) //포효 2 
            {
                bossstate = BOSSSTATE.SHOUT2;
            }
        }
        yield return new WaitForSeconds(4f);
        StartCoroutine(Pattern());
    }

    IEnumerator BossMove() // 보스 움직임
    {
        Vector3 MoveVec = Vector3.zero;
        string move = "";
        Vector3 playerPos = PlayerPos.transform.position;

        if (playerPos.x < transform.position.x - 0.5f)
        {
            move = "Left";
        }

        else if (playerPos.x > transform.position.x + 0.5f)
        {
            move = "Right";
        }

        if (move == "Left")
        {
            MoveVec = Vector3.left;
            transform.localScale = new Vector3(1, 1, 1);
            HozCheck = true;
        }

        else if (move == "Right")
        {
            MoveVec = Vector3.right;
            transform.localScale = new Vector3(-1, 1, 1);
            HozCheck = false;
        }

        transform.position += MoveVec * 1 * Time.deltaTime;
        yield return null;

    }
    IEnumerator Rush_P() //덮치기
    {
        Vector3 RushVec = Vector3.zero;
        Vector3 playerPos = player.transform.position;
        var t = 0f;

        do
        {
            if (HozCheck)
            {
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (!HozCheck)
            {
                RushVec = Vector3.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            t += Time.deltaTime * 2f;
            transform.position += RushVec * 10 * Time.deltaTime;
            yield return null;
        } while (t < 2);

        if (t >= 2)
        {
            RushCheck = false;
            yield return new WaitForSeconds(0.5f);
            ani.SetBool("Rushing", false);
            bossstate = BOSSSTATE.IDLE;
        }
    }


    void FixedUpdate()
    {
        float Cast = Mathf.Abs(transform.position.x - PlayerPos.transform.position.x);

        // 여긴 못지나가게 체크
        if (player.RollingCheck && Cast < 4)
        {
            coll.enabled = false;
        }

        else if (!player.RollingCheck && Cast > 4)
        {
            coll.enabled = true;
        }
    }

}
