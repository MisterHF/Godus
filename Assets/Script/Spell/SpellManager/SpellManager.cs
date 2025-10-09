using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;

    private SpellData activeSpell;
    private SpellBehavior spellBehavior;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ActivateSpell(SpellData newSpell)
    {
        if (activeSpell != null && activeSpell.spellID == newSpell.spellID)
        {
            CancelSpell();
            return;
        }

        activeSpell = newSpell;
        Cursor.SetCursor(activeSpell.cursorTexture, Vector2.zero, CursorMode.Auto);

        switch (activeSpell.type)
        {
            case SpellType.Lightning:
                spellBehavior = gameObject.AddComponent<LightningSpell>();
                break;
        }

        spellBehavior.data = activeSpell;
    }

    public void CancelSpell()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        activeSpell = null;
        if (spellBehavior) Destroy(spellBehavior);
    }

    void Update()
    {
        if (activeSpell == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                spellBehavior.CastSpell(hit.collider.gameObject);
                CancelSpell();
            }
        }
    }
}