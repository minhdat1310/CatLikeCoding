using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;
public class Graph : MonoBehaviour
{
    #region PROPERITIES

    float duration;

    bool transitioning;
    
    FunctionName transitionFunction;

    [SerializeField]
    Transform _pointPrefab;
    [SerializeField]
    FunctionName _function;

    [SerializeField, Range(10, 100)]
    int _resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    Transform[] _points;
    #endregion

    #region UNITY CORE
    // Start is called before the first frame update
    void Awake()
    {
        _points = new Transform[_resolution * _resolution];
        float step = 2f / _resolution;
        var scale = Vector3.one * step;
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i] = Instantiate(_pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning) 
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = _function;
            _function = GetNextFunctionName(_function);
        }
        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }
    #endregion

    #region MAIN
    void UpdateFunction()
    {

        Function func = GetFunction(_function);
        float time = Time.time;
        float step = 2f / _resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == _resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = func(u, v, time);
        }
    }

    void UpdateFunctionTransition()
    {
        Function
            from = GetFunction(transitionFunction),
            to = GetFunction(_function);
        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / _resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == _resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = Morph(
                u, v, time, from, to, progress
            );
        }
    }
    #endregion
}
