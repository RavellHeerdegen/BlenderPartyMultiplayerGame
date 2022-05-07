using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{

    public CharacterController2D controller;
    public Animator animator;

    public Rigidbody2D rb;
    
    private Timer timer;

    public float movementSpeed = 30f;

    public float horizontal = 0f;
    public float vertical = 0f;

    public bool jumping = false;

    public bool dashing = false;

    public bool dashReady = true;


    public override void OnStartServer()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public override void OnStartClient()
    {
        Debug.Log("Unfreezing Game Time");
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (timer == null)
        {
            try
            {
                timer = GameObject.Find("TimerCanvasNEU").GetComponent<Timer>();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
        }

        if (!hasAuthority) { return; }

        if (timer.movementBlocked) { return; }

        if (Input.GetButtonDown("Dash") && dashReady)
        {
            this.dashing = true;
            dashReady = false;
            Invoke("DashReadyToggle", 0.5f); // War vorher nur 1f
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.jumping = true;
        }
        
    }

    void FixedUpdate()
    {
        if (timer == null)
        {
            try
            {
                timer = GameObject.Find("TimerCanvasNEU").GetComponent<Timer>();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
        }

        if (!hasAuthority) { return; }

        if (timer.movementBlocked) { return; }

        horizontal = Input.GetAxisRaw("Horizontal"); // 1 oder -1 * 30
        vertical = Input.GetAxisRaw("Vertical"); // 1 oder -1 * 30

        controller.Move(horizontal * movementSpeed * Time.deltaTime, vertical * movementSpeed * Time.deltaTime, jumping, dashing);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        // MOVEMENT MOVEMENT MOVEMENT
        //CmdMove();

        jumping = false;
        dashing = false;
    }


    private void DashReadyToggle()
    {
        dashReady = true;
    }


    [Command]
    void CmdMove()
    {
        // Verarbeitung des Rigidbodys
        RpcMove();

    }

    [ClientRpc]
    void RpcMove()
    {
        // Aktualisier Rigidbody auf allen Clients

    }
}
