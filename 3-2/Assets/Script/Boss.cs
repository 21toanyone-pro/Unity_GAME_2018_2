using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public GameObject PlayerPos;
    public GameObject groundStinger; //땅에서 나오는 가시
    public GameObject World_Stinger;
    public GameObject Stones;
    public GameObject[] StoneDrop = new GameObject[20];
    // GameObject[] Stone;

    
    Camera_Shake CShacke;
    Animator ani;
    Player player;
    public Collider2D coll;

    public float Boss_HP = 2000f;
    float posUp = 0;
    int RandPattern;
    int StingerNum;

    bool CenterCheck;
    bool HozCheck;
    bool RushCheck;
    bool UprisingCheck;
    bool WallCheck;

    int paseCheck; // 1, 2, 3 = 페이즈 1,2,3

    public enum BOSSSTATE { SLEEP, IDLE, UPRISING, RUSH, SHOUT, SHOUT2, WAIT, MOVE, DEATH, PAGE03 }
    BOSSSTATE bossstate = BOSSSTATE.IDLE;

    // Use this for initialization
    void Awake () {
        StartCoroutine(FSM());
        StartCoroutine(Pattern());
        player = GameObject.Find("Player").GetComponent<Player>();
        CShacke = GameObject.Find("Main Camera").GetComponent<Camera_Shake>();
        ani = GetComponent<Animator>();
        CShacke.enabled = false;
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
                    if(paseCheck == 3)
                    {
                        bossstate = BOSSSTATE.PAGE03;
                        Debug.Log("!");
                    }
                    break;

                case BOSSSTATE.MOVE:
                    break;

                case BOSSSTATE.UPRISING: // 솟아오르기 (땅속으로 사라진 뒤 1초 후 플레이어가 있던 위치에 솟아오름)
                    if (paseCheck == 1)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                    }
                    else if(paseCheck ==2)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                    }
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.RUSH: // 덮치기 (플레이어가 있는 방향으로 빠르게 이동)
                    if(paseCheck == 1)
                    {
                       StartCoroutine(Rush_P());
                    }
                    
                    else if(paseCheck ==2)
                    {
                        StartCoroutine(Rush_Pase2());
                    }

                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.SHOUT: // 포효1 (플레이어가 서있는 위치에 0.8초마다 가시가 솟아오름)
                        ani.SetBool("Shoting", true);
                        StingerNum = 0;
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.DEATH:
                    break;

                case BOSSSTATE.WAIT:
                    break;
                case BOSSSTATE.PAGE03:
                    StartCoroutine(Pasge03_boss());
                    bossstate = BOSSSTATE.WAIT;
                    break;
            }
            yield return null;
        }
    }

    IEnumerator Pattern()
    {
        if (bossstate == BOSSSTATE.IDLE)
        {
            RandPattern = Random.Range(0, 3);
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
        }
        yield return new WaitForSeconds(4f);
        StartCoroutine(Pattern());
    }


    void bossSpikeing()
    {
        ani.SetBool("Shoting", false);
        ani.SetBool("Spikeing", true);
        Vector3 StingerPos = new Vector3(PlayerPos.transform.position.x, -5.63f, PlayerPos.transform.position.z);
        if(paseCheck ==1)
        {
            StartCoroutine(Shout_Stinger(StingerPos));
        }
        else if(paseCheck ==2)
        {
            StartCoroutine(Shout_World());
        }
       
    }

    IEnumerator Shout_World() 
    {
        Vector3 World_pos = new Vector3(12.0f, 0.77f, 0f);
        CShacke.enabled = true;
        CShacke.shake = 1f;
        Invoke("StopShake", 0.9f);
        yield return new WaitForSeconds(1f);
        Instantiate(World_Stinger, World_pos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        ani.SetBool("Spikeing", false);
        bossstate = BOSSSTATE.IDLE;
    }


    IEnumerator Shout_Stinger(Vector3 pos)
    {
        StingerNum += 1;
        yield return new WaitForSeconds(0.8f);
        Instantiate(groundStinger, pos, Quaternion.identity);
        if(StingerNum < 5)
        {
            StartCoroutine(Shout_Stinger(new Vector3(PlayerPos.transform.position.x, -5.63f, PlayerPos.transform.position.z)));
        }

        else if(StingerNum == 5)
        {
            ani.SetBool("Spikeing", false);
            yield return new WaitForSeconds(0.8f);
            bossstate = BOSSSTATE.IDLE;
        }
    }

    IEnumerator Drop_Stone()
    {
        int[] randArray = new int[10];
        bool isSame;
        for(int i =0; i< 10; i++)
        {
            while(true)
            {
                randArray[i] = Random.Range(0, 20);
                isSame = false;
                for(int j =0; j< i; ++j)
                {
                    if(randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
            Instantiate(Stones, StoneDrop[randArray[i]].transform.position, Quaternion.Euler(0,0,-36f));
        }
        yield return new WaitForSeconds(0.1f);


    }

    public int[] getRandomInt(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;

        for (int i = 0; i < length; ++i)
        {
            while (true)
            {
                randArray[i] = Random.Range(min, max);
                isSame = false;

                for (int j = 0; j < i; ++j)
                {
                    if (randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
        }
        return randArray;
    }

    IEnumerator Pasge03_boss()
    {
        Vector3 MoveVec = Vector3.zero;
        do
        {
            if (HozCheck)
            {
                ani.SetBool("Rushing", true);
                MoveVec = Vector3.left;
                if (CenterCheck)
                {
                    StartCoroutine(Crying_Boss());
                    break;
                }
            }
            else if (!HozCheck)
            {
                ani.SetBool("Rushing", true);
                MoveVec = Vector3.right;
                if (CenterCheck)
                {
                    StartCoroutine(Crying_Boss());
                    break;
                }
            }
            transform.position += MoveVec * 5 * Time.deltaTime;
            yield return null;
        } while (true);
       
    }

    IEnumerator Crying_Boss()
    {
        Debug.Log("Cry!!");
        bossstate = BOSSSTATE.IDLE;
        yield return null;
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
                ani.SetBool("Rushing", true);
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (!HozCheck)
            {
                ani.SetBool("Rushing", true);
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

    IEnumerator Rush_Pase2()
    {
        Vector3 RushVec = Vector3.zero;
        Vector3 playerPos = player.transform.position;
 
        do
        {
            if (HozCheck)
            {
                
                ani.SetBool("Rushing", true);
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (!HozCheck)
            {
                ani.SetBool("Rushing", true);
                RushVec = Vector3.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            transform.position += RushVec * 10 * Time.deltaTime;
            
            yield return null;
        } while (!WallCheck);

        if (WallCheck)
        {
            RushCheck = false;
            ani.SetBool("Rushing", false);
            StartCoroutine(Drop_Stone());
            bossstate = BOSSSTATE.UPRISING;
            WallCheck = false;
        }

    }

    IEnumerator Uprising(Vector3 End) // 들어갔다 나오기
    {
        ani.SetBool("Digging", true);
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(End.x, transform.position.y + 20, transform.position.z);
        ani.SetBool("Digging", false);
        yield return new WaitForSeconds(0.5f);
        bossstate = BOSSSTATE.IDLE;
    }

    public void Boss_Dig() //들어가는 이미지가 끝난후 아래로 내려준다.
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 20, transform.position.z);
    }


    void FixedUpdate()
    {
        float Cast = Mathf.Abs(transform.position.x - PlayerPos.transform.position.x);

        if(Boss_HP >= 1000)
        {
            paseCheck = 1;
        }

        else if(Boss_HP < 1000 && Boss_HP > 500)
        {
            paseCheck = 2;
        }

        else if (Boss_HP <= 500)
        {
            paseCheck = 3;
        }

        // 여긴 못지나가게 체크
        if (player.RollingCheck && Cast < 5)
        {
            coll.enabled = false;
        }

        else if (!player.RollingCheck && Cast > 5)
        {
            coll.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Wall" && paseCheck ==2)
        {
            WallCheck = true;
            CShacke.enabled = true;
            CShacke.shake = 0.5f;
            Invoke("StopShake", 0.5f);
        }

        if(other.gameObject.tag == "Center")
        {
            CenterCheck = true;
        }
    }

    void StopShake()
    {
        CShacke.enabled = false;
    }
}
