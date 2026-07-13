using UnityEngine;

public interface IDamagable 
{
    public void OnDamage(GameObject attacker, Ball causer, Vector3 hitPoint,
        DamageEvent damageEvent);
}
