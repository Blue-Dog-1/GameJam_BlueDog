using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CharacterController2D : MonoBehaviour
{
    [SerializeField] LayerMask layerMask = 1;
    [Range(0f, 20f)]
    [SerializeField] float m_velocity = 0f;
    [Range(0f, 20f)]
    [SerializeField] float m_jumpForce = 0f;
    [Range(0, 1.5f)]
    [SerializeField] float m_distanse = 0f;
    [SerializeField] Transform Skin = null;
    [SerializeField] int HP = 1000;
    [SerializeField] Transform m_target;
    [SerializeField] Transform m_Head;


    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;
    public UnityEvent OnJumpEvent;


    new Rigidbody2D rigidbody;

    bool m_FacingRight;
    public bool FacingRight => m_FacingRight;

    bool isGrounded;

    new Transform camera;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main.transform;

    }
    private void FixedUpdate()
    {
        RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, Vector2.down, m_distanse, layerMask);
        if (hitinfo)
        {
            if (!isGrounded) OnLanding();
            isGrounded = true;
        }
        else
        {
            if (isGrounded) OnJump();
            isGrounded = false;
        }


        Move(Input.GetAxis("Horizontal"), Input.GetKey(KeyCode.Space));
        CameraTraking();

        m_Head.rotation = m_target.rotation;
    }
    void Move(float move, bool jump)
    {
        transform.Translate((Vector2.right * move) * m_velocity * Time.fixedDeltaTime);
        if (jump && isGrounded)
        {
            rigidbody.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse);
        }
        
        var mouseX = (Screen.width / 2) - Input.mousePosition.x;
        
        if (mouseX > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (mouseX < 0 && m_FacingRight)
        {
            Flip();
        }
    }

    void CameraTraking()
    {
        var pos = transform.position;
        pos.z = camera.position.z;
        camera.position = Vector3.Lerp(pos, camera.position, Time.fixedDeltaTime);
    }
    public void OnJump()
    {
    }
    public void OnLanding()
    {
    }

    private void Flip()
    {
        //var y = (m_FacingRight) ? 0 : 180;
        //Skin.rotation = Quaternion.Euler(0, y, 0);

        m_FacingRight = !m_FacingRight;


        Vector3 theScale = Skin.localScale;
        theScale.x *= -1;
        Skin.localScale = m_Head.localScale = theScale;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody)
        {
            HP -= (int)(collision.rigidbody.mass * collision.rigidbody.velocity.magnitude);
            if (HP <= 0)
            {
                HP = 0;
                GameManager.OnEndGame();
            }
            GameManager.UIController.HP = HP;
        }
    }

   
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * m_distanse);
    }

}

