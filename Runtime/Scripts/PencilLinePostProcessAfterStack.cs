using System;
using UnityEngine;

#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

namespace Pencil_4
{

    [Serializable]
    [PostProcess(typeof(PencilLinePostProcessRendererAfterStack), PostProcessEvent.AfterStack, "Pencil+ 4/Line (After Stack)", false)]
    public class PencilLinePostProcessAfterStack : PencilLinePostProcess
    {
    }

    public class PencilLinePostProcessRendererAfterStack : PencilLinePostProcessRenderer
    {
    }
}
#else
namespace Pencil_4
{
    [Serializable]
    public class PencilLinePostProcessAfterStack : PencilLinePostProcess
    {
    }
}
#endif