using System;

namespace SixtyThreeBits.Core.Libraries.FileStorages.Core
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
