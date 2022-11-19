using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] m_musicClips;

	private AudioSource m_source;

	private void Awake()
	{
		m_source = GetComponent<AudioSource>(); 
	}

	private void Start()
	{
		if (m_musicClips.Length == 0)
			return; 

		if (GetComponent<RaceContestant>().IsPlayer)
			m_source.clip = m_musicClips[Random.Range(0, m_musicClips.Length)];

		m_source.Play(); 
	}
}
