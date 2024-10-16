using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FunctionLibrary;

public class GPUGraph : MonoBehaviour
{
    #region PROPERITIES

    float duration;

    bool transitioning;

    FunctionName transitionFunction;

    [SerializeField]
    FunctionName _function;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    ComputeShader computeShader;
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    ComputeBuffer positionsBuffer;

    const int maxResolution = 1000;

    [SerializeField, Range(10, maxResolution)]
    int _resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    #endregion

    #region UNITY CORE
    void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }


    // Start is called before the first frame update
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
            PickNextFunction();
        }
        UpdateFunctionOnGPU();
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    #endregion

    #region MAIN
    void PickNextFunction()
    {
        _function = transitionMode == TransitionMode.Cycle ?
            GetNextFunctionName(_function) :
            GetRandomFunctionNameOtherThan(_function);
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / _resolution;
        computeShader.SetInt(resolutionId, _resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }
        var kernelIndex =
                (int)_function + (int)(transitioning ? transitionFunction : _function) * 5;
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(_resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / _resolution));
        Graphics.DrawMeshInstancedProcedural(
            mesh, 0, material, bounds, _resolution * _resolution
        );
    }

    #endregion
}
