using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class ControllerConstants
{
    public static readonly int P_DIRX = Animator.StringToHash("DirX");
    public static readonly int P_DIRZ = Animator.StringToHash("DirZ");
}

public enum ActorTeam
{
    Enemy,
    Player
}

public class GameDirector : MonoBehaviour
{
    private static readonly int P_GATETRIGGER = Animator.StringToHash("GateTrigger");

    public LayerMask playerLayer;
    public LayerMask enemyLayer;

    public Transform nextExitMarker;

    public Animator nextExitGate;
    public Animator lastExitGate;

    public List<GameObject> rooms;

    public List<EnemyController> Enemies { get => _enemies; }

    private PlayerController _player;

    private GameObject _currentRoom;

    private List<EnemyController> _enemies;
    private int _enemyCount = 0;

    private int _roomNumber = 0;

    public int Score { get => _roomNumber; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        _enemies = new List<EnemyController>();

        _currentRoom = GameObject.FindGameObjectWithTag("Room");

        Invoke(nameof(InitNextRoom), 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        var gatePos = nextExitGate.transform.position;
        Gizmos.DrawSphere(gatePos, 2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void EnterNextRoom()
    {
        lastExitGate.SetTrigger(P_GATETRIGGER);

        foreach (var enemy in _enemies)
        {
            enemy.target = _player.transform;
        }
    }
    public void ReportDeath(DamageModel dmg)
    {
        if (dmg.team == ActorTeam.Enemy)
        {
            _enemies.Remove(dmg.gameObject.GetComponent<EnemyController>());

            _enemyCount = Mathf.Clamp(_enemyCount - 1, 0, int.MaxValue);

            if (_enemyCount == 0)
            {
                _roomNumber++;

                if (_roomNumber % 3 == 0)
                {
                    var plyrDmg = _player.GetComponent<DamageModel>();
                    plyrDmg.health = plyrDmg.maxHealth;

                    var plyrAtk = _player.GetComponent<AttackModel>();
                    plyrAtk.multiplier += plyrAtk.multiplier * .1f * plyrAtk.multiplier * .1f * plyrAtk.multiplier * .1f;
                    plyrAtk.attackRadius +=  plyrAtk.attackRadius * .15f * plyrAtk.attackRadius * .15f * plyrAtk.attackRadius * .15f;
                }

                InitNextRoom();
            }
        }
        else if (dmg.team == ActorTeam.Player)
        {
            Invoke(nameof(ResetGame), 3f);
        }
    }

    private void InitNextRoom()
    {
        var nextRoom = Instantiate(rooms[UnityEngine.Random.Range(0, rooms.Count)]);

        var entryPoint =  nextRoom.transform.Find("EntryPoint");
        entryPoint.SetParent(null);
        nextRoom.transform.SetParent(entryPoint);

        entryPoint.position = nextExitMarker.position;

        nextRoom.transform.SetParent(null);
        entryPoint.SetParent(nextRoom.transform);

        var midPoint = nextRoom.transform.Find("MidPoint");

        _enemyCount = _roomNumber + 1;

        for (int i = 0; i < _enemyCount; i++)
        {
            var theta = 2 * Mathf.PI / _enemyCount * i;
            var circlePos = new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta));

            var clone = Instantiate(_player.gameObject, midPoint.position + circlePos, Quaternion.LookRotation(Vector3.back));
            clone.layer = LayerMask.NameToLayer("Enemy");

            Destroy(clone.GetComponent<PlayerController>());

            var enemy = clone.AddComponent<EnemyController>();
            enemy.director = this;
            _enemies.Add(enemy);

            clone.TryGetComponent<Actor>(out var actor);
            actor.rotationSpeed = 180f;

            clone.TryGetComponent<AttackModel>(out var attack);
            attack.multiplier = 1f;
            attack.enemyLayer = playerLayer;
            attack.team = ActorTeam.Enemy;

            clone.TryGetComponent<DamageModel>(out var dmg);
            dmg.health = dmg.maxHealth;
            dmg.team = ActorTeam.Enemy;

            var ui = clone.GetComponent<CharUI>();
            ui.showScore = false;
        }

        nextExitGate.SetTrigger(P_GATETRIGGER);

        var nextExitDetector = nextRoom.GetComponentInChildren<ExitDetector>();
        nextExitDetector.director = this;

        lastExitGate = nextExitGate;
        nextExitGate = nextRoom.transform.Find("GateExit").GetComponent<Animator>();

        nextExitMarker = nextRoom.transform.Find("ExitPoint");

        _currentRoom = nextRoom;
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
