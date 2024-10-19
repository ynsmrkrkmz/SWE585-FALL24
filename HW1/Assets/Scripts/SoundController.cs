using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_AudioSource;

    private bool m_IsPlaying = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(m_IsPlaying)
            {
                m_AudioSource.Pause();
            }
            else
                m_AudioSource.Play();

            m_IsPlaying = !m_IsPlaying;
        }
    }
}
