using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float sin45 = Mathf.Sin(Mathf.PI / 4);
    private float movementX;
    private float movementZ;
    private float movementY;
    public float speed = 0;
    public float jumpStrength = 0;
    public float maxSpeed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        Vector3 movement = new Vector3(movementVector.x, 0.0f, movementVector.y);

        movementX = movementVector.x;
        movementZ = movementVector.y;
    }

    void OnJump() 
    {
        RaycastHit hitData;

        if(Physics.Raycast(rb.position, Vector3.down, out hitData, 1))
        {
            if(hitData.transform.CompareTag("Ground") && Mathf.Abs(Vector3.Dot(hitData.normal, Vector3.down)) > sin45)
            {
                movementY = 1;
                rb.AddForce(Vector3.up * jumpStrength);
            }
        }
    }

    void OnRespawn()
    {
        rb.position = Vector3.zero;
        rb.velocity = Vector3.zero;

        movementX = 0;
        movementY = 0;
        movementZ = 0;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 16)
        {
            winTextObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, movementY, movementZ);

        rb.AddForce(movement * speed * Time.deltaTime);

        Vector3 velocity = rb.velocity;

        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector2 clampedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(clampedVelocity.x, velocity.y, clampedVelocity.y);
        }

        if (rb.position.y < -2)
        {
            OnRespawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    public Vector3 GetPosition()
    {
        return rb.position;
    }

    public Vector3 GetDirection()
    {
        return transform.forward;
    }

    public void OnFire()
    {
        Instantiate(bullet, rb.position + transform.forward, transform.rotation);
    }
}
