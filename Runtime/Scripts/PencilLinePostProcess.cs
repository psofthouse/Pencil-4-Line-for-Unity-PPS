using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

namespace Pencil_4
{
    [Serializable]
    [UnityEngine.Rendering.PostProcessing.PostProcess(typeof(PencilLinePostProcessRenderer), PostProcessEvent.BeforeTransparent, "Pencil+ 4/Line", false)]
    public class PencilLinePostProcess : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("")]
        public FloatParameter alpha = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (context.camera != null && enabled.value && alpha.value > 0f)
            {
                foreach (var lineEffect in context.camera.GetComponents<PencilLineEffect>())
                {
                    if (lineEffect.isPostProsessingEnabled)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class PencilLinePostProcessRenderer : PostProcessEffectRenderer<PencilLinePostProcess>
    {
        public override void Render(PostProcessRenderContext context)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && RenderMode.GameViewRenderMode == RenderMode.Mode.Off)
            {
                context.command.Blit(context.source, context.destination);
                return;
            }
#endif

            bool draw = false;

            foreach (var lineEffect in context.camera.GetComponents<PencilLineEffect>())
            {
                if (lineEffect.PencilRenderer != null && lineEffect.PencilRenderer.Texture != null && lineEffect.isPostProsessingEnabled)
                {
                    // エフェクトの重ね掛け対応
                    if (draw)
                    {
                        context.command.Blit(context.destination, context.source);
                    }

                    // テクスチャ更新設定
                    if (lineEffect.isRendering == true)
                    {
#if UNITY_2018_3_OR_NEWER
                        var callback = NativeFunctions.GetTextureUpdateCallbackV2();
#else
                        var callback = NativeFunctions.GetTextureUpdateCallback();
#endif
                        if (callback == IntPtr.Zero)
                        {
                            continue;
                        }

                        // ハンドルを取得し、ネイティブで確保したバッファが意図せず解放されないようにする
                        // ハンドルはTextureUpdateCallback()のEndで自動的に解除される
                        var textureUpdateHandle = lineEffect.PencilRenderer.RequestTextureUpdate(0);
                        if (textureUpdateHandle == 0xFFFFFFFF)
                        {
                            // PencilLinePostProcessRenderer.Render()の呼び出しがlineEffect.OnPreRender()よりも早いケースが稀にあり、
                            // PostProcessing_RenderingEventモードのときに適切なライン描画が行われない場合がある
                            continue;
                        }
#if UNITY_2018_3_OR_NEWER
                        context.command.IssuePluginCustomTextureUpdateV2(callback, lineEffect.PencilRenderer.Texture, textureUpdateHandle);
#else
                        context.command.IssuePluginCustomTextureUpdate(callback, lineEffect.PencilRenderer.Texture, textureUpdateHandle);
#endif
                        // レンダーエレメント画像出力用のテクスチャ更新
                        for (int renderElementIndex = 0; true; renderElementIndex++)
                        {
                            var renderElementTexture = lineEffect.PencilRenderer.GetRenderElementTexture(renderElementIndex);
                            var renderElementTargetTexture = lineEffect.PencilRenderer.GetRenderElementTargetTexture(renderElementIndex);
                            if (renderElementTexture == null || renderElementTargetTexture == null)
                            {
                                break;
                            }

                            textureUpdateHandle = lineEffect.PencilRenderer.RequestTextureUpdate(1 + renderElementIndex);
                            if (textureUpdateHandle == 0xFFFFFFFF)
                            {
                                break;
                            }

#if UNITY_2018_3_OR_NEWER
                            context.command.IssuePluginCustomTextureUpdateV2(callback, renderElementTexture, textureUpdateHandle);
#else
                            context.command.IssuePluginCustomTextureUpdate(callback, renderElementTexture, textureUpdateHandle);
#endif
                            context.command.Blit(renderElementTexture, renderElementTargetTexture);
                        }
                    }

                    // 描画設定
                    var sheet = context.propertySheets.Get(Shader.Find("Hidden/Pcl4LinePostProcessingStack"));
                    sheet.properties.SetTexture("_LineTex", lineEffect.PencilRenderer.Texture);
                    sheet.properties.SetFloat("_Alpha", settings.alpha.value);
                    context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0, false);

                    //
                    draw = true;
                }
            }

            // 何も描画するものがなかった場合、RenderTargetを転写しておく
            if (!draw)
            {
                context.command.Blit(context.source, context.destination);
            }
        }
    }
}
#else
namespace Pencil_4
{
    [Serializable]
    public class PencilLinePostProcess : ScriptableObject
    {
    }
}
#endif