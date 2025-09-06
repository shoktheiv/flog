using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Canvas : MonoBehaviour
{
    public GameObject gui;
    public Slider powerBar;

    public TextMeshProUGUI scoreText;
    public GameObject pregameUI;
    public TextMeshProUGUI holeNameText;
    public TextMeshProUGUI parText;
    public GameObject win;

    public TextMeshProUGUI par;

    void Start()
    {
        GameManager.instance.scoreText = scoreText;
        GameManager.instance.pregameUI = pregameUI;
        GameManager.instance.holeNameText = holeNameText;
        GameManager.instance.parText = parText;
        GameManager.instance.gui = gui;
        GameManager.instance.powerBar = powerBar;
        GameManager.instance.win = win;
        GameManager.instance.parUnderText = par;
    }

    public void NextHole()
    {
        SceneManager.LoadScene(GameManager.instance.nextHole);
    }
}
