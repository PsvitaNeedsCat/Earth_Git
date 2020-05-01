using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestSync : MonoBehaviour
{
    [SerializeField] string m_levelName;

    private void OnCollisionEnter(Collision collision)
    {
        SceneDatabase.Instance.LoadScene(m_levelName);
    }
}
