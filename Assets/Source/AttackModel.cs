using System.Linq;

using UnityEngine;

public class AttackModel : MonoBehaviour
{
    public LayerMask enemyLayer;

    public ParticleSystem attackParticles;

    public ActorTeam team;

    public int damagePotential;
    public float attackRadius;

    public float multiplier = 1f;

    private Collider _selfCollider;
    private Transform _particleTransform;

    private float nextAttackAllowedTime = float.NegativeInfinity;

    private void Start()
    {
        TryGetComponent(out _selfCollider);

        attackParticles = Instantiate(attackParticles, this.transform.position, this.transform.rotation, this.transform);

        _particleTransform = attackParticles.transform;
    }

    private void OnDrawGizmos()
    {
        if (!_selfCollider)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_selfCollider.bounds.center + (transform.forward * attackRadius), attackRadius);
    }

    public void DoAttack(Vector3 direction)
    {
        if (Time.time < nextAttackAllowedTime)
            return;

        var emitPos = _selfCollider.bounds.center + (direction * attackRadius);
        var shape = attackParticles.shape;
        shape.radius = attackRadius;

        var main = attackParticles.main;
        main.startSpeedMultiplier = -(attackRadius * attackRadius * attackRadius * attackRadius);
        attackParticles.Emit(new ParticleSystem.EmitParams{position=emitPos,applyShapeToPosition=true}, 32);

        var amt = Mathf.FloorToInt(damagePotential * multiplier);

        var col = Physics.OverlapSphere(_selfCollider.bounds.center + (direction * attackRadius), attackRadius, enemyLayer);
        foreach (var dmg in col.Select(c => c.GetComponent<DamageModel>()))
        {
            dmg.DoDamage(amt, this.team);

            if (dmg.team == this.team && dmg.TryGetComponent<AttackModel>(out var atk))
            {
                atk.nextAttackAllowedTime = Time.time + .5f;
            }
        }
    }
}
