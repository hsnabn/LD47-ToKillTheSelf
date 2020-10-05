using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharUI : MonoBehaviour
{
    public GameDirector director;

    public Canvas canvas;

    public Slider healthBar;

    public TextMeshProUGUI scoreText;

    private DamageModel _dmg;
    private AttackModel _atk;

    private Camera _cam;

    public bool showScore = false;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _dmg);
        TryGetComponent(out _atk);

        _cam = Camera.main;

        if (!showScore)
            scoreText.gameObject.SetActive(false);
    }

    private void Update()
    {
        healthBar.value = (float)_dmg.health / (float)_dmg.maxHealth;

        scoreText.text = director.Score.ToString();

        canvas.transform.rotation = _cam.transform.rotation;
    }
}
