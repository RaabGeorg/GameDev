using System;
using UnityEngine;

public class DamageLogic : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20;
    [SerializeField] private float damageInterval = 5f;

    private BasicEnemy parentEnemy;
    private float nextDamageTime = 0f;


    void Awake()
    {
        parentEnemy = GetComponentInParent<BasicEnemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (parentEnemy == null || parentEnemy.IsEnemyDead()) return;

        if (Time.time >= nextDamageTime && other.CompareTag("Player"))
        {
            if (other.TryGetComponent<Character>(out var player))
            {
                player.InflictDamage(damageAmount);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && parentEnemy != null)
        {
            parentEnemy.SetMovementStopped(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && parentEnemy != null)
        {
            parentEnemy.SetMovementStopped(false);
        }
    }
}
