using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool roundRunning = false;
    private Transform target;         // the golf ball
    public GameObject golfBall;
    public Transform spawnPoint;
    public float distance = 5f;      // how far behind the ball
    public float height = 2f;        // how high above the ball
    public float rotationSpeed = 1000f;
    public float circleSpeed = 40f;

    public float circleRadius;
    public Transform mapCenter;
    public float circleHeight;

    private float currentAngle = 0f;

    public static CameraMovement instance;

    public Shader toonShader;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("instance");
        instance = this;
    }

    void Start()
    {
        StartCoroutine(StartRound());
    }

    public Material toonMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, toonMaterial);
    }

    public void Hole()
    {
        distance = 20f;
        height = 10f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator StartRound()
    {
        Debug.Log("startRound");
        if (roundRunning) yield break;
        roundRunning = true;

        float angle = 0f;
        while (angle < 360f)
        {
            angle += circleSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)) * circleRadius + Vector3.up * circleHeight;
            transform.position = Vector3.Lerp(transform.position, mapCenter.position + offset, Time.deltaTime * 6);
            transform.LookAt(mapCenter.position);
            yield return null;
        }

        // Spawn player ball
        GameObject ball = Instantiate(golfBall, spawnPoint.position, spawnPoint.rotation);
        target = ball.transform;
        GameManager.instance.pregameUI.SetActive(false);

        roundRunning = false;

        GameManager.instance.parUnderText.text = "Par: " + GameManager.instance.par.ToString();
    }
    void LateUpdate()
    {
        if (target == null) return;

        // Rotate camera around the ball with input
        float horizontal = Input.GetAxis("Mouse X");
        currentAngle += horizontal * rotationSpeed * Time.deltaTime;

        // Calculate desired offset
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 desiredOffset = rotation * Vector3.back * distance + Vector3.up * height;
        Vector3 desiredPosition = target.position + desiredOffset;

        // Raycast to detect obstacles
        RaycastHit hit;
        if (Physics.Raycast(target.position + Vector3.up * 0.5f, desiredOffset.normalized, out hit, distance))
        {
            // Adjust position to the hit point, slightly closer so camera doesn't clip
            desiredPosition = hit.point - desiredOffset.normalized * 0.2f;
        }

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}
