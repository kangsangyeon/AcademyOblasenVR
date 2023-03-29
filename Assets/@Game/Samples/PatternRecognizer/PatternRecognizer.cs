using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatternRecognizer : MonoBehaviour
{
    [SerializeField] private NodeGrid m_NodeGrid;
    private Pattern m_Pattern;

    private List<Node> m_VisitedNodeList;

    public Pattern Pattern
    {
        get => m_Pattern;
        set => m_Pattern = value;
    }

    public List<Node> GetVisitNodeList() => m_VisitedNodeList;

    public List<NodePosition> GetVisitedNodePath() => m_VisitedNodeList.Select(c => c.Position).ToList();

    public Vector3 GetPositionInPathByDelta(List<NodePosition> _path, float _delta)
    {
        var _nodePath = m_NodeGrid.GetNodesInPath(_path).Select(n => n.transform.position).ToList();
        List<float> _distanceFromStart = new List<float>();

        _distanceFromStart.Add(0);
        for (int i = 1; i < _path.Count; i++)
        {
            Vector3 _prevNodePosition =  m_NodeGrid.GetNodeAt(_path[i - 1].y, _path[i - 1].x).transform.position;
            Vector3 _currentNodePosition = m_NodeGrid.GetNodeAt(_path[i].y, _path[i].x).transform.position;
            _distanceFromStart.Add(_distanceFromStart[i - 1] + Vector3.Distance(_prevNodePosition, _currentNodePosition));
        }

        float _distanceStartToEnd = _distanceFromStart.Last();

        var _normalizedDistanceFromStart = _distanceFromStart.Select(e => e / _distanceStartToEnd).ToList();

        Vector3 _result = FindByNormalized(_nodePath, _normalizedDistanceFromStart, _delta);

        return _result;
    }

    public List<Vector3> GetPositionListInPathByDelta(List<NodePosition> _path, float _delta)
    {
        var _nodePath = m_NodeGrid.GetNodesInPath(_path).Select(n => n.transform.position).ToList();

        List<float> _distanceFromStart = new List<float>();

        _distanceFromStart.Add(0);
        for (int i = 1; i < _path.Count; i++)
        {
            Vector3 _prevNodePosition =  m_NodeGrid.GetNodeAt(_path[i - 1].y, _path[i - 1].x).transform.position;
            Vector3 _currentNodePosition = m_NodeGrid.GetNodeAt(_path[i].y, _path[i].x).transform.position;
            _distanceFromStart.Add(_distanceFromStart[i - 1] + Vector3.Distance(_prevNodePosition, _currentNodePosition));
        }

        float _distanceStartToEnd = _distanceFromStart.Last();

        var _normalizedDistanceFromStart = _distanceFromStart.Select(e => e / _distanceStartToEnd).ToList();

        // 

        List<Vector3> _result = new List<Vector3> { _nodePath[0] };
            
        for (int i = 0; i < _normalizedDistanceFromStart.Count - 1; i++)
        {
            var t1 = _normalizedDistanceFromStart[i];
            var t2 = _normalizedDistanceFromStart[i + 1];
            if (t1 <= _delta && _delta <= t2)
            {
                float tt = Mathf.InverseLerp(t1, t2, _delta);
                
                var v1 = _nodePath[i];
                var v2 = _nodePath[i + 1];
                var lerped = Vector3.Lerp(v1, v2, tt);
                _result.Add(lerped);
                break;
            }
            
            _result.Add(_nodePath[i + 1]);
        }

        return _result;
    }

    private Vector3 FindByNormalized(List<Vector3> vs, List<float> ts, float t)
    {
        for (int i = 0; i < ts.Count - 1; i++)
        {
            var t1 = ts[i];
            var t2 = ts[i + 1];
            if (t1 <= t && t <= t2)
            {
                var v1 = vs[i];
                var v2 = vs[i + 1];
                float tt = Mathf.InverseLerp(t1, t2, t);
                return Vector3.Lerp(v1, v2, tt);
            }
        }

        return t > 0.5f ? vs[vs.Count - 1] : vs[0];
    }

    public void DisableAllNodes()
    {
        m_NodeGrid.GetAllNodes().ForEach(n => n.gameObject.SetActive(false));
    }

    public void EnableNodesInPath(List<NodePosition> _path) => m_NodeGrid.GetNodesInPath(_path).ForEach(n => n.gameObject.SetActive(true));

    public void AttemptVisit(Node node)
    {
        if (enabled == false)
            return;

        if (m_VisitedNodeList.Count > 0 && m_VisitedNodeList.Last() == node)
        {
            // 직전에 방문한 노드와 현재 방문한 노드가 같다면,
            // 이번 방문 요청을 무시합니다.
            // 이는 TriggerStay에 의해 의도하지 않게 계속 같은 노드를 방문하는 것을 막기 위함입니다.
            return;
        }

        m_VisitedNodeList.Add(node);
        node.SetActivate(true);
    }

    private void Start()
    {
        m_NodeGrid.GetAllNodes().ForEach(n => n.gameObject.SetActive(false));
        enabled = false;
    }

    private void OnEnable()
    {
        m_VisitedNodeList = new List<Node>();
    }

    private void OnDisable()
    {
        var _path = GetVisitedNodePath();

        string _message = $"count: {_path.Count} \n";
        _path.ForEach(p => _message += $"{p.ToString()}, ");
        // Debug.Log(_message);

        if (m_Pattern)
        {
            bool _bMatch = m_Pattern.Match(_path);
            if (_bMatch)
                Debug.Log($"match success! : {m_Pattern.name}");
        }

        m_VisitedNodeList.ForEach(c => c.SetActivate(false));
        m_VisitedNodeList.Clear();
    }
}