using UnityEngine;

public class EntityWeak : MonoBehaviour
{
    [SerializeField] Entity entity;

    internal void GetDamage(int dmg)
    {
        entity.GetDamage(dmg);
    }
}
