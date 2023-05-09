using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private TrailRenderer dashEffect;
    private Rigidbody2D rb;

    [Header("Move")]
    public float moveSpeed = 3;

    [Header("Jump")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;
    public float jumpForce = 10;

    [Header("Fall")]
    public float fallMultiplier;
    private float gravityScale;

    [Header("Dash")]
    public bool canDash;
    public bool isDashing = false;
    public float dashForce = 14;
    public float dashTime = 0.5f;
    public float decelerationTime = 0.25f;
    private Vector2 dashDir;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScale = rb.gravityScale;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        Dash(horizontal, vertical);

        //If player grounded
        if (IsGround())
        {
            canDash = true;
            Jump();
        }

        #region Fall Gravity

        //Jika player terjun , maka gravityScale * multiplier
        if (rb.velocity.y < 0 && isDashing == false)
        {
            rb.gravityScale = gravityScale * fallMultiplier;
        }
        //Jika player naik , maka gravityScale = normal
        else if (rb.velocity.y >= 0 && isDashing == false)
        {
            rb.gravityScale = gravityScale;
        }

        #endregion


        Flip(horizontal);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    #region  Movement
    private void Flip(float horizontal)
    {
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    #endregion

    #region Jump
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // isGrounded = false;
        }
    }

    private bool IsGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    #endregion

    #region Dash

    private void Dash(float horizontal, float vertical)
    {
        float gravity = rb.gravityScale;

        if (Input.GetButtonDown("Dash") && canDash)
        {
            isDashing = true;
            canDash = false;
            dashEffect.emitting = true;
            dashDir = new Vector2(horizontal, vertical);
            rb.gravityScale = 0;

            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(StopDash(gravity, dashDir));
        }

        if (isDashing)
        {
            rb.velocity = dashDir.normalized * dashForce;
            return;
        }
    }

    IEnumerator StopDash(float gravity, Vector2 direction)
    {
        // Add counter force to stop upward dash
        if (direction.y > 0)
        {
            StartCoroutine(Decelerate());
        }

        yield return new WaitForSeconds(dashTime);

        dashEffect.emitting = false;
        isDashing = false;

        rb.gravityScale = gravity;
    }

    //Reduce upward dash
    private IEnumerator Decelerate()
    {
        yield return new WaitForSeconds(dashTime);

        float t = 0;
        Vector2 velocity = rb.velocity;
        while (t < decelerationTime)
        {
            t += Time.deltaTime;
            rb.velocity = Vector2.Lerp(velocity, Vector2.zero, t / decelerationTime);
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }

    #endregion

    #region Trigger & Collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dash Reset"))
        {
            canDash = true;
            other.gameObject.SetActive(false);

            StartCoroutine(SetActiveInSeconds(other.gameObject));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Restart();
        }
    }

    #endregion

    #region Debuging
    private void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator SetActiveInSeconds(GameObject dashReset)
    {
        yield return new WaitForSeconds(3);
        dashReset.SetActive(true);
    }

    #endregion
}