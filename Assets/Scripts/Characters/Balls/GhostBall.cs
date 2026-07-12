using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostBall : Ball
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.TryGetComponent(out Enemy enemy) )
        {
            Vector2 hitpoint = collision.GetContact(0).point;

            // 데미지 처리 (기본 Ball 클래스의 함수 활용)
            DealDamage(collision.gameObject, hitpoint, collision);

            rb.linearVelocity = lastVelocity.normalized * currentSpeed;

            // 관통하면서 특수효과 발동이 필요하다면 여기서 호출
            ApplySpecialEffect(collision.gameObject);

            Physics2D.IgnoreCollision(myCollider, collision.collider, true);

            return;
        }
        else
            Bounce(collision);
    }

    protected override void ApplySpecialEffect(GameObject target)
    {

    }
}