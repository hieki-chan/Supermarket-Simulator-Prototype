using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Build.Content;

//TODO: Remove this when we make this interface public
[assembly: InternalsVisibleTo("Unity.Addressables.Editor", AllInternalsVisible = true)]
namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the dependency data container
    /// </summary>
    internal interface IObjectDependencyData : IContextObject
    {
        /// <summary>
        /// Dependencies of a given object
        /// </summary>
        Dictionary<ObjectIdentifier, List<ObjectIdentifier>> ObjectDependencyMap { get; }
    }
}
