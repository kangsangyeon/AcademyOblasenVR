using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PatternRecognitionPhase : MonoBehaviour
{
    [SerializeField] private float m_PreviewDuration;
    [SerializeField] private float m_TryDrawDuration;
    [SerializeField] private float m_StepInterval = 1.0f;
    [SerializeField] private NodeGrid m_NodeGrid;
    [SerializeField] private PatternRecognizer m_Recognizer;
    [SerializeField] private Transform m_PreviewHand;
    [SerializeField] private AnimationCurve m_PreviewCurve;
    [SerializeField] private LineRenderer m_LineRenderer;

    public UnityEvent OnStartPhase;
    public UnityEvent OnEndPhase;
    public UnityEvent OnStartShowNodesInPathStep;
    public UnityEvent OnStartShowPathPreviewStep;
    public UnityEvent OnStartTryDrawStep;
    public UnityEvent OnEndShowNodesInPathStep;
    public UnityEvent OnEndShowPathPreviewStep;
    public UnityEvent OnEndTryDrawStep;
    public UnityEvent<Node> OnPopupNode;
    public UnityEvent OnStartDrawing;
    public UnityEvent OnEndDrawing;
    public UnityEvent<Node, float> OnUserProgressChanged;
    public UnityEvent<Node, ActionBasedController> OnUserFault;
    public UnityEvent OnUserFailed;
    public UnityEvent OnUserSuccess;

    private float m_MaxProgress;
    private float m_DamageMultiplier;
    private float m_PhaseLeftoverTime;
    private Pattern m_Pattern;

    public float GetTryDrawDuration() => m_TryDrawDuration;
    public float GetMaxProgress() => m_MaxProgress;
    public float GetDamageMultiplier() => m_DamageMultiplier;
    public float GetPhaseLeftoverTime() => m_PhaseLeftoverTime;

    private Transform m_HandPoint;
    private ActionBasedController m_Controller;

    public Transform HandPoint
    {
        set => m_HandPoint = value;
    }

    public ActionBasedController Controller
    {
        set => m_Controller = value;
    }

    public void SetPattern(Pattern _pattern)
    {
        m_Pattern = _pattern;
        m_Recognizer.Pattern = _pattern;

        if (_pattern == null)
            return;
        
        m_NodeGrid.GetNodesInPath(_pattern.path).ForEach(n => n.enabled = false);

        m_Recognizer.DisableAllNodes();

        m_NodeGrid.GetAllNodes().ForEach(n => n.SetPatternRecognizer(m_Recognizer));
        m_NodeGrid.gameObject.SetActive(true);

        m_MaxProgress = 0.0f;
        m_DamageMultiplier = 0.0f;

        OnStartPhase.Invoke();
    }

    private void Start()
    {
        HideNodeGrid();
        OnStartPhase.AddListener(ShowNodeGrid);
        OnEndPhase.AddListener(HideNodeGrid);
    }

    private void OnDestroy()
    {
        HideNodeGrid();
        OnStartPhase.RemoveListener(ShowNodeGrid);
        OnEndPhase.RemoveListener(HideNodeGrid);
    }

    public IEnumerator StartPhase(Pattern _pattern)
    {
        SetPattern(_pattern);
        
        //m_Recognizer.DisableAllNodes();

        //m_NodeGrid.GetAllNodes().ForEach(n => n.SetPatternRecognizer(m_Recognizer));
        //m_NodeGrid.gameObject.SetActive(true);

        //m_MaxProgress = 0.0f;
        //m_DamageMultiplier = 0.0f;

        //OnStartPhase.Invoke();

        m_PhaseLeftoverTime = m_TryDrawDuration;

        // 노드들이 순차적으로 표시된다.

        yield return StartCoroutine(ShowNodesInPathStep());

        // 과정을 그린다.

        yield return StartCoroutine(ShowPathPreviewStep());
        yield return new WaitForSeconds(1);

        // 사용자가 그리기를 시도한다.

        yield return StartCoroutine(TryDrawPathStep());

        OnEndPhase.Invoke();
    }

    public IEnumerator ShowPathPreviewStep()
    {
        OnStartShowPathPreviewStep.Invoke();

        float _startTime = Time.time;

        while (true)
        {
            float _elapsedTime = Time.time - _startTime;

            if (_elapsedTime > m_PreviewDuration)
                break;

            float _curvedDelta = m_PreviewCurve.Evaluate(_elapsedTime / m_PreviewDuration);

            // 프리뷰 손의 위치를 갱신합니다.

            m_PreviewHand.position = m_Recognizer.GetPositionInPathByDelta(m_Recognizer.Pattern.path, _curvedDelta);

            // 프리뷰 선을 그립니다.

            var positionListInPathByDelta = m_Recognizer.GetPositionListInPathByDelta(m_Recognizer.Pattern.path, _curvedDelta);

            m_LineRenderer.positionCount = positionListInPathByDelta.Count + 1;
            for (int i = 0; i < positionListInPathByDelta.Count; ++i)
                m_LineRenderer.SetPosition(i, positionListInPathByDelta[i]);

            m_LineRenderer.SetPosition(positionListInPathByDelta.Count, m_PreviewHand.position);

            yield return null;
        }

        OnEndShowPathPreviewStep.Invoke();

        yield return new WaitForSeconds(m_StepInterval);
    }
    
    public IEnumerator ShowNodesInPathStep()
    {
        OnStartShowNodesInPathStep.Invoke();

        var _nodesInPath = m_NodeGrid.GetNodesInPath(m_Recognizer.Pattern.path);

        for (int i = 0; i < _nodesInPath.Count; ++i)
        {
            _nodesInPath[i].gameObject.SetActive(true);
            OnPopupNode.Invoke(_nodesInPath[i]);
            yield return new WaitForSeconds(.2f);
        }

        OnEndShowNodesInPathStep.Invoke();

        yield return new WaitForSeconds(m_StepInterval);
    }

    public IEnumerator TryDrawPathStep()
    {
        OnStartTryDrawStep.Invoke();

        m_NodeGrid.GetNodesInPath(m_Recognizer.Pattern.path).ForEach(n => n.enabled = true);

        float _startTime = Time.time;

        List<NodePosition> visitedNodesPositionList = new List<NodePosition>();
        List<Node> visitedNodesList = new List<Node>();

        bool _bProgressLastFrame = true;
        float _progressLastFrame = 0.0f;
        bool _bRecognizerEnabledLastFrame = false;
        bool _success = false;

        while (true)
        {
            float _elapsedTime = Time.time - _startTime;
            m_PhaseLeftoverTime = m_TryDrawDuration - _elapsedTime;

            if (_elapsedTime > m_TryDrawDuration)
                break;

            if (m_Recognizer.enabled)
            {
                visitedNodesPositionList = m_Recognizer.GetVisitedNodePath();
                visitedNodesList = m_Recognizer.GetVisitNodeList();
            }

            // 패턴 진행도를 얻습니다.

            float _progress;
            bool _bProgress = m_Recognizer.Pattern.MatchProgress(visitedNodesPositionList, out _progress);

            // 진행도에 따른 이벤트를 발생시킵니다.

            if (_bProgress == true && visitedNodesPositionList.Count > 0)
            {
                m_MaxProgress = Mathf.Max(m_MaxProgress, _progress);

                if (_progress >= 1.0f)
                {
                    // 유저가 패턴을 성공시켰을 때 호출됩니다.
                    OnUserSuccess.Invoke();
                    _success = true;
                    break;
                }
                else if (_progress > _progressLastFrame)
                {
                    // 유저가 패턴 진행도를 높였을 때 호출됩니다.
                    OnUserProgressChanged.Invoke(visitedNodesList.Last(), _progress);
                }
            }
            else if (_bProgress == false && _bProgressLastFrame == true)
            {
                // 유저가 실수했을 때 호출됩니다.
                // (아직 step이 끝나지는 않았습니다. 유저는 다시 시도할 수 있습니다.)
                OnUserFault.Invoke(visitedNodesList.Last(), m_Controller);

                // 경로를 그리는 데 실수했다면,
                // recognizer를 강제로 비활성화시키고 다시 컨트롤러를 조작해 recognizer를 활성화하여
                // 인식할 수 있도록 강제합니다.
                m_Recognizer.enabled = false;

                visitedNodesPositionList = new List<NodePosition>();
                visitedNodesList = new List<Node>();
            }

            if (_bRecognizerEnabledLastFrame == false && m_Recognizer.enabled == true)
            {
                // 그리기를 시작했을 때 호출됩니다.
                OnStartDrawing.Invoke();
            }
            else if (_bRecognizerEnabledLastFrame == true && m_Recognizer.enabled == false)
            {
                // 그리기를 중단했을 때와 강제 중단되었을 때 호출됩니다.
                OnEndDrawing.Invoke();
            }

            _bProgressLastFrame = _bProgress;
            _progressLastFrame = _progress;
            _bRecognizerEnabledLastFrame = m_Recognizer.enabled;

            // 선을 그립니다.

            if (visitedNodesList.Count > 0 && m_Recognizer.enabled == true)
            {
                if (_bProgress == true)
                {
                    // 경로대로 잘 잇고 있으며 아직 마우스를 떼지 않았을 때,
                    // 지금까지 이어온 경로에 지금 마우스 위치를 포함하여 선을 그립니다.

                    m_LineRenderer.positionCount = visitedNodesList.Count + 1;
                    for (int i = 0; i < visitedNodesList.Count; ++i)
                        m_LineRenderer.SetPosition(i, visitedNodesList[i].transform.position);

                    m_LineRenderer.SetPosition(visitedNodesList.Count, m_HandPoint.position);
                }
                else
                {
                    m_LineRenderer.positionCount = visitedNodesList.Count;
                    for (int i = 0; i < visitedNodesList.Count; ++i)
                        m_LineRenderer.SetPosition(i, visitedNodesList[i].transform.position);
                }
            }
            else
            {
                m_LineRenderer.positionCount = 0;
            }

            yield return null;
        }

        m_PhaseLeftoverTime = 0.0f;
        
        m_LineRenderer.positionCount = 0;
        m_NodeGrid.GetNodesInPath(m_Recognizer.Pattern.path).ForEach(n => n.enabled = false);

        // 데미지 배율을 구합니다.
        m_DamageMultiplier =
            m_MaxProgress >= 0.81f ? 1f
            : m_MaxProgress >= 0.61f ? 0.8f
            : m_MaxProgress >= 0.41f ? 0.6f
            : m_MaxProgress >= 0.21f ? 0.4f
            : 0.2f;

        // 성공 여부에 따라 적절한 이벤트를 발생시킵니다.

        if (_success)
            OnUserSuccess.Invoke();
        else
            OnUserFailed.Invoke();

        OnEndTryDrawStep.Invoke();

        yield return new WaitForSeconds(m_StepInterval);
    }

    private void ShowNodeGrid()
    {
        m_NodeGrid.gameObject.SetActive(true);
    }

    private void HideNodeGrid()
    {
        m_NodeGrid.gameObject.SetActive(false);
    }
}