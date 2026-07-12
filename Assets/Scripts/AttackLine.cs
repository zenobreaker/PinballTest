using UnityEngine;

public class AttackLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Enemy>(out var enemy))
            enemy.BeginAttack();
    }
}
