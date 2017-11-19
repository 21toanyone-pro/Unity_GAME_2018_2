using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Animator animator;
    Rigidbody2D rigid;
    public Collider2D coll;

    public float PlayerHP = 100;
    public float PlayerST = 100;

    float CurrentHp;
    float CurrentST;
    float y = 0; //피채워줌 별거없음

    float JumpPower = 10; // 점프 파워

    bool JumpCheck; // 이중점프 체크
    bool MoveCheck; // 이동중 체크
    bool JumpCharsh; // 점프중 부딫힘
    bool AttackCheck;
    float AttackTime;
    public float Stamina_Time = 0f; //스태미나 차는 시간 체크
    public bool RollingCheck; // 구르기 체크

    int LR_Check = 1; // 좌우 체크
    int WAttackNum =0;

    // Use this for initialization
    void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(Recovery_ST());
    }

    void FixedUpdate()
    {
        if (AttackCheck)
        {
            AttackTime += Time.deltaTime;
        }

        if (!JumpCheck && !AttackCheck && !RollingCheck)
        {
            Stamina_Time += Time.deltaTime;
        }

        if (PlayerST <= 0)
        {
            PlayerST = 0;
        }
    }

    // Update is called once per frame
    void Update () {

        if(!RollingCheck) //구르는 중이 아니면 움직임 
        {
            if (!AttackCheck)
            {
                Moving();
            }
            WeakAttack();
            StrongAttack();
        }

        if (!JumpCheck && !AttackCheck)
        {
            Rolling(); // 구르기
        }

        if (Input.GetKeyDown(KeyCode.Space) && !JumpCheck && PlayerST >= 5) //점프 버튼 
        {
            animator.SetBool("Jumping", true);
            JumpCheck = true;
            Jumping();
        }
    }

    IEnumerator Recovery_ST()
    {
        do
        {
            if (Stamina_Time > 0.3f && PlayerST < 100)
            {
                PlayerST += 1;
            }
            yield return new WaitForSeconds(0.001f);
        } while (true);
    }

    IEnumerator Slow_ST()
    {
        yield return new WaitForSeconds(0.01f);
        if (y < CurrentST)
        {
            PlayerST -= 1;
            y += 1;
            StartCoroutine(Slow_ST());
        }

        else if (y == CurrentST)
            y = 0;
    }

    IEnumerator Slow_HP()
    {
        yield return new WaitForSeconds(0.01f);
        if (y < CurrentHp)
        {
            PlayerHP -= 1;
            y += 1;
            StartCoroutine(Slow_HP());
        }

        else if (y == CurrentHp)
            y = 0;
    }

    void Moving() //캐릭터 좌우 이동
    {
        float movepowers = 0;
       // float Cast = Mathf.Abs(transform.position.x - Boss2.transform.position.x);

        if (Input.GetKey(KeyCode.LeftArrow) && !AttackCheck)
        {
            animator.SetBool("Running", true);
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
            LR_Check = 0;
            movepowers = -5;
            MoveCheck = true;
        }

        else if (Input.GetKey(KeyCode.RightArrow) && !AttackCheck)
        {
            animator.SetBool("Running", true);
            movepowers = 5;
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
            LR_Check = 1;
            MoveCheck = true;
        }

        else
        {
            animator.SetBool("Running", false);
            MoveCheck = false;
        }

        if (!JumpCharsh && !AttackCheck)
            rigid.velocity = new Vector2(movepowers, rigid.velocity.y);
    }

    void Jumping() //점프
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(0, JumpPower), ForceMode2D.Impulse);
        CurrentST = 5;
        StartCoroutine(Slow_ST());
        Stamina_Time = 0;
    }


    void Rolling() //구르기 
    {
        var Start = transform.position;
        if (Input.GetKeyDown(KeyCode.LeftShift) && !RollingCheck && PlayerST >= 50) // 구르기 체크
        {
            animator.SetBool("Rolling", true);
            StartCoroutine(RollingState());
            RollingCheck = true;
            CurrentST = 50;
            StartCoroutine(Slow_ST());
            Stamina_Time = 0;
        }
    }

    IEnumerator RollingState()
    {
        Vector3 RollVec = Vector3.zero;
        float Rollinging = 0f;

        if (LR_Check == 0)
        {
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
            RollVec = Vector3.left * 1.5f;
        }

        else if (LR_Check == 1)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
            RollVec = Vector3.right * 1.5f;
        }
        while (Rollinging < 1f)
        {
            Rollinging += Time.deltaTime / 0.5f;
            transform.position += RollVec * 8f * Time.deltaTime;
            yield return null;
        }

        if(Rollinging >= 1)
        {
            animator.SetBool("Rolling", false);
            RollingCheck = false;
        }

    }

    

    public void WAttack_Count() // 첫번째 약공 끝날때
    {
        if(WAttackNum == 1)
        {
            animator.SetBool("WAttack01", false);
            AttackCheck = false;
            WAttackNum = 0;
            AttackTime = 0;
            coll.enabled = false;
        }
        coll.enabled = false;
    }

    public void WAttack_NextAttack() // 두번째 약공 끝날때
    {
        animator.SetBool("WAttack02", false);
        WAttackNum = 0;
        AttackTime = 0;
        coll.enabled = false;
        AttackCheck = false;
    }

    public void WAttack_StartColl() // 두번째 약공 다시 켜주기
    {
        coll.enabled = true;
    }

    void WeakAttack() // 약공
    {
        if (Input.GetKeyDown(KeyCode.S) && PlayerST >= 5)
        {
            AttackCheck = true;
            rigid.velocity = Vector2.zero;
            if (AttackCheck && WAttackNum ==0)
            {
                coll.enabled = true;
                WAttackNum = 1;
                animator.SetBool("WAttack01", true);
                CurrentST = 5;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }

            else if (AttackTime < 1.8f && WAttackNum ==1)
            {
                animator.SetBool("WAttack02", true);
                animator.SetBool("WAttack01", false);
                WAttackNum = 2;
                CurrentST = 5;
                StartCoroutine(Slow_ST());
            }
        }
    }

    public void SAttackCheck() //강공격 끝날때
    {
        animator.SetBool("SAttacking", false);
        coll.enabled = false;
        AttackCheck = false;
    }

    public void StrongAttack() // 강공격
    {
        if(Input.GetKeyDown(KeyCode.D) && PlayerST >= 5)
        {
            CurrentST = 5;
            StartCoroutine(Slow_ST());
            Stamina_Time = 0;
            AttackCheck = true;
            rigid.velocity = Vector2.zero;
            animator.SetBool("SAttacking", true);
            coll.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Ground") // 착지 체크
        {
            JumpCheck = false;
            JumpCharsh = false;
            animator.SetBool("Jumping", false);
        }

        if (other.gameObject.tag == "Stinger")
        {
            CurrentHp = 5;
            StartCoroutine(Slow_HP());
        }

        if(other.gameObject.tag == "MiniBoss")
        {
            CurrentHp = 30;
            StartCoroutine(Slow_HP());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") // 착지 체크
        {
            JumpCharsh = true;
            //rigid.velocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") // 착지 체크
        {
            JumpCharsh = true;
            //rigid.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") // 착지 체크
        {
            JumpCharsh = false;
            //rigid.velocity = Vector2.zero;
        }
    }

}
