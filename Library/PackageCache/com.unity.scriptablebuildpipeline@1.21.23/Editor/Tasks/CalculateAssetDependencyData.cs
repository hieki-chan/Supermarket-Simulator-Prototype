using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Player;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Tasks
{
    [Serializable]
    internal class ObjectDependencyInfo
    {
        public ObjectIdentifier Object;
        public List<ObjectIdentifier> Dependencies = new List<ObjectIdentifier>();
    }

#if !UNITY_2020_2_OR_NEWER
    internal class CalculateAssetDependencyHooks
    {
        public virtual UnityEngine.Object[] LoadAllAssetRepresentationsAtPath(string assetPath)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
        }
    }
#endif

    /// <summary>
    /// Calculates the dependency data for all assets.
    /// </summary>
    public class CalculateAssetDependencyData : IBuildTask
    {
        internal const int kVersion = 6;
        /// <inheritdoc />
        public int Version { get { return kVersion; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBundleBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IBuildContent m_Content;

        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext]
        IObjectDependencyData m_ObjectDependencyData;

        [InjectContext(ContextUsage.InOut, true)]
        IBuildResults m_Results;

        [InjectContext(ContextUsage.InOut, true)]
        IBuildSpriteData m_SpriteData;

        [InjectContext(ContextUsage.InOut, true)]
        IBuildExtendedAssetData m_ExtendedAssetData;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;

        [InjectContext(ContextUsage.In, true)]
        IBuildCache m_Cache;

        [InjectContext(ContextUsage.In, true)]
        IBuildLogger m_Log;
#pragma warning restore 649

        internal struct TaskInput
        {
            public IBuildCache BuildCache;
            public BuildTarget Target;
            public TypeDB TypeDB;
            public List<GUID> Assets;
            public IProgressTracker ProgressTracker;
            public BuildUsageTagGlobal GlobalUsage;
            public BuildUsageCache DependencyUsageCache;
#if !UNITY_2020_2_OR_NEWER
            public CalculateAssetDependencyHooks EngineHooks;
#endif
            public bool NonRecursiveDependencies;
            public IBuildLogger Logger;
        }

        internal struct AssetOutput
        {
            public GUID asset;
            public Hash128 Hash;
            public AssetLoadInfo assetInfo;
            public List<ObjectDependencyInfo> objectDependencyInfo;
            public BuildUsageTagSet usageTags;
            public SpriteImporterData spriteData;
            public ExtendedAssetData extendedData;
            public List<ObjectTypes> objectTypes;
        }

        internal struct TaskOutput
        {
            public AssetOutput[] AssetResults;
            public int CachedAssetCount;
        }

        static CacheEntry GetCacheEntry(GUID asset, TaskInput input)
        {
            if (input.BuildCache == null)
                return default;
#if NONRECURSIVE_DEPENDENCY_DATA
            CacheEntry entry = input.BuildCache.GetCacheEntry(asset, input.NonRecursiveDependencies ? -kVersion : kVersion);
#else
            CacheEntry entry = input.BuildCache.GetCacheEntry(asset, Version);
#endif

            return entry;
        }

        static CachedInfo GetCachedInfo(TaskInput input, GUID asset, AssetLoadInfo assetInfo, List<ObjectDependencyInfo> objectDependencies, BuildUsageTagSet usageTags, SpriteImporterData importerData, ExtendedAssetData assetData)
        {
            var info = new CachedInfo();
            info.Asset = GetCacheEntry(asset, input);

            var uniqueTypes = new HashSet<System.Type>();
            var objectTypes = new List<ObjectTypes>();
            var dependencies = new HashSet<CacheEntry>();
            ExtensionMethods.ExtractCommonCacheData(input.BuildCache, assetInfo.includedObjects, assetInfo.referencedObjects, uniqueTypes, objectTypes, dependencies);
            info.Dependencies = dependencies.ToArray();

            info.Data = new object[] { assetInfo, usageTags, importerData, assetData, objectTypes, objectDependencies };
            return info;
        }

        /// <inheritdoc />
        public ReturnCode Run()
        {
            TaskInput input = new TaskInput();
            input.Target = m_Parameters.Target;
            input.TypeDB = m_Parameters.ScriptInfo;
            input.BuildCache = m_Parameters.UseCache ? m_Cache : null;
#if NONRECURSIVE_DEPENDENCY_DATA
            input.NonRecursiveDependencies = m_Parameters.NonRecursiveDependencies;
#else
            input.NonRecursiveDependencies = false;
#endif
            input.Assets = m_Content.Assets;
            input.ProgressTracker = m_Tracker;
            input.DependencyUsageCache = m_DependencyData.DependencyUsageCache;
            input.GlobalUsage = m_DependencyData.GlobalUsage;
            input.Logger = m_Log;
            foreach (SceneDependencyInfo sceneInfo in m_DependencyData.SceneInfo.Values)
                input.GlobalUsage |= sceneInfo.globalUsage;

            ReturnCode code = RunInternal(input, out TaskOutput output);
            if (code == ReturnCode.Success)
            {
                foreach (AssetOutput o in output.AssetResults)
                {
                    m_DependencyData.AssetInfo.Add(o.asset, o.assetInfo);
                    foreach (var objectDependencyInfo in o.objectDependencyInfo)
                        m_ObjectDependencyData.ObjectDependencyMap[objectDependencyInfo.Object] = objectDependencyInfo.Dependencies;
                }

                foreach (AssetOutput assetOutput in output.AssetResults)
                {
#if NONRECURSIVE_DEPENDENCY_DATA
                    if (!input.NonRecursiveDependencies)
                        ExpandReferences(assetOutput, m_ObjectDependencyData.ObjectDependencyMap);
#endif

                    m_DependencyData.AssetUsage.Add(assetOutput.asset, assetOutput.usageTags);

                    Dictionary<ObjectIdentifier, System.Type[]> objectTypes = new Dictionary<ObjectIdentifier, Type[]>();
                    foreach (var objectType in assetOutput.objectTypes)
                        objectTypes.Add(objectType.ObjectID, objectType.Types);

                    if (m_Results != null)
                    {
                        AssetResultData resultData = new AssetResultData
                        {
                            Guid = assetOutput.asset,
                            Hash = assetOutput.Hash,
                            IncludedObjects = assetOutput.assetInfo.includedObjects,
                            ReferencedObjects = assetOutput.assetInfo.referencedObjects,
                            ObjectTypes = objectTypes
                        };
                        m_Results.AssetResults.Add(assetOutput.asset, resultData);
                    }

                    if (assetOutput.spriteData != null)
                    {
                        if (m_SpriteData == null)
                            m_SpriteData = new BuildSpriteData();
                        m_SpriteData.ImporterData.Add(assetOutput.asset, assetOutput.spriteData);
                    }

                    if (!m_Parameters.DisableVisibleSubAssetRepresentations && assetOutput.extendedData != null)
                    {
                        if (m_ExtendedAssetData == null)
                            m_ExtendedAssetData = new BuildExtendedAssetData();
                        m_ExtendedAssetData.ExtendedData.Add(assetOutput.asset, assetOutput.extendedData);
                    }

                    if (assetOutput.objectTypes != null)
                        BuildCacheUtility.SetTypeForObjects(assetOutput.objectTypes);
                }
            }

            return code;
        }

        // expand dependencies to explicit assets, results in the same output as recursive dependency calculation
        private void ExpandReferences(AssetOutput assetOutput, Dictionary<ObjectIdentifier, List<ObjectIdentifier>> objectDependencyMap)
        {
            HashSet<ObjectIdentifier> processed = new HashSet<ObjectIdentifier>();
            HashSet<ObjectIdentifier> referencedObjects = new HashSet<ObjectIdentifier>(assetOutput.assetInfo.referencedObjects);
            Stack<ObjectIdentifier> processStack = new Stack<ObjectIdentifier>(1024);
            if (assetOutput.objectDependencyInfo != null && assetOutput.objectDependencyInfo.Count > 0)
            {
                foreach (ObjectDependencyInfo info in assetOutput.objectDependencyInfo)
                {
                    foreach (ObjectIdentifier dependency in info.Dependencies)
                    {
                        if (processed.Contains(dependency) || dependency.guid == assetOutput.asset)
                            continue;

                        processStack.Push(dependency);
                        referencedObjects.Add(dependency);
                    }

                    // internal object to asset o
                    processed.Add(info.Object);
                }

                List<ObjectIdentifier> dependencies;
                while (processStack.Count > 0)
                {
                    var dep = processStack.Pop();
                    processed.Add(dep);
                    if (!objectDependencyMap.TryGetValue(dep, out dependencies) || dependencies.Count == 0)
                        continue;

                    foreach (ObjectIdentifier dependency in dependencies)
                    {
                        if (processed.Contains(dependency) || dependency.guid == assetOutput.asset)
                            continue;

                        processStack.Push(dependency);
                        referencedObjects.Add(dependency);
                    }
                }

                // go through all object dependencies that are not to self, add to stack
                // loop through external dependencies
                var refs = new List<ObjectIdentifier>(referencedObjects.ToArray());

                // to mimic recursive dependency calculation
                assetOutput.assetInfo.referencedObjects = refs;
            }
        }

#if !UNITY_2020_2_OR_NEWER
        static internal void GatherAssetRepresentations(string assetPath, System.Func<string, UnityEngine.Object[]> loadAllAssetRepresentations, ObjectIdentifier[] includedObjects, out ExtendedAssetData extendedData)
        {
            extendedData = null;
            var representations = loadAllAssetRepresentations(assetPath);
            if (representations.IsNullOrEmpty())
                return;

            var resultData = new ExtendedAssetData();
            for (int j = 0; j < representations.Length; j++)
            {
                if (representations[j] == null)
                {
                    BuildLogger.LogWarning($"SubAsset {j} inside {assetPath} is null. It will not be included in the build.");
                    continue;
                }

                if (AssetDatabase.IsMainAsset(representations[j]))
                    continue;

                string guid;
                long localId;
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(representations[j], out guid, out localId))
                    continue;

                resultData.Representations.AddRange(includedObjects.Where(x => x.localIdentifierInFile == localId));
            }

            if (resultData.Representations.Count > 0)
                extendedData = resultData;
        }

#else
        static internal void GatherAssetRepresentations(GUID asset, BuildTarget target, ObjectIdentifier[] includedObjects, out ExtendedAssetData extendedData)
        {
            extendedData = null;
            var includeSet = new HashSet<ObjectIdentifier>(includedObjects);
            // GetPlayerAssetRepresentations can return editor only objects, filter out those to only include what is in includedObjects
            ObjectIdentifier[] representations = ContentBuildInterface.GetPlayerAssetRepresentations(asset, target);
            var filteredRepresentations = representations.Where(includeSet.Contains);
            // Main Asset always returns at index 0, we only want representations, so check for greater than 1 length
            if (representations.IsNullOrEmpty() || filteredRepresentations.Count() < 2)
                return;

            extendedData = new ExtendedAssetData();
            extendedData.Representations.AddRange(filteredRepresentations.Skip(1));
        }

#endif

        static internal ReturnCode RunInternal(TaskInput input, out TaskOutput output)
        {
#if !UNITY_2020_2_OR_NEWER
            input.EngineHooks = input.EngineHooks != null ? input.EngineHooks : new CalculateAssetDependencyHooks();
#endif
            output = new TaskOutput();
            output.AssetResults = new AssetOutput[input.Assets.Count];

            IList<CachedInfo> cachedInfo = null;
            using (input.Logger.ScopedStep(LogLevel.Info, "Gathering Cache Entries to Load"))
            {
                if (input.BuildCache != null)
                {
                    IList<CacheEntry> entries = input.Assets.Select(x => GetCacheEntry(x, input)).ToList();
                    input.BuildCache.LoadCachedData(entries, out cachedInfo);
                }
            }

            HashSet<GUID> explicitAssets = new HashSet<GUID>(input.Assets);
            Dictionary<GUID, AssetOutput> implicitAssetsOutput = new Dictionary<GUID, AssetOutput>();

            // Populate the collection of packed sprites from the cache
            HashSet<GUID> packedSprites = new HashSet<GUID>();
            for (int i = 0; i < input.Assets.Count; i++)
            {
                if (cachedInfo != null && cachedInfo[i] != null)
                {
                    var guid = input.Assets[i];
                    var spriteData = cachedInfo[i].Data[2] as SpriteImporterData;
                    if (spriteData != null && spriteData.PackedSprite)
                        packedSprites.Add(guid);
                }
            }

            Queue<int> assetsToProcess = new Queue<int>();
            for (int i = 0; i < input.Assets.Count; i++)
            {
                using (input.Logger.ScopedStep(LogLevel.Info, "Calculate Asset Dependencies"))
                {
                    if (cachedInfo != null && cachedInfo[i] != null)
                    {
                        AssetOutput assetResult = new AssetOutput();
                        assetResult.asset = input.Assets[i];

                        var objectTypes = cachedInfo[i].Data[4] as List<ObjectTypes>;
                        var assetInfos = cachedInfo[i].Data[0] as AssetLoadInfo;
                        var objectDependencyInfo = cachedInfo[i].Data[5] as List<ObjectDependencyInfo>;

                        bool useCachedData = true;
                        foreach (var objectType in objectTypes)
                        {
                            //Sprite association to SpriteAtlas might have changed since last time data was cached, this might
                            //imply that we have stale data in our cache, if so ensure we regenerate the data.
                            if (objectType.Types[0] == typeof(UnityEngine.Sprite))
                            {
                                var referencedObjectOld = assetInfos.referencedObjects.ToArray();
                                ObjectIdentifier[] referencedObjectsNew = null;
#if NONRECURSIVE_DEPENDENCY_DATA
                                referencedObjectsNew = GetPlayerDependenciesForAsset(input.Assets[i], assetInfos.includedObjects.ToArray(), input, assetResult, explicitAssets, implicitAssetsOutput, packedSprites);
#else
                                referencedObjectsNew = ContentBuildInterface.GetPlayerDependenciesForObjects(assetInfos.includedObjects.ToArray(), input.Target, input.TypeDB);
#endif

                                if (Enumerable.SequenceEqual(referencedObjectOld, referencedObjectsNew) == false)
                                {
                                    useCachedData = false;
                                }
                                break;
                            }
                        }
                        if (useCachedData)
                        {
                            assetResult.assetInfo = assetInfos;
                            assetResult.objectDependencyInfo = objectDependencyInfo;
                            assetResult.usageTags = cachedInfo[i].Data[1] as BuildUsageTagSet;
                            assetResult.spriteData = cachedInfo[i].Data[2] as SpriteImporterData;
                            assetResult.extendedData = cachedInfo[i].Data[3] as ExtendedAssetData;
                            assetResult.objectTypes = objectTypes;
                            output.AssetResults[i] = assetResult;
                            output.CachedAssetCount++;
                            input.Logger.AddEntrySafe(LogLevel.Info, $"{assetResult.asset} (cached)");
                            continue;
                        }
                    }

                    GUID asset = input.Assets[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(asset.ToString());

                    if (!input.ProgressTracker.UpdateInfoUnchecked(assetPath))
                        return ReturnCode.Canceled;

                    // Process uncached Sprites first, then all other uncached assets
                    var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null && importer.textureType == TextureImporterType.Sprite)
                        output.AssetResults[i] = ProcessAsset(true, asset, assetPath, input, explicitAssets, implicitAssetsOutput, packedSprites, importer);
                    else
                        assetsToProcess.Enqueue(i);
                }
            }

            // Process all other uncached assets
            while (assetsToProcess.Count > 0)
            {
                int i = assetsToProcess.Dequeue();
                GUID asset = input.Assets[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(asset.ToString());
                output.AssetResults[i] = ProcessAsset(false, asset, assetPath, input, explicitAssets, implicitAssetsOutput, packedSprites);
            }

            using (input.Logger.ScopedStep(LogLevel.Info, "Gathering Cache Entries to Save"))
            {
                if (input.BuildCache != null)
                {
                    List<CachedInfo> toCache = new List<CachedInfo>();
                    for (int i = 0; i < input.Assets.Count; i++)
                    {
                        AssetOutput r = output.AssetResults[i];
                        CachedInfo info = cachedInfo[i];
                        if (info == null)
                        {
                            info = GetCachedInfo(input, input.Assets[i], r.assetInfo, r.objectDependencyInfo, r.usageTags, r.spriteData, r.extendedData);
                            toCache.Add(info);
                        }

                        r.Hash = info.Asset.Hash;
                        if (r.objectTypes == null && info.Data.Length > 4)
                        {
                            List<ObjectTypes> types = info.Data[4] as List<ObjectTypes>;
                            r.objectTypes = types;
                        }
                        output.AssetResults[i] = r;
                    }
                    input.BuildCache.SaveCachedData(toCache);
                }
                else
                {
                    for (int i = 0; i < input.Assets.Count; i++)
                    {
                        AssetOutput r = output.AssetResults[i];
                        if (r.objectTypes != null)
                            continue;

                        var info = BuildCacheUtility.GetCacheEntry(input.Assets[i], input.NonRecursiveDependencies ? -kVersion : kVersion);
                        r.Hash = info.Hash;

                        r.objectTypes = new List<ObjectTypes>(r.assetInfo.includedObjects.Count);
                        foreach (var objectId in r.assetInfo.includedObjects)
                        {
                            var types = BuildCacheUtility.GetSortedUniqueTypesForObject(objectId);
                            r.objectTypes.Add(new ObjectTypes(objectId, types));
                        }
                        output.AssetResults[i] = r;
                    }
                }
            }

            return ReturnCode.Success;
        }

        private static AssetOutput ProcessAsset(bool isSprite, GUID asset, string assetPath, TaskInput input, HashSet<GUID> explicitAssets,
            in Dictionary<GUID, AssetOutput> implicitAssetsOutput, HashSet<GUID> packedSprites, TextureImporter importer = null)
        {
            AssetOutput assetResult = new AssetOutput();
            assetResult.asset = asset;

            input.Logger.AddEntrySafe(LogLevel.Info, $"{assetResult.asset}");

            assetResult.assetInfo = new AssetLoadInfo();
            assetResult.objectDependencyInfo = new List<ObjectDependencyInfo>();
            assetResult.usageTags = new BuildUsageTagSet();

            assetResult.assetInfo.asset = asset;
            var includedObjects = ContentBuildInterface.GetPlayerObjectIdentifiersInAsset(asset, input.Target);
            assetResult.assetInfo.includedObjects = new List<ObjectIdentifier>(includedObjects);
            ObjectIdentifier[] referencedObjects;
#if NONRECURSIVE_DEPENDENCY_DATA
            referencedObjects = GetPlayerDependenciesForAsset(asset, includedObjects, input, assetResult, explicitAssets, implicitAssetsOutput, packedSprites);
#else
            referencedObjects = ContentBuildInterface.GetPlayerDependenciesForObjects(includedObjects, input.Target, input.TypeDB);
#endif
            assetResult.assetInfo.referencedObjects = new List<ObjectIdentifier>(referencedObjects);
            var allObjects = new List<ObjectIdentifier>(includedObjects);
            allObjects.AddRange(referencedObjects);
            ContentBuildInterface.CalculateBuildUsageTags(allObjects.ToArray(), includedObjects, input.GlobalUsage, assetResult.usageTags, input.DependencyUsageCache);

            if (isSprite)
            {
                assetResult.spriteData = new SpriteImporterData();
                assetResult.spriteData.PackedSprite = false;
                assetResult.spriteData.SourceTexture = includedObjects.FirstOrDefault();

                if (EditorSettings.spritePackerMode != SpritePackerMode.Disabled)
                {
                    foreach (var obj in referencedObjects)
                    {
                        var t = BuildCacheUtility.GetMainTypeForObject(obj);
                        if (t != typeof(Texture2D))
                        {
                            assetResult.spriteData.PackedSprite = true;
                            break;
                        }
                    }
                }

#if !UNITY_2020_1_OR_NEWER
                if (EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOn || EditorSettings.spritePackerMode == SpritePackerMode.BuildTimeOnly)
                    assetResult.spriteData.PackedSprite = !string.IsNullOrEmpty(importer.spritePackingTag);
#endif
                if (assetResult.spriteData.PackedSprite)
                    packedSprites.Add(asset);
            }

#if !UNITY_2020_2_OR_NEWER
            GatherAssetRepresentations(assetPath, input.EngineHooks.LoadAllAssetRepresentationsAtPath, includedObjects, out assetResult.extendedData);
#else
            GatherAssetRepresentations(asset, input.Target, includedObjects, out assetResult.extendedData);
#endif
            return assetResult;
        }

        private static ObjectIdentifier[] GetPlayerDependenciesForAsset(GUID inputAssetGuid, ObjectIdentifier[] includedObjects, TaskInput input, AssetOutput assetResult, HashSet<GUID> explicitAssets,
            in Dictionary<GUID, AssetOutput> implicitAssetsOutput, HashSet<GUID> packedSprites)
        {
            HashSet<ObjectIdentifier> otherReferencedAssetObjectsHashSet = new HashSet<ObjectIdentifier>();
            ObjectIdentifier[] singleObjectIdArray = new ObjectIdentifier[1];
            foreach (ObjectIdentifier subObject in includedObjects)
            {
                singleObjectIdArray[0] = subObject;
                ObjectIdentifier[] objs = ContentBuildInterface.GetPlayerDependenciesForObjects(singleObjectIdArray, input.Target, input.TypeDB, DependencyType.ValidReferences);
                foreach (ObjectIdentifier objRef in objs)
                {
                    // inputAssetGuid is an explicit build asset, so it is all objects in the asset and do not need to include them
                    if (objRef.guid == inputAssetGuid)
                        continue;
                    otherReferencedAssetObjectsHashSet.Add(objRef);
                }

                if (objs.Length > 0)
                {
                    if (assetResult.objectDependencyInfo == null)
                        assetResult.objectDependencyInfo = new List<ObjectDependencyInfo>();
                    assetResult.objectDependencyInfo.Add(new ObjectDependencyInfo()
                    {
                        Object = subObject,
                        Dependencies = new List<ObjectIdentifier>(objs)
                    });
                }
            }

            var collectedImmediateReferences = new HashSet<ObjectIdentifier>();
            var encounteredExplicitAssetDependencies = new HashSet<ObjectIdentifier>();
            var mainRepresentationNeeded = new HashSet<GUID>();

            Stack<ObjectIdentifier> objectLookingAt = new Stack<ObjectIdentifier>(otherReferencedAssetObjectsHashSet);
            while (objectLookingAt.Count > 0)
            {
                var obj = objectLookingAt.Pop();

                // Track which roots we encounter to do dependency pruning
                if (obj.guid != inputAssetGuid && explicitAssets.Contains(obj.guid))
                {
                    encounteredExplicitAssetDependencies.Add(obj); // might just be able to add to collected

                    // Don't include source textures for packed sprites
                    if (!packedSprites.Contains(obj.guid))
                        mainRepresentationNeeded.Add(obj.guid);
                }
                // looking for implicit assets we have not visited yet
                else if (!explicitAssets.Contains(obj.guid) && !collectedImmediateReferences.Contains(obj))
                {
                    collectedImmediateReferences.Add(obj);

                    ObjectIdentifier[] referencedObjects = null;
                    if (!implicitAssetsOutput.TryGetValue(obj.guid, out var implicitOutput))
                    {
                        implicitOutput = new AssetOutput()
                        {
                            asset = obj.guid,
                            objectDependencyInfo = new List<ObjectDependencyInfo>()
                        };
                        implicitAssetsOutput[obj.guid] = implicitOutput;
                    }
                    else // implicit player dependencies for asset is cached, check for specific object
                    {
                        List<ObjectIdentifier> foundObjectDependencies = null;
                        foreach (ObjectDependencyInfo dependencyInfo in implicitOutput.objectDependencyInfo)
                        {
                            if (dependencyInfo.Object == obj)
                            {
                                foundObjectDependencies = dependencyInfo.Dependencies;
                                break;
                            }
                        }

                        if (foundObjectDependencies != null)
                        {
                            foreach (ObjectIdentifier o in foundObjectDependencies)
                                objectLookingAt.Push(o);

                            if (foundObjectDependencies.Count > 0)
                            {
                                assetResult.objectDependencyInfo.Add(new ObjectDependencyInfo()
                                {
                                    Object = obj,
                                    Dependencies = foundObjectDependencies
                                });
                            }
                            continue;
                        }
                    }

                    // have not got the object references for this object yet
                    singleObjectIdArray[0] = obj;
                    referencedObjects = ContentBuildInterface.GetPlayerDependenciesForObjects(singleObjectIdArray, input.Target, input.TypeDB, DependencyType.ValidReferences);
                    List<ObjectIdentifier> referencedObjectList = new List<ObjectIdentifier>(referencedObjects);
                    implicitOutput.objectDependencyInfo.Add(new ObjectDependencyInfo()
                    {
                        Object = obj,
                        Dependencies = referencedObjectList
                    });
                    if (referencedObjectList.Count > 0)
                    {
                        assetResult.objectDependencyInfo.Add(new ObjectDependencyInfo()
                        {
                            Object = obj,
                            Dependencies = referencedObjectList
                        });
                    }

                    foreach (ObjectIdentifier o in referencedObjects)
                        objectLookingAt.Push(o);
                }
            }

            // We need to ensure that we have a reference to a visible representation so our runtime dependency appending process
            // can find something that can be appended, otherwise the necessary data will fail to load correctly in all cases. (EX: prefab A has reference to component on prefab B)
            foreach (var dependency in mainRepresentationNeeded)
            {
                // For each dependency, add just the main representation as a reference
                var representations = ContentBuildInterface.GetPlayerAssetRepresentations(dependency, input.Target);
                collectedImmediateReferences.Add(representations[0]);
            }
            collectedImmediateReferences.UnionWith(encounteredExplicitAssetDependencies);
            return collectedImmediateReferences.ToArray();
        }
    }
}
