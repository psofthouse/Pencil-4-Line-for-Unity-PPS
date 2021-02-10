using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pcl4Editor
{
    public class PostProcessingVersionWindow : VersionWindow
    {
        [MenuItem("Pencil+ 4/About Post Processing", false, 3)]
        private static void Open()
        {
            OpenWithPackageManifestGUID("522a726eac33a4f469a1a1aec9101240");
        }
    }
}