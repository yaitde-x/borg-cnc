using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Borg.Machine;
using Utilities;
using Yuni.Query;

namespace Yuni.Library
{

    public interface ILibraryRepo
    {
        Task<IQueryable<RS274Meta>> AllItems();
        Task<Stream> Read(string id);
        Task<Stream> Write(string id);
        Task<Stream> Slice(string id, int row, int col);
        Task WriteAnalysis(string id, RS274Analysis analysis);
        Task<RS274Analysis> GetAnalysis(string id);

        Task WriteMeta(string id, RS274Meta meta);
        Task<RS274Meta> GetMeta(string id);

        Task<Stream> WriteSlice(string id, int row, int column);
        Task<string> GetSlicePath(string id, int row, int column);
    }

    public class FileLibraryRepo : ILibraryRepo
    {
        private readonly string _basePath;

        public FileLibraryRepo(string basePath)
        {
            _basePath = basePath;
        }

        private string GetItemPath(string relativePath)
        {
            return Path.Combine(_basePath, relativePath);
        }
        public async Task<Stream> Write(string id)
        {
            var item = (await AllItems()).FirstOrDefault(l => l.Id.EqualsIgnorecase(id));
            if (item == null)
                throw new LibraryException($"library item {id} does not exist");

            var itemPath = GetItemPath(item.Profiles[0].RelativePath);
            EnsurePath(Path.GetDirectoryName(itemPath));
            var stream = new FileStream(itemPath, FileMode.OpenOrCreate, FileAccess.Write);
            return stream;
        }

        public async Task<Stream> Read(string id)
        {
            var item = (await AllItems()).FirstOrDefault(l => l.Id.EqualsIgnorecase(id));
            if (item == null)
                throw new LibraryException($"library item {id} does not exist");

            var itemPath = GetItemPath(item.Profiles[0].RelativePath);
            EnsurePath(Path.GetDirectoryName(itemPath));

            var stream = new FileStream(itemPath, FileMode.Open, FileAccess.Read);
            return stream;
        }

        public async Task<Stream> Slice(string id, int row, int col)
        {
            var item = (await AllItems()).FirstOrDefault(l => l.Id.EqualsIgnorecase(id));
            if (item == null)
                throw new LibraryException($"library item {id} does not exist");

            var sliceFullPath = GetSlicePath(item, row, col);
            var stream = new FileStream(sliceFullPath, FileMode.Open, FileAccess.Read);
            return stream;
        }

        private string GetSlicePath(RS274Meta item, int row, int col)
        {
            var itemPath = GetItemPath(item.Profiles[0].RelativePath);
            EnsurePath(Path.GetDirectoryName(itemPath));

            var fileName = Path.GetFileNameWithoutExtension(itemPath);

            var slicePath = Path.GetDirectoryName(itemPath);
            var sliceName = string.Format(fileName + "-cell_r{0}_c{1}", row, col);
            var sliceExtension = Path.GetExtension(itemPath);
            var sliceFullPath = Path.Combine(slicePath, sliceName) + sliceExtension;
            return sliceFullPath;
        }

        public async Task<Stream> WriteSlice(string id, int row, int column)
        {
            var meta = await GetMeta(id);
            var filePath = Path.GetDirectoryName(meta.Profiles[0].RelativePath);
            var baseFileName = Path.GetFileNameWithoutExtension(meta.Profiles[0].RelativePath);
            var ext = Path.GetExtension(meta.Profiles[0].RelativePath);
            var sliceFullPath = Path.Combine(_basePath, filePath, $"{baseFileName}-cell_r{row}_c{column}{ext}");

            EnsurePath(Path.GetDirectoryName(sliceFullPath));
            var stream = new FileStream(sliceFullPath, FileMode.OpenOrCreate, FileAccess.Write);
            return stream;
        }

        public async Task<string> GetSlicePath(string id, int row, int column)
        {
            var item = (await AllItems()).FirstOrDefault(l => l.Id.EqualsIgnorecase(id));
            if (item == null)
                throw new LibraryException($"library item {id} does not exist");

            var sliceFullPath = GetSlicePath(item, row, column);
            return sliceFullPath;
        }

        public async Task WriteAnalysis(string id, RS274Analysis analysis)
        {
            var item = await GetMeta(id);
            var analysisPath = item.AnalysisFilePath();
            var fullPath = Path.Combine(_basePath, analysisPath);
            EnsurePath(Path.GetDirectoryName(fullPath));

            await File.WriteAllTextAsync(fullPath, analysis.ToJson());
        }

        public async Task<RS274Analysis> GetAnalysis(string id)
        {
            var item = await GetMeta(id);
            var analysisPath = item.AnalysisFilePath();
            var fullPath = Path.Combine(_basePath, analysisPath);
            EnsurePath(Path.GetDirectoryName(fullPath));

            var analysis = await File.ReadAllTextAsync(fullPath);
            return analysis.FromJson<RS274Analysis>();
        }

        private string BuildMetaPath(string id)
        {
            return Path.Combine(_basePath, id, "meta.json");
        }
        public async Task<RS274Meta> GetMeta(string id)
        {
            var metaPath = BuildMetaPath(id);
            if (!File.Exists(metaPath))
                throw new FileNotFoundException($"file {metaPath} does not exist");

            var meta = await File.ReadAllTextAsync(metaPath);
            return meta.FromJson<RS274Meta>();
        }

        public async Task WriteMeta(string id, RS274Meta meta)
        {
            var metaPath = BuildMetaPath(id);
            EnsurePath(Path.GetDirectoryName(metaPath));

            await File.WriteAllTextAsync(metaPath, meta.ToJson());
        }

        private void EnsurePath(string path)
        {
            Directory.CreateDirectory(path);
        }

        public async Task<IQueryable<RS274Meta>> AllItems()
        {
            var libraryMetafiles = WalkFolder(_basePath);
            var libraryItems = new List<RS274Meta>();

            foreach (var metaFile in libraryMetafiles)
            {
                var meta = await File.ReadAllTextAsync(metaFile);
                libraryItems.Add(meta.FromJson<RS274Meta>());
            }

            return libraryItems.AsQueryable();
        }

        public string[] WalkFolder(string baseFolder)
        {
            var files = Directory.GetFiles(baseFolder, "meta.json").ToList();
            var folders = Directory.GetDirectories(baseFolder);

            foreach (var folder in folders)
            {
                files.AddRange(WalkFolder(Path.Combine(baseFolder, folder)));
            }

            return files.ToArray();
        }
    }

    public class RS274Meta
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RS274MetaProfile[] Profiles { get; set; }

        public string AnalysisFilePath()
        {
            var profile = Profiles[0];
            var path = Path.GetDirectoryName(profile.RelativePath);
            var fileName = Path.GetFileNameWithoutExtension(profile.RelativePath);
            return Path.Combine(path, fileName) + ".analysis";
        }
    }

    public class RS274MetaProfile
    {
        public string Id { get; set; }
        public LibraryItemType ItemType { get; set; }
        public string Image { get; set; }
        public string ToolProfile { get; set; }
        public string SpindleProfile { get; set; }
        public string RelativePath { get; set; }
        public SliceData Slices { get; set; } = SliceData.Default;
    }

    public class SliceData
    {
        public static SliceData Default { get { return new SliceData(1, 1, new Cell[0]); } }

        public SliceData()
        {

        }
        public SliceData(int rows, int columns, Cell[] cells)
        {
            Rows = rows;
            Columns = columns;
            Cells = cells;
        }

        public Cell[] Cells { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }

    public enum LibraryItemType
    {
        RS274 = 1, RS274L = 2, Design = 3, Svg = 4
    }

    public class LibraryException : Exception
    {

        public LibraryException(string message) : base(message) { }

    }
}