using UnityEngine;
using UnityEngine.VFX;

public class LightningSpell : SpellBehavior
{
    public override void CastSpell(GameObject target)
    {
        Debug.Log("CastSpell called on: " + target.name);
        if (target.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (data.spellEffectPrefab)
            {
                GameObject fx = Instantiate(
                    data.spellEffectPrefab,
                    target.transform.position + Vector3.up * 1f,
                    Quaternion.identity
                );
                ParticleSystem ps = fx.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
                else
                    Destroy(fx, 3f);
            }
        }

        Destroy(target);
    }
}