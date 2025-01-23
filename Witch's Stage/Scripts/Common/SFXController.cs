using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{    public static SFXController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip sfx = Resources.Load<AudioClip>("Sounds/SFX/" + sfxName);
        AudioSource.PlayClipAtPoint(sfx, Camera.main.transform.position);
    }
}
