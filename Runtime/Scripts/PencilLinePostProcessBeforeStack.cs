using System;
using UnityEngine;

#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

namespace Pencil_4
{

    [Serializable]
    [PostProcess(typeof(PencilLinePostProcessRendererBeforeStack), PostProcessEvent.BeforeStack, "Pencil+ 4/Line (Before Stack)", false)]
    public class PencilLinePostProcessBeforeStack : PencilLinePostProcess
    {
    }

    public class PencilLinePostProcessRendererBeforeStack : PencilLinePostProcessRenderer
    {
    }
}
#else
namespace Pencil_4
{
    [Serializable]
    public class PencilLinePostProcessBeforeStack : PencilLinePostProcess
    {
    }
}
#endif