﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Collider playerCollider;
    public Camera cam;

    public float speed = 4f;
    public float sprintSpeed = 6.5f;
    public float crouchSpeed = 2.5f;
    public float proneSpeed = 1.5f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public Texture crosshairTexture;

    Vector3 velocity;
    bool isGrounded;
    SmoothCrouching smoothCrouching;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        smoothCrouching = new SmoothCrouching(controller, playerCollider);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Player.Instance.changeInventoryVisible();
            if (canMove) canMove = false;
            else canMove = true;
        }

        if (!canMove) return;

        //verify if is on ground------
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //-----------------------------


        //player movement in x and z axis----
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;


        
        if (Input.GetButton("Sprint"))
        {
            controller.Move(move * sprintSpeed * Time.deltaTime);
        }
        else if (Input.GetButton("Crouch"))
        {
            controller.Move(move * crouchSpeed * Time.deltaTime);
        }
        else if (Input.GetButton("Prone"))
        {
            controller.Move(move * proneSpeed * Time.deltaTime);
        }
        else 
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        //-----------------------------

        //player jump------------------
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        //-----------------------------


        //gravity stuff-------
        velocity.y += gravity * Time.deltaTime;

        //fall velocity = g * t^2
        controller.Move(velocity * Time.deltaTime);
        //--------------------

        //crouch mechanics
        if (Input.GetButtonDown("Crouch"))
        {
            smoothCrouching.setCrouching(true);
        }else if(Input.GetButtonUp("Crouch"))
        {
            smoothCrouching.setCrouching(false);
        }
        smoothCrouching.Update();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable.canInteract(hit.distance))
            {
                interactable.look();
            }
        }

        if (Input.GetButtonDown("Interact"))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("I'm looking at " + hit.transform.name);
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null && interactable.canInteract(hit.distance))
                {
                    interactable.interact();
                }
            }
        }
    }
}
