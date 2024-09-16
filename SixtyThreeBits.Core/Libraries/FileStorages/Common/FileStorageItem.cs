using System;

namespace SixtyThreeBits.Core.Libraries.FileStorages.Common
{
    public record FileStorageItem
    (
        string Filename,
        long FilesizeBytes,
        DateTime FileDateCreated,
        DateTime FileDateCreatedUtc,
        DateTime FileDateUpdated,
        DateTime FileDateUpdatedUtc
    );
}
