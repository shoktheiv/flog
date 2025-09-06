using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string nextHole;
    public int par;
    public string holeName;

    public Dictionary<int, string> score = new Dictionary<int, string>();
    public Dictionary<int, Color> scoreColors = new Dictionary<int, Color>();
    public TextMeshProUGUI scoreText;


    [Header("Particle Systems and GUI")]
    public Slider powerBar;
    public GameObject gui;
    public ParticleSystem putEffect;
    public Transform marker;
    public TextMeshProUGUI putsText;
    public Transform club;

    public GameObject pregameUI;
    public TextMeshProUGUI holeNameText;
    public TextMeshProUGUI parText;
    public TextMeshProUGUI parUnderText;
    public GameObject win;

    public ParticleSystem winEffect;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        holeNameText.text = holeName;
        parText.text = "Par: " + par.ToString();
        pregameUI.SetActive(true);

        score.Clear();
        scoreColors.Clear();
        score.Add(0, "Par");
        score.Add(-3, "Albatross!");
        score.Add(-2, "Eagle!");
        score.Add(-1, "Birdie!");
        score.Add(1, "Bogey");
        score.Add(2, "Double Bogey");
        score.Add(3, "Triple Bogey :(");

        // Add score color mappings
        scoreColors.Add(-3, Color.blue); // Albatross
        scoreColors.Add(-2, Color.yellow); // Eagle
        scoreColors.Add(-1, new Color(0.5f, 1f, 0.5f)); // Birdie (light green)
        scoreColors.Add(0, Color.green); // Par
        scoreColors.Add(1, Color.red); // Bogey
        scoreColors.Add(2, Color.red); // Double Bogey
        scoreColors.Add(3, Color.red); // Triple Bogey
    }

    public void Came(int strokes)
    {
        string title = "";
        Color c = Color.white;
        if (strokes > 1 && strokes - par <= 3)
        {
            int s = strokes - par;
            title = score[s];
            c = scoreColors[s];
        }
        else if (strokes <= 1)
        {
            title = "Hole In One!";
            c = Color.yellow;
        }
        else if (strokes - par > 3)
        {
            title = strokes + " Strokes ???";
            c = Color.red;
        }
        else if (strokes == par)
        {
            title = "Par";
            c = scoreColors[0];
        }

        scoreText.text = title;
        scoreText.color = c;
        win.SetActive(true);
        gui.SetActive(false);
        PlayerMovement.Instance.gameObject.SetActive(false);

        CameraMovement.instance.Hole();
        winEffect.Play();
    }
}
