using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    [Header("Physics")]
    public float maxPower = 20f;
    public float chargeSpeed = 10f;
    private float power;
    private Rigidbody rb;
    private bool isCharging = false;
    private bool isGrounded;
    private bool isMoving = true;
    public float frictionCoefficient;
    public static PlayerMovement Instance;

    // For bounce cooldown
    private float lastBouncePlayTime = -1f;
    [SerializeField] private float bounceCooldown = 0.12f;

    [Header("Game Logic")]
    public int puts;
    public Gradient putPowerColor;
    private Transform club;
    private Vector3 lastPos;

    [Header("Particle Systems and GUI")]
    private Slider powerBar;
    private GameObject gui;
    private ParticleSystem putEffect;
    private Transform marker;
    private TextMeshProUGUI putsText;
    public AudioClip bounce;

    // For velocity-based bounce detection
    private Vector3 lastVelocity;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        powerBar = GameManager.instance.powerBar;
        gui = GameManager.instance.gui;
        putEffect = GameManager.instance.putEffect;
        marker = GameManager.instance.marker;
        putsText = GameManager.instance.putsText;
        powerBar.maxValue = maxPower;
        club = GameManager.instance.club;
        Instance = this;
    }

    void Start()
    {
        lastPos = transform.position;
        putsText.gameObject.SetActive(true);

    }
    void Update()
    {

        HandlePut();
        HandleGUI();

        if (transform.position.y < -40)
        {
            Die();
        }
    }

    void SetupClub()
    {
        Vector3 pos = (CameraMovement.instance.transform.position - transform.position).normalized * 0.25f;
        pos.y = transform.position.y;
        Vector3 desiredPos = pos + transform.position + (club.transform.forward * 0.5f);

        club.gameObject.SetActive(true);
        club.position = desiredPos;
        Vector3 rot = CameraMovement.instance.transform.rotation.eulerAngles + new Vector3(0, -90, 0);
        club.rotation = Quaternion.Euler(0, rot.y, 0);

    }

    void UpdateClub()
    {
        Vector3 pos = (CameraMovement.instance.transform.position - transform.position).normalized * 0.25f;
        pos.y = 0f;
        if (power > 1) pos *= power / 3;
        pos.y = transform.position.y;
        Vector3 desiredPos = pos + transform.position + (club.transform.forward * 0.5f);
        club.position = desiredPos;
    }

    void HandleGUI()
    {
        if (isGrounded && rb.velocity.magnitude < 0.1f && !gui.activeSelf)
        {
            powerBar.value = 0;
            powerBar.fillRect.GetComponent<Image>().color = putPowerColor.Evaluate(0f);
            gui.SetActive(true);
            isMoving = false;
            marker.gameObject.SetActive(true);
        }
        else if (rb.velocity.magnitude >= 0.1f && gui.activeSelf)
        {
            gui.SetActive(false);
            isMoving = true;
            if (marker.gameObject.activeSelf)
            {
                marker.gameObject.SetActive(false);
            }
        }
    }

    void HandlePut()
    {
        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.magnitude < 0.1f && isMoving == false)
        {
            isCharging = true;
            power = 0f;
            SetupClub();
        }

        // While holding space, increase power
        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            power += chargeSpeed * Time.deltaTime;
            power = Mathf.Min(power, maxPower);
            powerBar.value = power;
            powerBar.fillRect.GetComponent<Image>().color = putPowerColor.Evaluate(power / maxPower);
            UpdateClub();
        }

        // On release, shoot
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Shoot();
            isCharging = false;
            isMoving = true;
            marker.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < 0.1f)
        {
            marker.transform.position = transform.position - new Vector3(0, 0.125f, 0);
        }

        // Detect bounce by velocity direction flip or sudden slowdown
        if (lastVelocity.sqrMagnitude > 0.01f && rb.velocity.sqrMagnitude > 0.01f)
        {
            float dot = Vector3.Dot(lastVelocity.normalized, rb.velocity.normalized);

            // If velocity reversed sharply or slowed suddenly
            if (dot < -0.2f || (lastVelocity.magnitude - rb.velocity.magnitude) > 1f)
            {
                if (Time.time - lastBouncePlayTime > bounceCooldown)
                {
                    PlayBounce();
                    lastBouncePlayTime = Time.time;
                }
            }
        }

        lastVelocity = rb.velocity;
    }

    void Shoot()
    {
        club.GetComponent<Club>().SetTarget(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        lastPos = transform.position;
    }

    public void Push()
    {
        putEffect.transform.position = transform.position;
        putEffect.Play();
        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0;

        rb.AddForce(direction.normalized * power, ForceMode.Impulse);
        gui.SetActive(false);

        puts++;
        putsText.text = "Strokes: " + puts;
        float pitch = Random.Range(-0.5f, 0.5f);
        GetComponent<AudioSource>().volume = power / maxPower; ;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().volume = 1;


        club.gameObject.SetActive(false);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            Die();
        }
    }

    private void PlayBounce()
    {
        var source = GetComponent<AudioSource>();
        source.pitch = Random.Range(0.5f, 1f);
        source.PlayOneShot(bounce, rb.velocity.magnitude / 30f);
    }

    void Die()
    {
        transform.position = lastPos;
        puts += 1;
        putsText.text = "Strokes: " + puts;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hole"))
        {
            GameManager.instance.Came(puts);
        }
    }
}