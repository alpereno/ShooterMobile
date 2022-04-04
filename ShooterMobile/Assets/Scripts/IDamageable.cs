using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    //damage and hit variable (used to hitPoint and hitDirection instead of hit variable cause the hit variable is very inclusive)
    void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection);

    //just damage
    void takeDamage(float damage);
}