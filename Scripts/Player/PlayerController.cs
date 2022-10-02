using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    public float speed = 1f;
    public float ang_speed = 1f;
    public float gravity_intensity = 1f;

    public float angle = 0f;
    
    [Range(0, 1)]
    public float mvt_responsivity = 1f;
    [Range(0, 1)]
    public float rot_responsivity = 1f;
    
    public Vector2 camera_pos_offset = new Vector2(0, 0);
    public Vector2 camera_rot_offset = new Vector2(0, 0);

    private Rigidbody player_rigidbody;
    private Transform body;
    private Transform Camera;

    // Start is called before the first frame update
    void Start()
    {
        Camera = this.transform.Find("Camera");

        body = this.transform.Find("Body");
        player_rigidbody = body.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        angle += Input.GetAxis("Horizontal") * ang_speed;
        Quaternion player_rot = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * ang_speed, body.position);
        Quaternion orientation = Quaternion.FromToRotation(body.up, body.position);
        body.rotation = player_rot * orientation * body.rotation;
        Vector3 move_displacement = body.forward * Time.deltaTime * speed * Input.GetAxis("Vertical");
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

}
