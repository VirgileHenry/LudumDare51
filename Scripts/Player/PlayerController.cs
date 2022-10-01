using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    public float speed = 1f;
    public float gravity_intensity = 1f;
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
        // Update Player transform (gravity + mvt)
        Vector3 position = Vector3.Normalize(body.position);

        Vector3 moveForce = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Quaternion rotate_quat = Quaternion.FromToRotation(Vector3.forward, position);
        moveForce = rotate_quat * moveForce;

        player_rigidbody.AddForce(speed * moveForce - gravity_intensity * position, ForceMode.Force);

        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, position);
        body.transform.rotation = orientation;

        // Update Camera transform
        Quaternion toward_body_quat = body.rotation;
        Quaternion offset_quat = Quaternion.AngleAxis(camera_rot_offset.x, body.right);
        Quaternion anti_offset_quat = Quaternion.AngleAxis(-body.rotation.y, body.up);

        Camera.rotation = Quaternion.AngleAxis(180f, body.up) * offset_quat * anti_offset_quat * toward_body_quat;

        Camera.position = position + camera_pos_offset.x * Camera.forward + camera_pos_offset.y * Camera.up;

    }
}
