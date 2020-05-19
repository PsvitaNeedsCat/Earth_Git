using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeMovement : MonoBehaviour
{
    public List<CentipedeSegmentMover> m_segments = new List<CentipedeSegmentMover>();

    private float t = 0.0f;
    private Quaternion headTargetRotation;

    private void Awake()
    {
        for (int i = 0; i < m_segments.Count - 1; i++)
        {
            m_segments[i].m_segmentBehind = m_segments[i + 1];
        }

        headTargetRotation = m_segments[0].transform.rotation;
        m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, m_segments[0].transform.rotation);
    }

    private void Update()
    {
        TestInput();
    }

    private void TestInput()
    {
        int horInput = 0;
        if (Input.GetKeyDown(KeyCode.F)) horInput -= 1;
        if (Input.GetKeyDown(KeyCode.H)) horInput += 1;

        // horInput = Random.Range(0, 3) - 1;

        float forwardInput = (Input.GetKey(KeyCode.T)) ? 1.0f : 0.0f;
        Forward(forwardInput);
        Turn(horInput);
    }

    private void Forward(float _input)
    {
        t += Time.smoothDeltaTime * 2.0f * _input * CentipedeBoss.GetCurrentStateInfo().m_moveSpeed;

        if (t >= 1.0f)
        {
            m_segments[0].NextPos(m_segments[0].transform.position + m_segments[0].transform.forward, headTargetRotation);
            t = 0.0f;
        }

        m_segments[0].Move(t);
    }

    private void Turn(float _horInput)
    {
        // m_segments[0].gameObject.transform.Rotate(Vector3.up * _horInput * 90.0f);

        headTargetRotation = Quaternion.Euler(Vector3.up * 90.0f * _horInput) * headTargetRotation;

        //if (Random.Range(0, 200) == 0)
        //{
        //    int flip = Random.Range(0, 2);
        //    int turn = (flip == 0) ? -1 : 1;
        //    headTargetRotation = Quaternion.Euler(Vector3.up * 90.0f * turn) * headTargetRotation;
        //}
    }
}
