using UnityEngine;
using System.Collections;

public class BasicEnemy : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip squashSound;

    private Transform player;
    private bool isChasing = false;
    private bool isDead = false;

    void Update()
    {
        if (isDead) return;

        if (isChasing && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * 5f);
            transform.position += direction * moveSpeed * Time.deltaTime;

            if(animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            if(animator != null) animator.SetBool("isWalking", false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            player = null;
        }
    }
    
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null && squashSound != null)
            audioSource.PlayOneShot(squashSound);

        StartCoroutine(SquashAnimation());
    }

    IEnumerator SquashAnimation()
    {
        Vector3 targetScale = new Vector3(transform.localScale.x * 1.5f, transform.localScale.y * 0.1f, transform.localScale.z * 1.5f);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
            yield return null;
        }
        Destroy(gameObject, 0.2f);
    }
}