using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterSpritesSheet", menuName = "Character")]
public class CharacterSpriteSheet : ScriptableObject
{
    public string characterId;
    public Sprite idleSprite;
    public Sprite flySprite;
}

public enum CharacterSpriteStatus
{
    Idle,
    Fly
}