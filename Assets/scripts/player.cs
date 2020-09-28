using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class player : MonoBehaviour
{

    public Animator ani;
    public Transform tran;
    Rigidbody2D rig;
    public float speed = 0.1f;
    public float speed_x_constraint;
    public int RunSpeed;
    public Animator MotionAnimator;
    public LayerMask whatIsGround;
    public bool isFacingRight = true;
    private bool isVine;


    [Header("Jump")]
    //public bool FloorCheck = false;
    //public bool jumptofallcheck = false;
    public float JumpSpeed;
    //public bool jumpingCheck = false;
    //public bool fallingCheck = false;
    //public float fallingSpeed;
    //public float jumpForce;

    private bool canDoubleJump;
    public float DoubleJumpSpeed;

    [Header("Health")]
    public int hp = 4;
    public int maxHp = 4;
    public Material PercentageMat;
    public float health;
    public float maxHealth;

    [Header("Iframe Stuff & Invulnerability")]
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;
    public Collider2D triggerCollider;
    public SpriteRenderer mySprite;
    Renderer rend;
    Color c;
    public float invulnerableTime;

    [Header("Damage Screen")]
    public Color damageColor;
    public Image damageImage;
    float colorSmoothing = 2f;
    bool isTalkingDamage = false;

    [Header("Climb")]
    public GameObject Target;
    private bool isLadder;
    private BoxCollider2D myFeet;
    public float climbSpeed;
    private float PlayerGravity;
    private bool isJumping;


    public float distance;
    public LayerMask whatIsLadder;

    [Header("Platform & Ladder")]
    private bool isFloor;
    private bool isOnWayPlatform;
    public float RestoreTime;


    public void Start()
    {
        rig = this.gameObject.GetComponent<Rigidbody2D>();
        hp = maxHp;
        maxHp = 4;
        PercentageMat.SetFloat("_Percentage", health / maxHealth);
        rend = GetComponent<Renderer>();
        c = rend.material.color;
        myFeet = Target.GetComponent<BoxCollider2D>();
        PlayerGravity = rig.gravityScale;
    }
    void Update()
    {
        CheckAirStatus();
        walk();
        jump();
        Run();
        Check();
        CheckFloor();
        OnWayPlatformCheck();
        SwitchAnimation();
        print (isFloor);
        //JumpToFall();
        //FallingFunction();

        #region HealthBar
        if (rig.velocity.x > speed_x_constraint)
        {
            rig.velocity = new Vector2(speed_x_constraint, rig.velocity.y);
        }
        if (rig.velocity.x < speed_x_constraint)
        {
            rig.velocity = new Vector2(-speed_x_constraint, rig.velocity.y);
        }
        print(rig.velocity);


       
        if(hp == 4)
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
        if(hp == 0)
        {
            health = 314;
            PercentageMat.SetFloat("_Percentage", health / maxHealth);
        }
        #endregion

        if (isTalkingDamage)
        {
            damageImage.color = damageColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, colorSmoothing * Time.deltaTime);
        }
        isTalkingDamage = false;
    }

    private void FixedUpdate()
    {

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
        isVine = myFeet.IsTouchingLayers(LayerMask.GetMask("vine"));
    }
        
    void OnCollisionEnter2D(Collision2D other)
    {
        //if (other.gameObject.tag == "floor")
        //{
        //    FloorCheck = true;
        //    jumpingCheck = true;
        //    Debug.Log("Floor");
        //    MotionAnimator.SetBool("OnFloor", FloorCheck);
        //}

        if (other.gameObject.tag == "monster")
        {
            print(other.gameObject.name);
            hp -= 1;
            Vector2 difference = transform.position - other.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
            StartCoroutine(FlashCollider());
            StartCoroutine(GetInerable());
            isTalkingDamage = true;
        }

        if (other.gameObject.tag == "Trampoline")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 25f);
        }
    }

    void OnCollisionExit2D(Collision2D otherexit)
    { 
        
       // if (otherexit.gameObject.tag == "floor" || isFloor)
       //    {
       //         FloorCheck = false;
       //         Debug.Log("OffFloor");
       //         MotionAnimator.SetBool("OnFloor", FloorCheck);
       //    }
    }

    void OnWayPlatformCheck()
    {
        if(isFloor && gameObject.layer != LayerMask.NameToLayer("player"))
        {
            gameObject.layer = LayerMask.NameToLayer("player");
        }

        float moveY = Input.GetAxis("Vertical");
        if(isOnWayPlatform && moveY < -0.1f)
        {
            gameObject.layer = LayerMask.NameToLayer("OnWayPlatform");
            Invoke("RestorePlayerLayer", RestoreTime);
        }
    }
    void RestorePlayerLayer()
    {
        if(!isFloor && gameObject.layer != LayerMask.NameToLayer("player"))
        {
            gameObject.layer = LayerMask.NameToLayer("player");
        }
    }

    #region Walk
    private void walk()
    {
        bool IsWalking = false;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
            float verticalMoement = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
            Vector3 directionOfMovement = new Vector3(horizontalMovement, verticalMoement, 0);
            gameObject.transform.Translate(directionOfMovement);
            IsWalking = true;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            IsWalking = true;
            Vector3 theScale = tran.localScale;
            theScale.x = -0.3f;
            tran.localScale = theScale;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            IsWalking = true;
            Vector3 theScale = tran.localScale;
            theScale.x = 0.3f;
            tran.localScale = theScale;
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

    #region Jump
    private void jump()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && FloorCheck == true)
        //{
            //jumpingCheck = true;
            //MotionAnimator.SetBool("jumping", jumpingCheck);
            //rig.AddForce(new Vector2(0, JumpSpeed), ForceMode2D.Impulse);
            //ani.SetTrigger("Jump");
            //ani.SetBool("status", false);
            //ani.SetBool("Run", false);
            //ani.SetBool("Jump", true);
        //}

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isFloor)
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
                if(canDoubleJump)
                {
                    //ani.SetBool("DoubleJump", true);
                    Vector2 doubleJumpVel = new Vector2(0.0f,DoubleJumpSpeed);
                    rig.velocity = Vector2.up * doubleJumpVel;
                    canDoubleJump = false;
                }
            }
        }
    }
    void JumpToFall()
    {
       // fallingSpeed = rig.velocity.y;
        //if (fallingSpeed <= 1 && FloorCheck == false)
        //{
            //jumptofallcheck = true;
            //MotionAnimator.SetBool("jumptofall", jumptofallcheck);
            //MotionAnimator.SetBool("jumping", jumpingCheck);
        //}
    }
    void FallingFunction()
    {
        //if (fallingSpeed < 0 && FloorCheck == false && jumptofallcheck == true)
        //{
            //fallingCheck = true;
            //MotionAnimator.SetBool("falling", jumptofallcheck);
            //MotionAnimator.SetBool("OnFloor", FloorCheck);
        //}
    }
    #endregion

    void Climb()
    {
        if(isLadder)
        {
            float moveY = Input.GetAxis("Vertical");
            if(moveY > 0.5 || moveY < 0.5f)
            {
                //myAnim.Setbool("Climbing",true);
                rig.gravityScale = 0.0f;
                rig.velocity = new Vector2(rig.velocity.x, moveY * climbSpeed);
            }
            else
            {
                if(isJumping)
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


    #region Run
    void Run()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetKey(KeyCode.LeftShift) || Input.GetAxisRaw("Vertical") != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal") * RunSpeed * Time.deltaTime;
            float verticalMoement = Input.GetAxisRaw("Vertical") * RunSpeed * Time.deltaTime;
            Vector3 directionOfMovement = new Vector3(horizontalMovement, verticalMoement, 0);
            gameObject.transform.Translate(directionOfMovement);
            Vector3 theScale = tran.localScale;
            theScale.x = 1;
            tran.localScale = theScale;
            ani.SetBool("status", false);
            ani.SetBool("Jump", false);
            ani.SetBool("Run", true);

        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 theScale = tran.localScale;
            theScale.x = -1;
            tran.localScale = theScale;
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



    private IEnumerator FlashCollider()
    {
        int temp = 0;
        triggerCollider.enabled = false;
        while(temp < numberOfFlashes)
        {
            mySprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            mySprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        triggerCollider.enabled = true;
    }

    private IEnumerator GetInerable()
    {
        Physics2D.IgnoreLayerCollision(14,15,true);
        c.a = 0.5f;
        rend.material.color = c;
        yield return new WaitForSeconds(invulnerableTime);
        Physics2D.IgnoreLayerCollision(14,15,false);
        c.a = 1f;
        rend.material.color = c;
    }
    
    void SwitchAnimation()
    {
        ani.SetBool("Idle", false);
        if(ani.GetBool("Jump"))
        {
            if(rig.velocity.y < 0.0f)
            {
                ani.SetBool("Jump", false);
                ani.SetBool("Fall", true);
            }
        }
        else if(isFloor)
        {
            ani.SetBool("Fall", false);
            ani.SetBool("Idle", true);
        }
    }

    void CheckAirStatus()
    {
        isJumping = ani.GetBool("Jump");
    }
}







