using UnityEngine;

public enum SpellType
{
    Lightning
}

[CreateAssetMenu(fileName = "SpellData", menuName = "Scriptable Objects/SpellData")]
public class SpellData : ScriptableObject
{
    
    public string spellID;
    public string spellName;
    public Sprite icon;
    public SpellType type;
    public GameObject spellEffectPrefab;
    public Texture2D cursorTexture;
}
