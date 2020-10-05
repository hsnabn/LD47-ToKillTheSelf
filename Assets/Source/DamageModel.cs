using UnityEngine;

public class DamageModel : MonoBehaviour
{
    public int maxHealth;

    public int health;
    public ActorTeam team;

    public float defense;

    public ParticleSystem deathParticles;

    private GameDirector _director;

    private void Start()
    {
        deathParticles = Instantiate(deathParticles, this.transform.position, this.transform.rotation, this.transform);

        _director = FindObjectOfType<GameDirector>();
    }

    public void DoDamage(int amount, ActorTeam attackerTeam)
    {
        if (attackerTeam == this.team)
            return;

        health -= amount - Mathf.CeilToInt(amount * defense);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        _director.ReportDeath(this);

        deathParticles.transform.parent = null;
        deathParticles.Play();
        this.gameObject.SetActive(false);

        Invoke(nameof(Cleanup), 5f);
    }

    private void Cleanup()
    {
        Destroy(this);
        Destroy(deathParticles.gameObject);
    }
}
