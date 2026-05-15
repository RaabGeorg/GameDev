using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out var character)) {
            character.InflictDamage(9999f); //KILLEM
        }
    }
}
