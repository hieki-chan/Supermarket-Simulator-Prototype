using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

#if !UNITY_2019_3_OR_NEWER
using System.IO;
#endif

namespace UnityEditor.Build.Pipeline.Tasks
{
    /// <summary>
    /// Calculates the dependency data for all scenes.
    /// </summary>
    public class CalculateSceneDependencyData : IBuildTask
    {
        /// <inheritdoc />
        public int Version { get { return 6; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IBuildContent m_Content;

        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.InOut, true)]
        IBuildResults m_Results;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;

        [InjectContext(ContextUsage.In, true)]
        IBuildCache m_Cache;

        [InjectContext(ContextUsage.In, true)]
        IBuildLogger m_Log;
#pragma warning restore 649

        CacheEntry GetCacheEntry(GUID asset)
        {
            CacheEntry entry;
#if NONRECURSIVE_DEPENDENCY_DATA
            entry = m_Cache.GetCacheEntry(asset, m_Parameters.NonRecursiveDependencies ? -Version : Version);
#else
            entry = m_Cache.GetCacheEntry(asset, Version);
#endif
            return entry;
        }

        CacheEntry GetCacheEntry(string asset)
        {
#if UNITY_2020_1_OR_NEWER
            GUID guid = AssetDatabase.AssetPathToGUID_Internal(asset);
#else
            string stringGUID = AssetDatabase.AssetPathToGUID(asset);
            if (GUID.TryParse(stringGUID, out GUID guid))
                return GetCacheEntry(guid);
#endif
            if (!guid.Empty())
                return GetCacheEntry(guid);

#if NONRECURSIVE_DEPENDENCY_DATA
            return m_Cache.GetCacheEntry(asset, m_Parameters.NonRecursiveDependencies ? -Version : Version);
#else
            return m_Cache.GetCacheEntry(asset, Version);
#endif
        }

        CachedInfo GetCachedInfo(GUID scene, IEnumerable<ObjectIdentifier> references, SceneDependencyInfo sceneInfo, BuildUsageTagSet usageTags, IEnumerable<CacheEntry> prefabEntries, Hash128 prefabDependency)
        {
            var info = new CachedInfo();
            info.Asset = GetCacheEntry(scene);

#if ENABLE_TYPE_HASHING || UNITY_2020_1_OR_NEWER
            var uniqueTypes = new HashSet<System.Type>(sceneInfo.includedTypes);
#else
            var uniqueTypes = new HashSet<System.Type>();
#endif
            var objectTypes = new List<ObjectTypes>();
            var dependencies = new HashSet<CacheEntry>(prefabEntries);
            ExtensionMethods.ExtractCommonCacheData(m_Cache, null, references, uniqueTypes, objectTypes, dependencies);
            info.Dependencies = dependencies.ToArray();

            info.Data = new object[] { sceneInfo, usageTags, prefabDependency, objectTypes };

            return info;
        }

        /// <inheritdoc />
        public ReturnCode Run()
        {
            if (m_Content.Scenes.IsNullOrEmpty())
                return ReturnCode.SuccessNotRun;

            IList<CachedInfo> cachedInfo = null;
            IList<CachedInfo> uncachedInfo = null;
            if (m_Parameters.UseCache && m_Cache != null)
            {
                IList<CacheEntry> entries = m_Content.Scenes.Select(x => GetCacheEntry(x)).ToList();
                m_Cache.LoadCachedData(entries, out cachedInfo);

                uncachedInfo = new List<CachedInfo>();
            }

            List<CachedInfo> info = new List<CachedInfo>(m_Content.Scenes.Count);
            BuildSettings settings = m_Parameters.GetContentBuildSettings();
            for (int i = 0; i < m_Content.Scenes.Count; i++)
            {
                using (m_Log.ScopedStep(LogLevel.Info, "Calculate Scene Dependencies"))
                {
                    GUID scene = m_Content.Scenes[i];
                    string scenePath = AssetDatabase.GUIDToAssetPath(scene.ToString());

                    SceneDependencyInfo sceneInfo;
                    BuildUsageTagSet usageTags;
                    Hash128 prefabDependency = new Hash128();
                    bool useCachedScene = false;

                    if (cachedInfo != null && cachedInfo[i] != null)
                    {
                        useCachedScene = true;
                        if (!m_Tracker.UpdateInfoUnchecked(string.Format("{0} (Cached)", scenePath)))
                            return ReturnCode.Canceled;
                        m_Log.AddEntrySafe(LogLevel.Info, $"{scene} (cached)");

                        sceneInfo = (SceneDependencyInfo)cachedInfo[i].Data[0];
                        // case 1288677: Update scenePath in case scene moved, but didn't change
                        sceneInfo.SetScene(scenePath);
                        usageTags = cachedInfo[i].Data[1] as BuildUsageTagSet;
                        prefabDependency = (Hash128)cachedInfo[i].Data[2];
                        var objectTypes = cachedInfo[i].Data[3] as List<ObjectTypes>;
                        if (objectTypes != null)
                        {
                            BuildCacheUtility.SetTypeForObjects(objectTypes);

                            foreach (var objectType in objectTypes)
                            {
                                //Sprite association to SpriteAtlas might have changed since last time data was cached, this might
                                //imply that we have stale data in our cache, if so ensure we regenerate the data.
                                if (objectType.Types[0] == typeof(UnityEngine.Sprite))
                                {
                                    ObjectIdentifier[] filteredReferences = sceneInfo.referencedObjects.ToArray();
                                    ObjectIdentifier[] filteredReferencesNew = null;
#if NONRECURSIVE_DEPENDENCY_DATA
                                    if (m_Parameters.NonRecursiveDependencies)
                                    {
                                        var sceneInfoNew = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache, DependencyType.ValidReferences);
                                        filteredReferencesNew = sceneInfoNew.referencedObjects.ToArray();
                                        filteredReferencesNew = ExtensionMethods.FilterReferencedObjectIDs(scene, filteredReferencesNew, m_Parameters.Target, m_Parameters.ScriptInfo, new HashSet<GUID>(m_Content.Assets));
                                    }
                                    else
#endif
                                    {
                                        var sceneInfoNew = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache);
                                        filteredReferencesNew = sceneInfoNew.referencedObjects.ToArray();
                                    }

                                    if (Enumerable.SequenceEqual(filteredReferences,filteredReferencesNew) == false)
                                    {
                                        useCachedScene = false;
                                    }
                                    // ADDR-3596: break must be here to prevent CalculatePlayerDependenciesForScene from being called unnecessarily.
                                    break;
                                }
                            }

                        }

                        if (useCachedScene)
                        {
                            SetOutputInformation(scene, sceneInfo, usageTags, prefabDependency);
                            info.Add(cachedInfo[i]);
                        }
                    }

                    if (!useCachedScene)
                    {
                        if (!m_Tracker.UpdateInfoUnchecked(scenePath))
                            return ReturnCode.Canceled;
                        m_Log.AddEntrySafe(LogLevel.Info, $"{scene}");

                        usageTags = new BuildUsageTagSet();

#if UNITY_2019_3_OR_NEWER
#if NONRECURSIVE_DEPENDENCY_DATA
                        if ( m_Parameters.NonRecursiveDependencies)
                        {
                            sceneInfo = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache, DependencyType.ValidReferences);
                            ObjectIdentifier[] filteredReferences = sceneInfo.referencedObjects.ToArray();
                            filteredReferences = ExtensionMethods.FilterReferencedObjectIDs(scene, filteredReferences, m_Parameters.Target, m_Parameters.ScriptInfo, new HashSet<GUID>(m_Content.Assets));
                            ContentBuildInterface.CalculateBuildUsageTags(filteredReferences, filteredReferences, sceneInfo.globalUsage, usageTags);
                            sceneInfo.SetReferencedObjects(filteredReferences);
                        }
                        else
#endif
                        {
                            sceneInfo = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache);
                        }
#else
                        string outputFolder = m_Parameters.TempOutputFolder;
                        if (m_Parameters.UseCache && m_Cache != null)
                            outputFolder = m_Cache.GetCachedArtifactsDirectory(GetSceneCacheEntry(scene, Version));
                        System.IO.Directory.CreateDirectory(outputFolder);

                        sceneInfo = ContentBuildInterface.PrepareScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache, outputFolder);
#endif
                        if (uncachedInfo != null)
                        {
                            // We only need to gather prefab dependencies and calculate the hash if we are using caching, otherwise we can skip it
                            string[] sceneDependencyPaths = AssetDatabase.GetDependencies(scenePath);
                            List<CacheEntry> prefabEntries = new List<CacheEntry>();
                            foreach (string assetPath in sceneDependencyPaths)
                            {
                                if (assetPath.EndsWith(".prefab", StringComparison.Ordinal))
                                    prefabEntries.Add(GetCacheEntry(assetPath));
                            }
                            prefabDependency = HashingMethods.Calculate(prefabEntries).ToHash128();
                            var cacheInfo = GetCachedInfo(scene, sceneInfo.referencedObjects, sceneInfo, usageTags, prefabEntries, prefabDependency);
                            uncachedInfo.Add(cacheInfo);
                            info.Add(cacheInfo);
                        }
                        SetOutputInformation(scene, sceneInfo, usageTags, prefabDependency);
                    }
                }
            }

            if (m_Parameters.UseCache && m_Cache != null)
                m_Cache.SaveCachedData(uncachedInfo);

            if (info != null && m_Results != null)
            {
                foreach (CachedInfo sceneFileInfo in info)
                {
                    Dictionary<ObjectIdentifier, System.Type[]> objectTypes = new Dictionary<ObjectIdentifier, System.Type[]>();
                    List<ObjectTypes> types = sceneFileInfo.Data[3] as List<ObjectTypes>;
                    foreach (var objectType in types)
                        objectTypes.Add(objectType.ObjectID, objectType.Types);

                    AssetResultData resultData = new AssetResultData
                    {
                        Guid = sceneFileInfo.Asset.Guid,
                        Hash = sceneFileInfo.Asset.Hash,
                        IncludedObjects = new List<ObjectIdentifier>(),
                        ReferencedObjects = null,
                        ObjectTypes = objectTypes
                    };
                    m_Results.AssetResults.Add(resultData.Guid, resultData);
                }
            }

            return ReturnCode.Success;
        }

        void SetOutputInformation(GUID asset, SceneDependencyInfo sceneInfo, BuildUsageTagSet usageTags, Hash128 prefabDependency)
        {
            // Add generated scene information to BuildDependencyData
            m_DependencyData.SceneInfo.Add(asset, sceneInfo);
            m_DependencyData.SceneUsage.Add(asset, usageTags);
            m_DependencyData.DependencyHash.Add(asset, prefabDependency);
        }
    }
}
