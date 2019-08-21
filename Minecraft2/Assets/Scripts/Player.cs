using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;

    private Transform cam;
    private World world;

    public float walkSpeed=3f;
    public float sprintSpeed=6f;
    public float jumpForce = 5f;
    public float gravity =-9.8f;

    public float playerWidth=0.3f;


    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update()
    {
        GetPlayerInputs();

        velocity = ((transform.forward * vertical) + (transform.right*horizontal))*Time.deltaTime*walkSpeed;
        velocity += Vector3.up * gravity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * - mouseVertical);

        transform.Translate(velocity,Space.World);
    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
    }
}
