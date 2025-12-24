using Honoo.IO.Hashing;
using Microsoft.VisualBasic.FileIO;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honoo.MangaPacker.Models
{
    internal static class Pack
    {
        private static readonly HashSet<long> _adCrcs = [];
        private static readonly HashSet<long> _adLengths = [];
        private static readonly Crc _crc32 = Crc.Create(CrcName.CRC32);

        private static readonly WriterOptions _writerOptions = new(CompressionType.None)
        {
            ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8)
        };

        internal static HashSet<long> ADCrcs => _adCrcs;
        internal static HashSet<long> ADLengths => _adLengths;

        internal static PackResult Do(string path, PackSettings settings)
        {
            if (!Directory.Exists(path))
            {
                return new PackResult(false, "文件夹不存在。“" + path + "”");
            }
            string title = Path.GetFileName(path);
            int remove = path.Length;
            string dir = path;
            if (settings.PackRemoveNest)
            {
                string[] d = Directory.GetDirectories(dir);
                string[] f = Directory.GetFiles(dir);
                while (d.Length == 1 && f.Length == 0)
                {
                    dir = d[0];
                    remove = dir.Length;
                    string t = Path.GetFileName(dir);
                    if (t.Length > title.Length)
                    {
                        title = t;
                    }
                    d = Directory.GetDirectories(dir);
                    f = Directory.GetFiles(dir);
                }
            }
            var files = new List<FileInfo>(new DirectoryInfo(dir).GetFiles("*.*", System.IO.SearchOption.AllDirectories));
            if (files.Count > 0)
            {
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    string fileName = files[i].FullName;
                    if (fileName.EndsWith("Thumbs.db", StringComparison.OrdinalIgnoreCase)
                        || fileName.EndsWith("desktop.ini", StringComparison.OrdinalIgnoreCase)
                        || fileName.Contains("__MACOSX", StringComparison.OrdinalIgnoreCase)
                        || fileName.Contains(".DS_Store", StringComparison.OrdinalIgnoreCase))
                    {
                        files.RemoveAt(i);
                    }
                }
            }
            if (settings.PackRemoveAD && files.Count > 0)
            {
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    FileInfo file = files[i];
                    if (_adLengths.Contains(file.Length))
                    {
                        long crc = _crc32.ComputeFinal(File.ReadAllBytes(file.FullName)).ToUInt32();
                        if (_adCrcs.Contains(crc))
                        {
                            if (!settings.ADBackupDir.Exists)
                            {
                                settings.ADBackupDir.Create();
                            }
                            file.CopyTo(Path.Combine(settings.ADBackupDir.FullName, crc.ToString() + file.Extension), true);
                            files.RemoveAt(i);
                        }
                    }
                }
            }
            if (files.Count == 0)
            {
                return new PackResult(false, "文件夹中没有有效的文件。“" + path + "”");
            }
            string zip = Path.Combine(settings.PackDir, $"{title}.zip");
            int n = 1;
            while (File.Exists(zip))
            {
                zip = Path.Combine(settings.PackDir, $"{title} ({n}).zip");
                n++;
            }
            try
            {
                using var archive = ZipArchive.Create();
                foreach (var file in files)
                {
                    string key = file.FullName[remove..];
                    if (settings.PackAddNest)
                    {
                        key = $"{title}{key}";
                    }
                    var entry = archive.AddEntry(key, new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read), true);
                }
                archive.SaveTo(zip, _writerOptions);
            }
            catch
            {
                return new PackResult(false, "写入压缩包失败。“" + path + "”");
            }
            if (settings.PackDelSource)
            {
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            return new PackResult(true, "打包完成。“" + path + "”");
        }
    }
}