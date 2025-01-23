using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadController : MonoBehaviour
{
    public float fadeSpeed;
    private float fadeTime;
    [SerializeField] private Image backgroundImage;
    private Material material;

    void Start()
    {
        backgroundImage.material = Instantiate(backgroundImage.materialForRendering);
        material = backgroundImage.material;
    }

    public IEnumerator Open()
    {
        fadeTime = 0f;
        while (true)
        {
            fadeTime += Time.deltaTime * fadeSpeed;
            material.SetFloat("_SplitValue", Mathf.Cos(fadeTime));
            if (Mathf.Cos(fadeTime) <= 0.1f)
            {
                material.SetFloat("_SplitValue", 0);
                break;
            }

            yield return null;
        }
        yield break;
    }

    public IEnumerator Close()
    {
        fadeTime = 0f;
        while (true)
        {
            fadeTime += Time.deltaTime * fadeSpeed;
            material.SetFloat("_SplitValue", Mathf.Sin(fadeTime));
            if (Mathf.Sin(fadeTime) >= 0.99f)
            {
                material.SetFloat("_SplitValue", 1);
                break;
            }
            yield return null;
        }
        yield break;
    }
}
