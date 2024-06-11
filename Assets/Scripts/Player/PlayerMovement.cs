using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed;
    private Vector2 moveInput;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ParticleSystem particleSystem;

    private ParticleSystem.MainModule mainModule;

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        
        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move, Vector3.up);
            
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        }
        
        
        playerAnimator.SetFloat("moveX", moveInput.x);
        playerAnimator.SetFloat("moveZ", moveInput.y);
        
        AdjustParticleSize(moveInput);
    }
    
    private void AdjustParticleSize(Vector2 moveInput)
    {
        float speed = Mathf.Sqrt(moveInput.x * moveInput.x + moveInput.y * moveInput.y);
        
        float maxParticleSize = 1.0f;
        float minParticleSize = 0.5f;
        
        float normalizedSpeed = speed / 0.1f;
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        
        float particleSize = Mathf.Lerp(minParticleSize, maxParticleSize, normalizedSpeed);
        
        mainModule.startSize = speed == 0 ? 0 : particleSize;
    }
    
}
