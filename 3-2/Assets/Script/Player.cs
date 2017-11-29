using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Player : MonoBehaviour {

    Animator animator;
    Rigidbody2D rigid;
    public Collider2D coll;
    public GameObject BossPos;
    public SpriteRenderer Shadow;
    SkeletonAnimator skeleton;
    Boss boss;

    public float PlayerHP = 100;
    public float PlayerST = 100;

    float CurrentHp;
    float CurrentST;
    float y = 0; //피채워줌 별거없음

    float JumpPower = 60; // 점프 파워
    public float AttackDamage = 0;

    bool JumpCheck; // 이중점프 체크
    bool JumpCharsh; // 점프중 부딫힘
    bool AttackCheck;
    bool NextAttackCheck;
    bool GuardCheck;
    bool HitCheck;
    bool UnHitCheck;
    bool RushHit;


    public float Stamina_Time = 0f; //스태미나 차는 시간 체크
    public bool RollingCheck; // 구르기 체크

    int LR_Check = 1; // 좌우 체크
    int HitRandNum;

    // Use this for initialization
    void Awake () {

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(Recovery_ST());
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        skeleton = GetComponent<SkeletonAnimator>();
    }

    void FixedUpdate()
    {

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

        if(!RollingCheck && !HitCheck) //구르는 중이 아니면 움직임 
        {
            if (!AttackCheck && !HitCheck)
            {
                Moving();
            }
            StrongAttack();
        }

        if (!JumpCheck && !AttackCheck && !HitCheck)
        {
            Rolling(); // 구르기
        }

        if (Input.GetKeyDown(KeyCode.S) && PlayerST >= 10 && !HitCheck)
        {
            if (!NextAttackCheck)
            {
                AttackDamage = 20;
                NextAttackCheck = true;
                coll.enabled = true;
                StartCoroutine(Co_WAttack());
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }

            else if(NextAttackCheck && PlayerST >= 15)
            {
                AttackDamage = 22;
                StartCoroutine(Co_WAttack02());
                CurrentST = 15;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !JumpCheck && PlayerST >= 5 && !HitCheck) //점프 버튼 
        {
            animator.SetBool("Jumping", true);
            JumpCheck = true;
            Jumping();
        }

        if(Input.GetKeyDown(KeyCode.A) && !HitCheck) // 가드 하기
        {
            StartCoroutine(Guard());
        }

        else if(Input.GetKeyUp(KeyCode.A)) // 가드 풀기
        {
            StartCoroutine(UnGuard());
        }
    }

    IEnumerator Guard() //가드 하기
    {
        animator.SetBool("Guard", true);
        GuardCheck = true;
        yield return null;
    }

    IEnumerator UnGuard() // 가드 풀기
    {
        animator.SetBool("Guard", false);
        GuardCheck = false;
        yield return null;
    }

    IEnumerator Recovery_ST() //스태미너 회복 
    {
        do
        {
            if (Stamina_Time > 0.3f && PlayerST < 100)
            {
                PlayerST += 1.5f;
            }
            yield return new WaitForSeconds(0.001f);
        } while (true);
    }

    IEnumerator Slow_ST() //천천히 다는 스태미너
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

        if (Input.GetKey(KeyCode.LeftArrow) && !AttackCheck )
        {
            animator.SetBool("Running", true);
            transform.localScale = new Vector3(-0.8f, 0.8f, 1);
            LR_Check = 0;
            movepowers = -7;
        }

        else if (Input.GetKey(KeyCode.RightArrow) && !AttackCheck)
        {
            animator.SetBool("Running", true);
            movepowers = 7;
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
            LR_Check = 1;
        }
        else
        {
            animator.SetBool("Running", false);
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
        Shadow.enabled = false;
    }


    void Rolling() //구르기 
    {
        var Start = transform.position;
        if (Input.GetKeyDown(KeyCode.LeftShift) && !RollingCheck && PlayerST >= 25) // 구르기 체크
        {
            animator.SetBool("Rolling", true);
            StartCoroutine(RollingState());
            RollingCheck = true;
            CurrentST = 25;
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
            transform.localScale = new Vector3(-0.8f, 0.8f, 1);
            RollVec = Vector3.left * 2.0f;
        }

        else if (LR_Check == 1)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
            RollVec = Vector3.right * 2.0f;
        }
        while (Rollinging < 1f)
        {
            Rollinging += Time.deltaTime / 0.5f;
            transform.position += RollVec * 10f * Time.deltaTime;
            yield return null;
        }

        if(Rollinging >= 1)
        {
            animator.SetBool("Rolling", false);
            RollingCheck = false;
        }

    }

    public void WAttack_StartColl() // 두번째 약공 다시 켜주기
    {
        coll.enabled = true;
    }

    IEnumerator Co_WAttack()
    {

        rigid.velocity = Vector2.zero;
        AttackCheck = true;
        animator.SetBool("WAttack01", true);
        yield return new WaitForSeconds(0.5f);
        NextAttackCheck = false;
        animator.SetBool("WAttack01", false);
        AttackCheck = false;
        coll.enabled = false;
    }

    IEnumerator Co_WAttack02()
    {

        rigid.velocity = Vector2.zero;
        AttackCheck = true;
        animator.SetBool("WAttack02", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("WAttack02", false);
        AttackCheck = false;
        NextAttackCheck = false;
        coll.enabled = false;
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
            AttackDamage = 36;
            CurrentST = 20;
            StartCoroutine(Slow_ST());
            Stamina_Time = 0;
            AttackCheck = true;
            rigid.velocity = Vector2.zero;
            animator.SetBool("SAttacking", true);
            coll.enabled = true;
        }
    }

    public void RushCheck()
    {
        if(RushHit)
        {
            animator.SetBool("RushHit", true);
        }
    }

    public void DeathCheck()
    {
        if(PlayerHP >0)
        {
            animator.SetBool("RushHit", false);
            RushHit = false;
        }
    }

    void Angel_move(Vector3 velocity) //밀어주기
    {
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    void VectorAdd(Vector2 Addforce)
    {
        rigid.AddForce(Addforce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "Stone" && !UnHitCheck && !RollingCheck)
        {
            Vector2 hitvec = Vector2.zero;
            if (!GuardCheck)
            {
                HitRandNum = Random.Range(1, 3);
                animator.SetInteger("HitNum", HitRandNum);
                animator.SetBool("HitCheck", true);
                hitvec = new Vector2(-4f, 7f);
                CurrentHp = 10;
                StartCoroutine(Slow_HP());
                
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
            }

            else
            {
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }
        }

        if(other.gameObject.tag=="Boss" && boss.RushCheck && !RollingCheck && !UnHitCheck)
        {
            if (transform.position.x > BossPos.transform.position.x)
            {
                animator.SetBool("HitCheck", true);
                RushHit = true;
                VectorAdd(new Vector2(120f, 60f));
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
            }

            else
            {
                animator.SetBool("HitCheck", true);
                RushHit = true;
                VectorAdd(new Vector2(-120f, 60f));
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
            }
        }

        if(other.gameObject.tag == "Ground") // 착지 체크
        {
            JumpCheck = false;
            JumpCharsh = false;
            animator.SetBool("Jumping", false);
            Shadow.enabled = true;

            if(RushHit)
            {
                DeathCheck();
            }
        }

        if (other.gameObject.tag == "Stinger" && !UnHitCheck && !RollingCheck)
        {
            Vector2 hitvec2 = Vector2.zero;
            if(!GuardCheck)
            {
                animator.SetBool("HitCheck", true);
                hitvec2 = new Vector2(-7f, 7f);
                CurrentHp = 10;
                StartCoroutine(Slow_HP());
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
            }

            else
            {
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }
            rigid.AddForce(hitvec2, ForceMode2D.Impulse);
        }

        if(other.gameObject.tag == "MiniBoss" && !UnHitCheck && !RollingCheck) 
        {
            Vector2 hitvec3 = Vector2.zero;

            if (!GuardCheck)
            {
                animator.SetBool("HitCheck", true);
                hitvec3 = new Vector2(-4f, 7f);
                CurrentHp = 20;
                StartCoroutine(Slow_HP());
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
            }

            else
            {
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
            }
            rigid.AddForce(hitvec3, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") 
        {
            JumpCharsh = true;
            //rigid.velocity = Vector2.zero;
        }
    }

    void ReHit()
    {
        HitCheck = false;
    }

    IEnumerator NoHitTime()
    {
        UnHitCheck = true;
        int Hited = 0;
        while (Hited < 3)
        {
            if (Hited % 2 == 0)
            {
                skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 0.8f;
            }
            else
                skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 1f;
            yield return new WaitForSeconds(0.1f);
            Hited++;
        }
        skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 1f;
        yield return null;
        UnHitCheck = false;
        animator.SetBool("HitCheck", false);
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
