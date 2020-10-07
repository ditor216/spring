using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerControl : MonoBehaviour
{
    public Transform tran;
    Rigidbody2D rig;
    public Animator ani;
    private bool isFloor;
    private bool isOnWayPlatform;
    private float RestoreTime;
    private BoxCollider2D myFeet;
    public GameObject Target;
    private bool isLadder;
    public float speed_x_constraint;

    [Header("走路")]
    public float speed = 0.1f; //走路速度
    private SpriteRenderer spr;
    public enum Face { Right, Left };
    public Face face;  //面相方向maybe

    [Header("跑步")]
    public float RunSpeed; //跑步速度

    [Header("跳躍")]
    public float JumpSpeed;
    private bool canDoubleJump;
    public float DoubleJumpSpeed;

    [Header("攀爬(石菇)")]
    public float climbSpeed;
    private bool isJumping;
    private float PlayerGravity;

    [Header("血量條")]
    public int hp = 4;
    public int maxHp = 4;
    public Material PercentageMat;
    public float health;
    public float maxHealth;

    [Header("閃爍效果以及受傷後的短暫無敵時間")]
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;
    public Collider2D triggerCollider;
    public SpriteRenderer mySprite;
    Renderer rend;
    Color c;
    public float invulnerableTime;

    [Header("受傷螢幕效果")]
    public Color damageColor;
    public Image damageImage;
    float colorSmoothing = 2f;
    bool isTalkingDamage = false;



    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        myFeet = Target.GetComponent<BoxCollider2D>();
        spr = this.transform.GetComponent<SpriteRenderer>();
        spr.flipX = true; //開始時面向的方向
        if(spr.flipX)
        {
            face = Face.Right;
        }
        else
        {
            face = Face.Left;
        }
        PlayerGravity = rig.gravityScale;
        hp = maxHp;
        maxHp = 4;
        PercentageMat.SetFloat("_Percentage", health / maxHealth);
        c = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFloor();
        Check();
        OnWayPlatformCheck();
        SwitchAnimation();
        CheckAirStatus();
        healthBar();
        #region 受傷屏幕效果
        if (isTalkingDamage)
        {
            damageImage.color = damageColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, colorSmoothing * Time.deltaTime);
        }
        isTalkingDamage = false;
        #endregion
    }
    private void FixedUpdate()
    {
        walk();
        Run();
        Jump();
        Climb();
    }

    void CheckFloor()
    {
        isFloor = myFeet.IsTouchingLayers(LayerMask.GetMask("floor")) ||
                  myFeet.IsTouchingLayers(LayerMask.GetMask("OnWayPlatform"));
        isOnWayPlatform = myFeet.IsTouchingLayers(LayerMask.GetMask("OnWayPlatform"));
    }
    void Check()
    {
        isLadder = myFeet.IsTouchingLayers(LayerMask.GetMask("ladder"));
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            print(other.gameObject.name);
            hp -= 1;
            Vector2 difference = transform.position - other.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y); // 當角色受到傷害後使其擊退一定距離
            StartCoroutine(FlashCollider()); // 受到傷害角色閃爍渲染效果
            StartCoroutine(GetInerable());   // 受到傷害後的無敵時間
            isTalkingDamage = true;
        }
        if (other.gameObject.tag == "Trampoline") // 彈跳床
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 25f);
        }
    }
    #region 走路
    private void walk()
    {
        bool IsWalking = false;
        if (Input.GetAxisRaw("Horizontal") != 0 )
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
            Vector3 directionOfMovement = new Vector3(horizontalMovement, 0, 0);
            gameObject.transform.Translate(directionOfMovement);
            IsWalking = true;
            spr.flipX = false;
            face = Face.Right;

        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            IsWalking = true;
            spr.flipX = true;
            face = Face.Left;
        }
        if (IsWalking)
        {
            if (ani.GetInteger("status") == 0)
                ani.SetInteger("status", 1);
        }
        else
        {
            if (ani.GetInteger("status") == 1)
                ani.SetInteger("status", 0);
        }
    }
    #endregion
    #region 跑步
    void Run()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal") * RunSpeed * Time.deltaTime;
            Vector3 directionOfMovement = new Vector3(horizontalMovement, 0, 0);
            gameObject.transform.Translate(directionOfMovement);
            spr.flipX = false;
            face = Face.Right;
            ani.SetBool("status", false);
            ani.SetBool("Jump", false);
            ani.SetBool("Run", true);

        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            spr.flipX = true;
            face = Face.Left;
            ani.SetBool("status", false);
            ani.SetBool("Jump", false);
            ani.SetBool("Run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            ani.SetBool("Run", false);
        }
    }
    #endregion
    #region 跳躍
    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            
            if (isFloor)
            {
                ani.SetTrigger("Jump");
                ani.SetBool("status", false);
                ani.SetBool("Run", false);
                ani.SetBool("Jump", true);
                Vector2 jumpVel = new Vector2(0.0f, JumpSpeed);
                rig.velocity = Vector2.up * jumpVel;
                canDoubleJump = true;
            }
            else
            {
                if (canDoubleJump)
                {
                    //ani.SetBool("DoubleJump", true);
                    Vector2 doubleJumpVel = new Vector2(0.0f, DoubleJumpSpeed);
                    rig.velocity = Vector2.up * doubleJumpVel;
                    canDoubleJump = false;
                }
            }
        }
    }
    void SwitchAnimation() //關於跳躍動畫間的切換
    {
        ani.SetBool("Idle", false);
        if (ani.GetBool("Jump"))
        {
            if (rig.velocity.y < 0.0f)
            {
                ani.SetBool("Jump", false);
                ani.SetBool("Fall", true);
            }
        }
        else if (isFloor)
        {
            ani.SetBool("Fall", false);
            ani.SetBool("Idle", true);
        }
    }
    void CheckAirStatus()
    {
        isJumping = ani.GetBool("Jump");
    }
    #endregion
    #region 攀爬(石菇)
    void Climb()
    {
        if (isLadder)
        {
            float moveY = Input.GetAxis("Vertical");
            if (moveY > 0.5 || moveY < 0.5f)
            {
                //myAnim.Setbool("Climbing",true);
                rig.gravityScale = 0.0f;
                rig.velocity = new Vector2(rig.velocity.x, moveY * climbSpeed);
            }
            else
            {
                if (isJumping)
                {
                    //myAnim.Setbool("Climbing",false);
                }
                else
                {
                    //myAnim.Setbool("Climbing",false);
                    rig.velocity = new Vector2(rig.velocity.x, 0.0f);
                }
            }
        }
        else
        {
            rig.gravityScale = PlayerGravity;
        }
    }
    void OnWayPlatformCheck() //玩家層與單向通道層間的切換，以便上下爬
    {
        if (isFloor && gameObject.layer != LayerMask.NameToLayer("player"))
        {
            gameObject.layer = LayerMask.NameToLayer("player");
        }

        float moveY = Input.GetAxis("Vertical");
        if (isOnWayPlatform && moveY < -0.1f)
        {
            gameObject.layer = LayerMask.NameToLayer("OnWayPlatform");
            Invoke("RestorePlayerLayer", RestoreTime);
        }
    }
    void RestorePlayerLayer() //從上層要下爬石菇時的轉化
    {
        if (!isFloor && gameObject.layer != LayerMask.NameToLayer("player"))
        {
            gameObject.layer = LayerMask.NameToLayer("player");
        }
    }
    #endregion
    #region 血量條
    void healthBar()
    {
        if (rig.velocity.x > speed_x_constraint)
        {
            rig.velocity = new Vector2(speed_x_constraint, rig.velocity.y);
        }
        if (rig.velocity.x < speed_x_constraint)
        {
            rig.velocity = new Vector2(-speed_x_constraint, rig.velocity.y);
        }
        print(rig.velocity);



        if (hp == 4)
        {
            health = 0;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
        if (hp == 3)
        {
            health = 70;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
        if (hp == 2)
        {
            health = 211;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
        if (hp == 1)
        {
            health = 285;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
        if (hp == 0)
        {
            health = 314;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
    }
    #endregion
    #region 受到傷害腳色閃爍效果
    private IEnumerator FlashCollider()
    {
        int temp = 0;
        triggerCollider.enabled = false;
        while (temp < numberOfFlashes)
        {
            mySprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            mySprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        triggerCollider.enabled = true;
    }
    #endregion
    #region 受到傷害後的無敵時間
    private IEnumerator GetInerable()
    {
        Physics2D.IgnoreLayerCollision(11, 15, true);//第一個數值與第二個數值分別回主角(player)所在的層級數與怪物(monster)所在的層級數
        c.a = 0.5f;
        rend.material.color = c;
        yield return new WaitForSeconds(invulnerableTime);
        Physics2D.IgnoreLayerCollision(11, 15, false);
        c.a = 1f;
        rend.material.color = c;
    }
    #endregion
}
