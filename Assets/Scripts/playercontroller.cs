using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IsometricPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody rb;
    private Vector3 userInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Prevents the character from falling over due to physics
        rb.freezeRotation = true; 
    }

    private void Update()
    {
        GatherInput();
        RotateCharacter();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void GatherInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Normalize to prevent faster diagonal movement
        userInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void MoveCharacter()
    {
        if (userInput == Vector3.zero) return;

        Vector3 isometricDirection = ConvertToIsometric(userInput);
        Vector3 newPosition = rb.position + isometricDirection * moveSpeed * Time.fixedDeltaTime;
        
        rb.MovePosition(newPosition);
    }

    private void RotateCharacter()
    {
        if (userInput == Vector3.zero) return;

        Vector3 isometricDirection = ConvertToIsometric(userInput);
        Quaternion targetRotation = Quaternion.LookRotation(isometricDirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Rotates the input vector by 45 degrees on the Y-axis
    private Vector3 ConvertToIsometric(Vector3 input)
    {
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));
        return isoMatrix.MultiplyPoint3x4(input);
    }
}