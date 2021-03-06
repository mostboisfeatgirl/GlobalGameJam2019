﻿using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Weapon setup")]
    [SerializeField] private Collider2D[] KillColliders;
    [SerializeField] private Collider2D[] BlockColliders;
    [SerializeField] private Collider2D[] PhyiscalColliders;

    [Header("Weapon properties")]
    public float knockbackForce = 10.0f;
    public bool isHeld = false;
    public Player carrier;
    public bool isFlying = false;

    [Header("Effects")]
    [SerializeField] private GameObject collisonSparkEffect;

    public enum WeaponSFXType { COLLISION1, COLLISION2, THROW, FINAL };

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] WeaponSFXClips = new AudioClip[(int)WeaponSFXType.FINAL];

    private void Start()
    {
        SetCombatCollidersActive(false);
        SetPhysicalCollidersActive(true);

        audioSource = GetComponent<AudioSource>();
        
    }

    private void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

    public void PlayThrowSound()
    {
        PlaySound(WeaponSFXClips[(int)WeaponSFXType.THROW]);
    }

    public void SetCombatCollidersActive(bool newState)
    {
        for (int i = 0; i < KillColliders.Length; i++)
        {
            KillColliders[i].enabled = newState;
        }

        for (int i = 0; i < KillColliders.Length; i++)
        {
            BlockColliders[i].enabled = newState;
        }
    }

    public void SetPhysicalCollidersActive(bool newState)
    {
        for (int i = 0; i < PhyiscalColliders.Length; i++)
        {
            PhyiscalColliders[i].enabled = newState;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFlying)
        {
             if (other.transform.tag == "Player") 
             {
                 other.GetComponentInParent<Player>().KillPlayer();
                PlaySound(WeaponSFXClips[Random.Range((int)WeaponSFXType.COLLISION1, (int)WeaponSFXType.COLLISION2)]);
            }
             isFlying = false;
             SetCombatCollidersActive(false);
             SetPhysicalCollidersActive(true);
        }

        if (other.transform.tag == "Weapon")
        {
            var weaponComponent = other.GetComponent<WeaponBase>();
            if(isHeld && weaponComponent.isHeld)
            {
                carrier.KnockBackPlayer(knockbackForce);
                weaponComponent.carrier.KnockBackPlayer(knockbackForce);
                Vector3 halfDistance = (other.gameObject.transform.position - transform.position) * 0.5f;

                var newParticleSystem = GameObject.Instantiate(collisonSparkEffect, transform.position + halfDistance, Quaternion.identity);
                Destroy(newParticleSystem, 5.0f);
                return;
            }
        }

        if (carrier != null)
        {
            if (other.transform.tag == "Player" && other.gameObject != carrier.gameObject)
            {
                if (!other.GetComponentInParent<Player>().isDead)
                {
                    PlaySound(WeaponSFXClips[Random.Range((int)WeaponSFXType.COLLISION1,(int)WeaponSFXType.COLLISION2)]);
                    other.GetComponentInParent<Player>().KillPlayer();
                }
            }
        }
    }
}