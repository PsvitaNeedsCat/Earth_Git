using UnityEngine;
using System.Collections;

public class GreyscaleCameraScript : MonoBehaviour
{
    public Material mat;
    [SerializeField] private bool turnGrey = false;

    void Start()
    {
        mat.SetFloat("_Power", 0.0f);
    }

    private void Update()
    {
        return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!turnGrey)
            {
                mat.SetFloat("_Power", 1.0f);
                turnGrey = true;
            }
            else
            {
                mat.SetFloat("_Power", 0.0f);
                turnGrey = false;
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}