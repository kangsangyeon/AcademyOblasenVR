using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Witch/Pattern")]
public class Pattern : ScriptableObject
{
    public string name;
    public List<NodePosition> path;

    public bool Match(List<NodePosition> _path)
    {
        if (this.path.Count != _path.Count)
            return false;

        for (int i = 0; i < this.path.Count; ++i)
        {
            if (Equals(this.path[i], _path[i]) == false)
                return false;
        }

        return true;
    }

    public bool MatchProgress(List<NodePosition> _path, out float _progress)
    {
        if (_path.Count > path.Count)
        {
            _progress = 0.0f;
            return false;
        }
        
        for (int i = 0; i < _path.Count; ++i)
        {
            if (Equals(_path[i], this.path[i]) == false)
            {
                _progress = 0.0f;
                return false;
            }
        }

        _progress = (float)_path.Count / path.Count;
        return true;
    }
}
