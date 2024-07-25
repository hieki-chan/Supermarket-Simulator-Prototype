using NUnit.Framework;
using UnityEditor.Modules;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    public class ContentPipelineTests
    {


        [Test]
        public void TestCanBuildPlayer()
        {
#if UNITY_2021_3_OR_NEWER
            // this will always return false for IsBuildTargetSupported, so it tests that pathway
            Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.NoTarget, BuildTargetGroup.Unknown, null));
#if UNITY_EDITOR_WIN
            // this can happen if the player is not installed like in yamato, it will always return true
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows))
            {
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
            }
            else
            {
                Assert.AreEqual(false, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, new TestBuildWindowExtension(true)));
            }
#elif UNITY_EDITOR_OSX
            // this can happen if the player is not installed like in yamato, it will always return true
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX))
            {
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
            } else {
                Assert.AreEqual(false, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, new TestBuildWindowExtension(true)));
            }

#elif UNITY_EDITOR_LINUX
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64))
            {
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
            } else {
                Assert.AreEqual(false, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, new TestBuildWindowExtension(false)));
                Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, new TestBuildWindowExtension(true)));
            }
#endif

#else
            Assert.AreEqual(true, ContentPipeline.CanBuildPlayer(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone));
#endif
        }

#if UNITY_2021_3_OR_NEWER
        public class TestBuildWindowExtension : IBuildWindowExtension
        {
            private bool m_EnabledBuildButton;
            public TestBuildWindowExtension(bool enabledBuildButton)
            {
                m_EnabledBuildButton = enabledBuildButton;
            }
            public void ShowPlatformBuildOptions()
            {
            }

            public void ShowPlatformBuildWarnings()
            {
            }

            public void ShowInternalPlatformBuildOptions()
            {
            }

            public bool EnabledBuildButton()
            {
                return m_EnabledBuildButton;
            }

            public bool EnabledBuildAndRunButton()
            {
                return true;
            }

            public void GetBuildButtonTitles(out GUIContent buildButtonTitle, out GUIContent buildAndRunButtonTitle)
            {
                buildButtonTitle = null;
                buildAndRunButtonTitle = null;
            }

            public bool AskForBuildLocation()
            {
                return false;
            }

            public bool ShouldDrawRunLastBuildButton()
            {
                return true;
            }

            public void DoRunLastBuildButtonGui()
            {
            }

            public bool ShouldDrawScriptDebuggingCheckbox()
            {
                return false;
            }

            public bool ShouldDrawProfilerCheckbox()
            {
                return false;
            }

            public bool ShouldDrawDevelopmentPlayerCheckbox()
            {
                return false;
            }

            public bool ShouldDrawExplicitNullCheckbox()
            {
                return false;
            }

            public bool ShouldDrawExplicitDivideByZeroCheckbox()
            {
                return false;
            }

            public bool ShouldDrawExplicitArrayBoundsCheckbox()
            {
                return false;
            }

            public bool ShouldDrawForceOptimizeScriptsCheckbox()
            {
                return false;
            }

            public bool ShouldDrawWaitForManagedDebugger()
            {
                return false;
            }

            public bool ShouldDrawManagedDebuggerFixedPort()
            {
                return false;
            }

            public bool ShouldDisableManagedDebuggerCheckboxes()
            {
                return false;
            }

            public void DoScriptsOnlyGUI()
            {
            }
        }
#endif
    }
}

