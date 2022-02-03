﻿using System.Xml.Serialization;

namespace OpenConstructionSet.Mods
{
    /// <inheritdoc/>
    public class ModFile : IModFile
    {
        /// <summary>
        /// Creates a new <see cref="ModFile"/> instance from the provided path.
        /// </summary>
        /// <param name="path">The full path of the <see cref="ModFile"/>.</param>
        public ModFile(string path)
        {
            Path = path;
            Filename = System.IO.Path.GetFileName(path);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        /// <inheritdoc/>
        public string Filename { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        /// <inheritdoc/>
        public virtual async Task<ModFileData> ReadDataAsync(CancellationToken cancellationToken = default)
        {
            var buffer = await File.ReadAllBytesAsync(Path, cancellationToken).ConfigureAwait(false);

            using var reader = new OcsReader(buffer);

            var type = (DataFileType)reader.ReadInt();

            return new(reader.ReadHeader(), reader.ReadInt(), reader.ReadItems().ToList(), await ReadInfoAsync(cancellationToken).ConfigureAwait(false));
        }

        /// <inheritdoc/>
        public virtual Task<Header> ReadHeaderAsync(CancellationToken cancellationToken = default)
        {
            using var reader = new OcsReader(File.OpenRead(Path));

            return Task.FromResult((DataFileType)reader.ReadInt() == DataFileType.Mod ? reader.ReadHeader() : throw new InvalidDataException("Target file is not a valid mod"));
        }

        /// <inheritdoc/>
        public virtual Task<ModInfoData?> ReadInfoAsync(CancellationToken cancellationToken = default)
        {
            var infoPath = OcsPathHelper.GetInfoPath(Path);

            if (!File.Exists(infoPath))
            {
                return Task.FromResult<ModInfoData?>(null);
            }

            using var stream = File.OpenRead(infoPath);

            var serialiser = new XmlSerializer(typeof(ModInfoData));

            return Task.FromResult((ModInfoData?)serialiser.Deserialize(stream));
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Filename} ({Path})";

        /// <inheritdoc/>
        public virtual async Task WriteDataAsync(ModFileData data, CancellationToken cancellationToken = default)
        {
            using var writer = new OcsWriter(File.OpenWrite(Path));

            writer.Write((int)DataFileType.Mod);
            writer.Write(data.Header);
            writer.Write(data.LastId);
            writer.Write(data.Items);

            if (data.Info is not null)
            {
                await WriteInfoAsync(data.Info, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task WriteHeaderAsync(Header header, CancellationToken cancellationToken = default)
        {
            var data = await ReadDataAsync(cancellationToken).ConfigureAwait(false) with { Header = header };

            await WriteDataAsync(data, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual Task WriteInfoAsync(ModInfoData info, CancellationToken cancellationToken = default)
        {
            using var stream = File.OpenWrite(OcsPathHelper.GetInfoPath(Path));

            var serialiser = new XmlSerializer(typeof(ModInfoData));

            serialiser.Serialize(stream, info);

            return Task.CompletedTask;
        }
    }
}
