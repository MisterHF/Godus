using UnityEngine;

public abstract class SpellBehavior : MonoBehaviour
{
    public SpellData data;
    public abstract void CastSpell(GameObject target);
}

