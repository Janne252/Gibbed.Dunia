/* Copyright (c) 2021 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using Gibbed.Dunia.FileFormats;
using System.Collections.Generic;
using System.Linq;
using Big = Gibbed.Dunia.FileFormats.Big;
using System.Threading;

namespace Gibbed.Dunia.Packing
{
    internal sealed class UnpackContext<TArchive, THash>
        where TArchive : Big.IArchive<THash>, new()
    {
        private long _ProcessedEntryCount = 0;

        public List<(string archiveName, long totalCount, long extractedCount, long IgnoredCount, long ExcludedCount, long ExistingCount)> Tallies { get; private set; } = new();
        public readonly long TotalEntryCount;
        public readonly string OutputPath;
        public readonly ProjectData.HashList<THash> Hashes;

        public UnpackContext(
            ProjectData.Project project,
            BoundArchive<TArchive, THash>[] archives,
            string outputPath,
            Big.TryGetHashOverride<THash> tryGetHashOverride)
        {
            this.TotalEntryCount = archives.Sum(archive => archive.Fat.Entries.Count);
            this.OutputPath = outputPath;
            project.LoadListsFileNames(
                s => archives.First().Fat.ComputeNameHash(s, tryGetHashOverride), out Hashes);
        }

        public long ProcessedEntryCount => _ProcessedEntryCount;

        public void Tally(string archiveName, long total, long extracted, long ignored, long excluded, long existing)
        {
            this.Tallies.Add((archiveName, total, extracted, ignored, excluded, existing));
        }

        public void IncrementProcessedEntryCount()
        {
            Interlocked.Increment(ref this._ProcessedEntryCount);
        }
    }
}
