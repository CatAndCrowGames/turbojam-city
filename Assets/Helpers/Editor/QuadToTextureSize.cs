using UnityEngine;

[ExecuteAlways]
public class QuadToTextureSize : MonoBehaviour
{
    [SerializeField] bool scaleNow;
    void Update()
    {
        if (scaleNow)
            Scale();
    }

    void Scale()
    {
        scaleNow = false;

        Material material = GetComponent<Renderer>().sharedMaterial;
        if (material == null)
        {
            Debug.Log("No material found on " + name);
            return;
        }
        Texture texture = material.mainTexture;
        if (texture == null)
        {
            Debug.Log("No main texture in material:" + material.name);
            return;
        }

        float xSize = texture.width / (float)texture.height;
        Vector3 scaleVector = new Vector3(xSize, 1, 1);
        transform.localScale = scaleVector;
    }
}
