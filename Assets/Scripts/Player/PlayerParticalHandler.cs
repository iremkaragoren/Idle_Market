using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticalHandler : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Animator animator;
    public string speedParameter = "Speed";
    
    private ParticleSystem.MainModule mainModule;

    void Start()
    {
        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
        }
    }

    void Update()
    {
        if (animator != null && particleSystem != null)
        {
            // Animatordan hız değerini al
            float speed = animator.GetFloat(speedParameter);

            // Hız değerine göre partikül boyutunu ayarla
            mainModule.startSize = Mathf.Lerp(0.1f, 1f, speed); // Örneğin, 0.1 ile 1 arasında boyut ayarlama
        }
    }
}
