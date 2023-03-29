using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    private readonly int ROW_COLUMN_COUNT = 5;

    [SerializeField] private GameObject m_Asset_NodePrefab;
    [SerializeField] private Transform m_GridHolder;
    [SerializeField] private float m_NodeInterval;

    private Node[,] m_NodeGrid;

    public List<Node> GetAllNodes()
    {
        List<Node> _list = new List<Node>(ROW_COLUMN_COUNT * ROW_COLUMN_COUNT);
        for(int y = 0; y < ROW_COLUMN_COUNT; ++y)
        for(int x = 0; x < ROW_COLUMN_COUNT; ++x)
            _list.Add(m_NodeGrid[y, x]);
        return _list;
    }
    
    public Node GetNodeAt(int x, int y) => m_NodeGrid[y, x];
    
    public List<Node> GetNodesInPath(List<NodePosition> _path) => _path.Select(p => m_NodeGrid[p.y, p.x]).ToList();

    private void OnEnable()
    {
        CacheNodeGridIfNeed();
    }

    private void OnValidate()
    {
        CacheNodeGridIfNeed();
        
        float _gridWidth = (ROW_COLUMN_COUNT - 1) * m_NodeInterval;
        float _startPos = _gridWidth * -0.5f;

        for (int y = 0; y < ROW_COLUMN_COUNT; ++y)
        {
            for (int x = 0; x < ROW_COLUMN_COUNT; ++x)
            {
                float _xPos = _startPos + m_NodeInterval * x;
                float _yPos = _startPos + m_NodeInterval * y;
                m_NodeGrid[y, x].transform.localPosition = new Vector3(_xPos, _yPos, 0.0f);
            }
        }
    }

    private void CacheNodeGridIfNeed()
    {
        bool _shouldRecache = false;
        
        if (m_GridHolder.childCount != ROW_COLUMN_COUNT * ROW_COLUMN_COUNT)
        {
            for (int i = 0; i < m_GridHolder.childCount; ++i)
            {
                if (Application.isEditor)
                    DestroyImmediate(m_GridHolder.GetChild(i).gameObject);
                else
                    Destroy(m_GridHolder.GetChild(i).gameObject);
            }

            for (int i = 0; i < ROW_COLUMN_COUNT * ROW_COLUMN_COUNT; ++i)
                GameObject.Instantiate(m_Asset_NodePrefab, m_GridHolder);

            _shouldRecache = true;
        }
        else if(m_NodeGrid == null)
        {
            _shouldRecache = true;
        }

        if (_shouldRecache)
            CacheNodeGrid();
    }

    private void CacheNodeGrid()
    {
        m_NodeGrid = new Node[ROW_COLUMN_COUNT, ROW_COLUMN_COUNT];

        for (int y = 0; y < ROW_COLUMN_COUNT; ++y)
        {
            for (int x = 0; x < ROW_COLUMN_COUNT; ++x)
            {
                GameObject _go = m_GridHolder.GetChild(y * ROW_COLUMN_COUNT + x).gameObject;
                var _node = _go.GetComponent<Node>();
                _node.Position = new NodePosition(x, y);

                m_NodeGrid[y, x] = _node;
            }
        }
    }
}