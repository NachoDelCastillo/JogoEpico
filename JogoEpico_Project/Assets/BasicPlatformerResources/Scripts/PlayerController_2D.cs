using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using DG.Tweening;

public class PlayerController_2D : MonoBehaviour
{

    // Components
    Rigidbody2D rb;
    [SerializeField] SpriteRenderer gfx;
    [SerializeField] Animator anim;

    // Movement
    float input_hor;
    float input_ver;
    int dir = 1;
    [SerializeField] float speed;

    // Jump
    [SerializeField] float jumpForce;
    float smoothTime = .1f;
    private Vector3 m_Velocity = Vector3.zero;
    bool canDoubleJump;
    float jumpRemember = .2f;
    float jumpRememberTimer = -1;

    float groundRemember = .2f;
    float groundRememberTimer = -1;

    // Ground checker
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] LayerMask groundLayer;
    bool onGround;
    float groundCheck_radius = .2f;


    // Dash
    [SerializeField] CapsuleCollider2D normalCollider;
    [SerializeField] CapsuleCollider2D dashCollider;
    bool pressingDash;
    float dashForce = 30;


    // Shoot
    [SerializeField] Transform standPoint;
    [SerializeField] Transform crouchPoint;
    [SerializeField] GameObject bullet;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateFlipGfx();

        Jump_check();


        // Limit down velocity
        if (rb.velocity.y < -30)
            rb.velocity = new Vector2(rb.velocity.x, -30);


        //float maxVelocity = 
        //else if (rb.velocity.y > 21)
        //    rb.velocity = new Vector2(rb.velocity.x, 21);


        // Update animator
        if (input_hor > .1f || input_hor < -.1f)
            anim.SetFloat("HorVel", Mathf.Abs(rb.velocity.x));
        else
            anim.SetFloat("HorVel", 0);


        anim.SetFloat("VerVel", rb.velocity.y);

        anim.SetBool("OnGround", onGround);

        anim.SetBool("Dashing", pressingDash);
    }

    #region Input

    public void Jump_Input(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpRememberTimer = jumpRemember;

        if (context.canceled && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .55f);
    }

    public void Dash_Input(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pressingDash = true;

            normalCollider.enabled = false;
            dashCollider.enabled = true;

            rb.velocity = new Vector3(dashForce * Mathf.Sign(gfx.transform.localScale.x), rb.velocity.y, 0);
        }

        if (context.canceled)
        {
            pressingDash = false;

            normalCollider.enabled = true;
            dashCollider.enabled = false;
        }

        if (context.started || context.canceled)
            Debug.Log("pressingDash = " + pressingDash);
    }

    public void Shoot_Input(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 bulletSpawnPoint;

            if (!pressingDash)
                bulletSpawnPoint = standPoint.position;
            else
                bulletSpawnPoint = crouchPoint.position;

            Bullet newBullet = Instantiate(bullet, bulletSpawnPoint, Quaternion.identity).GetComponent<Bullet>();
            newBullet.dir = (int) gfx.transform.localScale.x;
            newBullet.speed = 2000;

            newBullet.gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        }
    }

    #endregion

    #region Movility

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        //Debug.Log("context.ReadValue<Vector2>().x = " + context.ReadValue<Vector2>().x);

        input_hor = context.ReadValue<Vector2>().x;
        input_ver = context.ReadValue<Vector2>().y;
    }

    void UpdateFlipGfx()
    {
        // Cambiar direccion actual
        if (input_hor != 0)
        {
            if (input_hor < 0)
            { dir = -1; gfx.transform.localScale = new Vector3(-1, 1, 0); }
            else
            { dir = 1; gfx.transform.localScale = new Vector3(1, 1, 0); }
        }
    }

    void FixedUpdate()
    {
        Move();

        CheckGround();
    }

    void Move()
    {

        float realInputHor = input_hor;
        float realInputVer = input_ver;

        //if (Attacking())
        //{
        //    if (attackingGround)
        //    { realInputHor = 0; realInputVer = 0; }

        //    else if (attackingAir)
        //    { }
        //}

        if (pressingDash)
            realInputHor = 0;


        Vector3 targetVelocity = new Vector2(realInputHor * speed, rb.velocity.y);
        // And then smoothing it out and applying it to the character


        if (!pressingDash)
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, smoothTime);
        else
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, .4f);
    }

    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck_tr.position, groundCheck_radius, groundLayer);

        if (colliders.Length == 0)
            onGround = false;
        else
        {
            onGround = true;
            canDoubleJump = true;

            groundRememberTimer = groundRemember;

            // Se da por finalizado el salto despues del dash
        }

        //if (!onGround_Remember && onGround && !isRespawning)
        //    AudioManager_PK.instance.Play("Fall", Random.Range(.3f, .6f));

        onGround_Remember = onGround;
    }

    bool onGround_Remember;

    void Jump_check()
    {
        jumpRememberTimer -= Time.deltaTime;
        groundRememberTimer -= Time.deltaTime;

        if (rb.velocity.y > 4)
            groundRememberTimer = 0;

        if (jumpRememberTimer > 0 && groundRememberTimer > 0)
        {
            jumpRememberTimer = 0;
            groundRememberTimer = 0;

            onGround = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));

            // AudioManager_PK.instance.Play("Jump", 1);
        }

        if (jumpRememberTimer > 0 && canDoubleJump)
        {
            jumpRememberTimer = 0;

            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));
        }
    }

    #endregion

}
