using UnityEngine;
using UnityEngine.Events;

public class MicInputUser : MonoBehaviour
{
    [SerializeField] private MicInput m_MicInput;
    
    private float m_HighestLevel;
    private float m_CurrentLevel;
    
    public UnityEvent<float> OnEndAction;

    public float GetHighestLevel() => m_HighestLevel;
    public float GetCurrentLevel() => m_CurrentLevel;

    private void OnEnable()
    {
        m_HighestLevel = float.NegativeInfinity;
        // m_MicInput.enabled = true;
    }

    private void OnDisable()
    {
        OnEndAction.Invoke(m_HighestLevel);
        // m_MicInput.enabled = false;
    }

    private void Update()
    {
        m_CurrentLevel = m_MicInput.GetHighestLevel();
        m_HighestLevel = Mathf.Max(m_HighestLevel, m_CurrentLevel);
    }
}
