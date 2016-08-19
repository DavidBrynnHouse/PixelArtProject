using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private bool _IsFacingRight;
    private CharacterController2D _controller;
    private float _normalizedHorizontalSpeed;
    private Collider2D _collider2D;

    public float MaxSpeed = 8;
    public float SpeedAccelerationOnground = 10f;
    public float SpeedAccelerationInAir = 5f;
    public Animator Animator;

    public bool IsDead { get; private set; }

    public void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _IsFacingRight = transform.localScale.x > 0;
        _collider2D = GetComponent<Collider2D>();
    }

    public void Update()
    {
        if(!IsDead)
            HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnground : SpeedAccelerationInAir;

        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        Animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x) / MaxSpeed);
    }

    public void Kill()
    {
        _controller.HandleCollisions = false;
        _collider2D.enabled = false;
        IsDead = true;
        _controller.SetForce(new Vector2(0, 20));
    }

    public void RespawnAt(Transform spawnPoint)
    {
        if (!_IsFacingRight)
            Flip();

        IsDead = false;
        _collider2D.enabled = true;
        _controller.HandleCollisions = true;

        transform.position = spawnPoint.position;

    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_IsFacingRight)
                Flip();
        }
        else if(Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if (_IsFacingRight)
                Flip();
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }

        if (_controller.CanJump && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _IsFacingRight = transform.localScale.x > 0;
    }

}
