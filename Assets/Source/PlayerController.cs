using UnityEngine;

using static ControllerConstants;

[RequireComponent(typeof(Actor), typeof(DamageModel), typeof(AttackModel))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Camera _cam;
    private Actor _actor;
    private Animator _anim;
    private AttackModel _attack;

    void Start()
    {
        _cam = Camera.main;

        TryGetComponent(out _actor);
        TryGetComponent(out _anim);
        TryGetComponent(out _attack);
    }

    void Update()
    {
        var input = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical")
        );

        var dir = _cam.transform.TransformDirection(input);
        dir.y = 0f;

        var moving = dir.sqrMagnitude > 0.1f;

        var jump = Input.GetKeyDown(KeyCode.Space);

        _actor.Move(dir, jump, !moving);
        _actor.Rotate(_cam.transform.forward);

        var attack = Input.GetMouseButtonDown(0);

        if (attack)
            _attack.DoAttack(_cam.transform.forward);

        _anim.SetFloat(P_DIRX, input.x);
        _anim.SetFloat(P_DIRZ, input.z);
    }
}
