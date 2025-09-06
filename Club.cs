using UnityEngine;

public class Club : MonoBehaviour
{

    bool isFiring;
    Vector3 target;
    public float speed;

    public void SetTarget(Vector3 t)
    {
        target = t;
        isFiring = true;
    }

    void FixedUpdate()
    {
        if (isFiring)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, target) < 0.5f)
            {
                isFiring = false;
                PlayerMovement.Instance.Push();
            }
        }
    }
}
