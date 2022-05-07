using Mirror;
using UnityEngine;

public class CharacterController2D : NetworkBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [Range(0, 50f)] [SerializeField] private float dashMultiplier = 20f;        // How much dash to multiply to the movement
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

    [Header("Sounds")]
    [SerializeField] private GameObject jumpsound_Prefab;
    [SerializeField] private GameObject dashsound_Prefab;
    [SerializeField] private GameObject runningsound_Prefab;

    private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
    private Transform m_transform;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
    private bool dashUsedInAir = false;

    public Animator animator;
    public bool canStopJumpingAnimation = false;
    public bool canStopDashingAnimation = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_transform = GetComponent<Transform>();
	}


	private void FixedUpdate()
	{
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (canStopDashingAnimation)
                {
                    animator.SetBool("IsDashing", false);
                }

                if (canStopJumpingAnimation)
                {
                    animator.SetBool("IsJumping", false);
                }
            }
        }
    }

	public void Move(float move_horizontal, float move_vertical, bool jump, bool dashing)
	{

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

            if (m_Grounded)
            {
                dashUsedInAir = false;
            }

            Vector3 targetVelocity = new Vector3();
			// Move the character by finding the target velocity
            if (dashing)
            {

                // Check if Dash was used in air
                if (m_Grounded)
                {
                    dashUsedInAir = false;
                    if (!dashUsedInAir)
                    {
                        jump = false;
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                        m_Rigidbody2D.AddForce(Vector2.zero);
                        targetVelocity = getTargetVelocityDash(move_horizontal, move_vertical);
                        //targetVelocity = new Vector2((move * 10f) * dashMultiplier, m_Rigidbody2D.velocity.y);

                        animator.SetBool("IsDashing", true);
                        canStopDashingAnimation = false;
                        Invoke("EnableCanStopDash", 0.3f);

                        CmdPlaySound("dash");
                    }
                }
                else
                {
                    if (!dashUsedInAir)
                    {
                        jump = false;
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                        m_Rigidbody2D.AddForce(Vector2.zero);
                        targetVelocity = getTargetVelocityDash(move_horizontal, move_vertical);
                        //targetVelocity = new Vector2((move * 10f) * dashMultiplier, m_Rigidbody2D.velocity.y);

                        animator.SetBool("IsDashing", true);
                        canStopDashingAnimation = false;
                        Invoke("EnableCanStopDash", 0.3f);

                        CmdPlaySound("dash");
                    }
                    dashUsedInAir = true;
                }

                
            }
            else
            {
                targetVelocity = new Vector2(move_horizontal * 10f, m_Rigidbody2D.velocity.y);
            }
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move_horizontal > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move_horizontal < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// TODO: Force auf 0 setzen
			// Add a vertical force to the player.
			m_Grounded = false;
            animator.SetBool("IsJumping", true);
            canStopJumpingAnimation = false;
            Invoke("EnableCanStopJump", 0.3f);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce)); // Horizontale Force oder Vertikale

            CmdPlaySound("jump");
		}
	}

    private void EnableCanStopJump()
    {
        canStopJumpingAnimation = true;
    }
    private void EnableCanStopDash()
    {
        canStopDashingAnimation = true;
    }


    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private Vector2 getTargetVelocityDash(float move_horiz, float move_vertic)
    {
		//Debug.Log("Vertical:" + move_vertic + " Horizontal:" + move_horiz);

		return new Vector2((move_horiz * 10f) * dashMultiplier, (move_vertic * 3f) * dashMultiplier);
	}

    [Command]
    private void CmdPlaySound(string choice)
    {
        if (choice == "jump")
        {
            // Play jump sound
            GameObject jumpsound_object = Instantiate(jumpsound_Prefab);
            NetworkServer.Spawn(jumpsound_object);
        }
        else if(choice == "dash")
        {
            // Play dash sound
            GameObject dashsound_object = Instantiate(dashsound_Prefab);
            NetworkServer.Spawn(dashsound_object);
        }
    }

}
