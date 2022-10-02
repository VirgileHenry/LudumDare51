using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    public float speed = 1f;
    public float carryingSpeed = 1f;
    public float ang_speed = 1f;

    public float angle = 0f;

    public float max_fuel = 5f;

    public Vector3 shakeAmplidtude;
    public float shakeDuration;
    public float shakeSpeed;

    [Range(0, 1)]
    public float mvt_responsivity = 1f;
    
    public Vector2 camera_pos_offset = new Vector2(0, 0);
    public Vector2 camera_rot_offset = new Vector2(0, 0);

    public float fuel_qte = 0f;

    private Rigidbody player_rigidbody;
    private Transform body;
    private Transform Camera;
    private PlayerHolding holder;

    public Transform[] deathChecker;
    public float deatRaycastDistance = 0.1f;

    public bool disabled;
    private float fallout;

    private float last_shake_event;

    // Start is called before the first frame update
    void Start()
    {
        Camera = this.transform.Find("Camera");

        body = this.transform.Find("Body");
        player_rigidbody = body.GetComponent<Rigidbody>();
        holder = this.GetComponent<PlayerHolding>();

        disabled = false;
    }

    public void ShakeCamera()
    {
        last_shake_event = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(disabled) {return;}
        angle += Input.GetAxis("Horizontal") * ang_speed;
        Quaternion player_rot = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * ang_speed, body.position);
        Quaternion orientation = Quaternion.FromToRotation(body.up, body.position);
        body.rotation = player_rot * orientation * body.rotation;
        Vector3 move_displacement = body.forward * Time.deltaTime * (holder.currentState == PlayerHolding.HoldingState.Spaceship ? carryingSpeed : speed) * Input.GetAxis("Vertical");
        body.position = Vector3.Normalize(body.position - move_displacement);

        Quaternion toward_body_quat = body.rotation;
        Quaternion offset_quat = Quaternion.AngleAxis(camera_rot_offset.x, body.right);

        float shake_time = Time.time - last_shake_event;
        fallout = shake_time * Mathf.Exp(-shake_time/shakeDuration);
        Vector3 shake = fallout * new Vector3(Mathf.PerlinNoise(shakeSpeed * Time.time, 0f),
            Mathf.PerlinNoise(shakeSpeed * Time.time, 7f),
            Mathf.PerlinNoise(shakeSpeed * Time.time, 13f));
        shake = Vector3.Scale(shake, shakeAmplidtude);
        Quaternion offset_shake = Quaternion.Euler(shake);

        Camera.rotation = offset_shake * Quaternion.AngleAxis(180f, body.up) * offset_quat * toward_body_quat;

        Vector3 camera_desired_pos = body.position + camera_pos_offset.x * Camera.forward + camera_pos_offset.y * Camera.up;

        Camera.position = Vector3.Lerp(
            Camera.position,
            camera_desired_pos,
            mvt_responsivity
        );

        // check for death
        bool alive = false;
        foreach(Transform startRay in deathChecker) {
            if(Physics.Raycast(startRay.position, -startRay.position, deatRaycastDistance, 1 << 9)) {
                alive = true;
                break;
            }
        }
        if(!alive) {
            StartCoroutine(PlayerDeath());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        SpaceshipPart spaceship;
        if (other.TryGetComponent<SpaceshipPart>(out spaceship))
        {
            if (spaceship.part == 15)
            {
                float previous_fuel_qte = fuel_qte;
                fuel_qte = Mathf.Max(fuel_qte - Time.deltaTime, 0f);
                spaceship.refill(previous_fuel_qte - fuel_qte);
            }
        }
        else
        {
            fuel_qte = Mathf.Min(fuel_qte+ Time.deltaTime, max_fuel);
        }

        GameManager.instance.fuelBar.Set(fuel_qte / 5.0f);

    }

    private void OnTriggerEnter(Collider other)
    {
        SpaceshipPart spaceship;
        if (other.TryGetComponent<SpaceshipPart>(out spaceship))
        {
            if (spaceship.part == 15)
            {
                GameManager.instance.spaceshipPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SpaceshipPart spaceship;
        if (other.TryGetComponent<SpaceshipPart>(out spaceship))
        {
            if (spaceship.part == 15)
            {
                GameManager.instance.spaceshipPanel.SetActive(false);
            }
        }
    }

    private IEnumerator PlayerDeath() {
        disabled = true;
        float duration = 2f;
        float timer = 0f;
        Vector3 position = body.position;
        while(timer < duration) {
            float x = timer / duration;
            float t = -2 * x * x + x + 1;
            body.position = position * t;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.PlayerDefeat();
    }
}
