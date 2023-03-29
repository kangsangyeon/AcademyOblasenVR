using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestPatternRecognitionPhase : MonoBehaviour
{
    [SerializeField] private Pattern m_Asset_Pattern;
    [SerializeField] private PatternRecognitionPhase m_Phase;
    [SerializeField] private Transform m_HandPoint;
    [SerializeField] private ActionBasedController m_Controller;
    [SerializeField] private TextMeshProUGUI m_Text_Progress;

    private void Start()
    {
        m_Phase.Controller = m_Controller;
        m_Phase.HandPoint = m_HandPoint;

        m_Phase.OnUserProgressChanged.AddListener((_node, _p) => m_Text_Progress.text = _p.ToString());
        m_Phase.OnUserSuccess.AddListener(() => m_Text_Progress.text = "success!");
        m_Phase.OnUserFault.AddListener((_node, _controller) => m_Text_Progress.text = "fault! retry");
        m_Phase.OnUserFailed.AddListener(() => m_Text_Progress.text = "failed...");

        if (m_Asset_Pattern)
            StartCoroutine(m_Phase.StartPhase(m_Asset_Pattern));
    }


    [ContextMenu("Replay")]
    public void Replay()
    {
        if (m_Asset_Pattern)
            StartCoroutine(m_Phase.StartPhase(m_Asset_Pattern));
    }
}