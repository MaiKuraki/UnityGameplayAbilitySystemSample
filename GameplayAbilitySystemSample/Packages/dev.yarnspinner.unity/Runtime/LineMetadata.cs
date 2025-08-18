using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity
{
    [Serializable]
    public class LineMetadata
    {
        [Serializable]
        class StringDictionary : SerializedDictionary<string, string> { }

        [SerializeField]
        private StringDictionary _lineMetadata = new StringDictionary();

        internal LineMetadata(IEnumerable<LineMetadataTableEntry> lineMetadataTableEntries)
        {
            AddMetadata(lineMetadataTableEntries);
        }

        /// <summary>
        /// Adds any metadata if they are defined for each line. The metadata is internally
        /// stored as a single string with each piece of metadata separated by a single
        /// whitespace.
        /// </summary>
        /// <param name="lineMetadataTableEntries">IEnumerable with metadata entries.</param>
        internal void AddMetadata(IEnumerable<LineMetadataTableEntry> lineMetadataTableEntries)
        {
            foreach (var entry in lineMetadataTableEntries)
            {
                if (entry.Metadata.Length == 0)
                {
                    continue;
                }

                _lineMetadata.Add(entry.ID, String.Join(" ", entry.Metadata));
            }
        }

        /// <summary>
        /// Gets the line IDs that contain metadata.
        /// </summary>
        /// <returns>The line IDs.</returns>
        public IEnumerable<string> GetLineIDs()
        {
            // The object returned doesn't allow modifications and is kept in
            // sync with `_lineMetadata`.
            return _lineMetadata.Keys;
        }

        /// <summary>
        /// Returns metadata for a given line ID, if any is defined.
        /// </summary>
        /// <param name="lineID">The line ID.</param>
        /// <returns>An array of each piece of metadata if defined, otherwise returns null.</returns>
        public string[] GetMetadata(string lineID)
        {
            if (_lineMetadata.TryGetValue(lineID, out var result))
            {
                return result.Split(' ');
            }

            return null;
        }
    }
}
