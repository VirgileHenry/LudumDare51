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
    
    [Range(0, 1)]
    public float mvt_responsivity = 1f;
    
    public Vector2 camera_pos_offset = new Vector2(0, 0);
    public Vector2 camera_rot_offset = new Vector2(0, 0);

    public float fuel_qte = 0f;

    private Rigidbody player_rigidbody;
    private Transform body;
    private Transform Camera;
    private PlayerHolding holder;

    // Start is called before the first frame update
    void Start()
    {
        Camera = this.transform.Find("Camera");

        body = this.transform.Find("Body");
        player_rigidbody = body.GetComponent<Rigidbody>();
        holder = this.GetComponent<PlayerHolding>();
    }

    // Update is called once per frame
    void Update()
    {

        angle += Input.GetAxis("Horizontal") * ang_speed;
        Quaternion player_rot = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * ang_speed, body.position);
        Quaternion orientation = Quaternion.FromToRotation(body.up, body.position);
        body.rotation = player_rot * orientation * body.rotation;
        Vector3 move_displacement = body.forward * Time.deltaTime * (holder.currentState == PlayerHolding.HoldingState.Spaceship ? carryingSpeed : speed) * Input.GetAxis("Vertical");
        body.position = Vector3.Normalize(body.position - move_displacement);

        Quaternion toward_body_quat = body.rotation;
        Quaternion offset_quat = Quaternion.AngleAxis(camera_rot_offset.x, body.right);

        Camera.rotation = Quaternion.AngleAxis(180f, body.up) * offset_quat * toward_body_quat;

        Vector3 camera_desired_pos = body.position + camera_pos_offset.x * Camera.forward + camera_pos_offset.y * Camera.up;

        Camera.position = Vector3.Lerp(
            Camera.position,
            camera_desired_pos,
            mvt_responsivity
        );
    }

    private void OnTriggerStay(Collider other)
    {
        SpaceshipPart spaceship;
        Debug.Log("oui");
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

    }

}
