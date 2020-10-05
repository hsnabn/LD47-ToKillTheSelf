using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Actor : MonoBehaviour
{
    [Min(0f)]
    public float moveSpeed = 10f;

    public Vector3 drag = Vector3.one;

    [Range(0f, 360f)]
    public float rotationSpeed = 45f;

    [Min(0f)]
    public float jumpHeight = 2f;

    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    public Vector3 Velocity { get => _vel; }

    private CharacterController _cc;

    private Vector3 _vel;
    private Vector3 _jumpVel;

    private void Start()
    {
        TryGetComponent(out _cc);

        var jumpTime = Mathf.Sqrt(2 * jumpHeight / gravity.magnitude);
        _jumpVel = transform.up * gravity.magnitude * jumpTime;
    }

    public void Move(Vector3 direction, bool doJump, bool applyDrag)
    {
        var xzVel = direction * moveSpeed;
        var yVel = _vel;

        yVel += gravity * Time.deltaTime;

        if (_cc.isGrounded && doJump)
        {
            yVel += _jumpVel;
        }

        xzVel.y = 0f;
        yVel.x = yVel.z = 0f;

        _vel = xzVel + yVel;

        if (applyDrag)
        {
            _vel.x /= 1 + (drag.x * Time.deltaTime);
            _vel.y /= 1 + (drag.y * Time.deltaTime);
            _vel.z /= 1 + (drag.z * Time.deltaTime);
        }

        _cc.Move(_vel * Time.deltaTime);
    }

    public void Rotate(Vector3 direction)
    {
        var rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }
}