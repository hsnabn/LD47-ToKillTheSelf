using System.Linq;

using UnityEngine;

using static ControllerConstants;

public class EnemyController : MonoBehaviour
{
    public Transform target;

    public GameDirector director;

    public float attackAngle = 45f;
    public float attackDelayMin = 0.75f;
    public float attackDelayMax = 1.2f;

    private float _nextAttackTime = float.NegativeInfinity;

    private Actor _actor;
    private Animator _anim;
    private AttackModel _attack;

    void Start()
    {
        TryGetComponent(out _actor);
        TryGetComponent(out _anim);
        TryGetComponent(out _attack);
    }

    void Update()
    {
        if (target == null)
            return;

        var theta = 2 * Mathf.PI / director.Enemies.Count * director.Enemies.IndexOf(this);
        var circlePos = new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta));

        var dir = target.position - (transform.position + circlePos);

        var dirNormal = dir.normalized;

        if (dir.sqrMagnitude > (Vector3.one * _attack.attackRadius).sqrMagnitude)
        {
            _actor.Move(dirNormal, false, dir.sqrMagnitude < 0.1f);
        }
        else if (Vector3.Angle(dirNormal, transform.forward) < 4)
        {
            if (Time.time > _nextAttackTime)
            {
                _attack.DoAttack(dirNormal);

                _nextAttackTime = Time.time + Random.Range(attackDelayMin, attackDelayMax);
            }
        }

        var vel = dirNormal;

        _anim.SetFloat(P_DIRX, vel.x);
        _anim.SetFloat(P_DIRZ, vel.z);

        _actor.Rotate(dirNormal);
    }
}
