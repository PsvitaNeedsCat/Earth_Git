using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraSwapper : MonoBehaviour
{
    [SerializeField] private float m_transitionTime = 4.0f;
    [SerializeField] private CinemachineVirtualCamera[] m_cameras = new CinemachineVirtualCamera[] { };

    private CinemachineBrain m_cinemachineBrain = null;
    private CinemachineVirtualCamera m_currentCamera = null;

    private void Awake()
    {
        DeactivateCameras();

        m_cinemachineBrain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
    }

    public void BeginTransition(int _camNumber)
    {
        if (_camNumber < 0 || _camNumber >= m_cameras.Length)
        {
            return;
        }

        StartCoroutine(Transition(_camNumber));
    }

    // Transitions to the new camera after 1 frame
    private IEnumerator Transition(int _camNumber)
    {
        yield return null;

        // Deactivate current camera
        m_currentCamera = m_cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        m_currentCamera.gameObject.SetActive(false);

        // Activate correct camera
        m_cameras[_camNumber].gameObject.SetActive(true);

        StartCoroutine(TransitionBack());
    }

    // Transitions back to the original camera
    private IEnumerator TransitionBack()
    {
        yield return new WaitForSeconds(m_transitionTime);

        DeactivateCameras();

        if (m_currentCamera)
        {
            m_currentCamera.gameObject.SetActive(true);
        }
    }

    // Sets all the cameras to inactive
    private void DeactivateCameras()
    {
        for (int i = 0; i < m_cameras.Length; i++)
        {
            m_cameras[i].gameObject.SetActive(false);
        }
    }
}
