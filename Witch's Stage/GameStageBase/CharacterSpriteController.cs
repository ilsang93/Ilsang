using System;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    public CharacterSpriteSheet characterSpriteSheet;
    public Action onCharacterStatusChanaged;
    private CharacterSpriteStatus characterSpriteStatus;

    void Start()
    {
        onCharacterStatusChanaged += () =>
        {
            switch (GetCharacterStatus())
            {
                case CharacterSpriteStatus.Idle:
                    SetSprite(characterSpriteSheet.idleSprite);
                    break;
                case CharacterSpriteStatus.Fly:
                    SetSprite(characterSpriteSheet.flySprite);
                    break;
            }
        };
    }
    
    public void SetCharacterStatus(CharacterSpriteStatus status)
    {
        characterSpriteStatus = status;
        onCharacterStatusChanaged?.Invoke();
    }

    private CharacterSpriteStatus GetCharacterStatus()
    {
        return characterSpriteStatus;
    }

    private void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
