using UnityEngine;

public class Jewel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.TriggerVictory();
            Destroy(this.gameObject);
        }
    }
}
