using UnityEditor;
using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEditor.DeviceSimulation;

namespace DeviceSimulation {

	class DeviceConfigValidation
	{
		[Test, TestCaseSource(typeof(DeviceAssetsEnumerable))]
		public void TestDeviceAssetCorrectness(DeviceInfoAsset deviceAsset)
		{
			Assert.NotNull(deviceAsset);
			Assert.IsTrue(deviceAsset.parseErrors == null || deviceAsset.parseErrors.Length == 0);
		}
		
		[Test, TestCaseSource(typeof(DeviceAssetsEnumerable))]
        public void TestScreenOverlayCanBeLoaded(DeviceInfoAsset deviceAsset)
        {
	        for (int i = 0; i < deviceAsset.deviceInfo.screens.Length; i++)
            {
                Assert.NotNull(DeviceLoader.LoadOverlay(deviceAsset, i));
            }
        }

        [Test, TestCaseSource(typeof(ScreenOverlayEnumerable))]
        public void TestAllOverlaysAreUsed(string texturePath)
        {
	        var used = false;
	        foreach (object[] deviceAssetContainer in new DeviceAssetsEnumerable())
	        {
		        var deviceAsset = deviceAssetContainer[0] as DeviceInfoAsset;
		        foreach (var screen in deviceAsset.deviceInfo.screens)
		        {
			        if(texturePath.ToLower().Contains(screen.presentation.overlayPath.ToLower()))
			        {
				        used = true;
				        break;
			        }
		        }

		        if (used)
			        break;
	        }
	        Assert.True(used);
        }
	}

	class DeviceAssetsEnumerable : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			var deviceInfoGUIDs = AssetDatabase.FindAssets("t:DeviceInfoAsset");
			foreach (var guid in deviceInfoGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var deviceAsset = AssetDatabase.LoadAssetAtPath<DeviceInfoAsset>(assetPath);
				deviceAsset.directory = Path.GetDirectoryName(assetPath);
				yield return new object[] {deviceAsset};
			}
		}
	}
	
	class ScreenOverlayEnumerable : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			var deviceInfoGUIDs = AssetDatabase.FindAssets("t:Texture", new []{"Packages/com.unity.device-simulator.devices/Editor/Devices"});
			foreach (var guid in deviceInfoGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				yield return new object[] {assetPath};
			}
		}
	}
}
