using System;

namespace SixtyThreeBits.Core.Libraries.FileStorages.DTO
{
    public record FileStorageItemDTO
    (
        string Filename,
        long? FilesizeBytes,
        DateTime? FileDateCreated,
        DateTime? FileDateCreatedUtc,
        DateTime? FileDateUpdated,
        DateTime? FileDateUpdatedUtc
    );
}
