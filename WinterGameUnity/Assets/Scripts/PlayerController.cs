using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveChange;
    [SerializeField] private int moveSpeed;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        moveChange = Vector3.zero;
        moveChange.x = moveInput.x * moveSpeed * Time.deltaTime;
        moveChange.y = moveInput.y * moveSpeed * Time.deltaTime;
        transform.position += moveChange;
    }
}
