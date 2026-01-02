using Honoo.IO.Hashing;
using Honoo.MangaPacker.ViewModels;
using Microsoft.VisualBasic.FileIO;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Honoo.MangaPacker.Models
{
    internal static class Pack
    {
        private static readonly HashSet<long> _adCrcs = [];
        private static readonly HashSet<long> _adLengths = [];
        private static readonly Crc _crc32 = Crc.Create(CrcName.CRC32);

        private static readonly WebpEncoder _webpEncoder = new()
        {
            FileFormat = WebpFileFormatType.Lossy,
            Quality = 100,
            Method = WebpEncodingMethod.Default
        };

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
            var files = new List<FileInfo>(new DirectoryInfo(dir).GetFiles("*.*", System.IO.SearchOption.AllDirectories));
            var list = new List<string[]>();
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
                    else
                    {
                        list.Add(fileName[remove..].Trim('\\').Split('\\'));
                    }
                }
                if (settings.PackRemoveAD && files.Count > 0)
                {
                    if (!Directory.Exists(settings.ADBackupDir))
                    {
                        Directory.CreateDirectory(settings.ADBackupDir);
                    }
                    for (int i = files.Count - 1; i >= 0; i--)
                    {
                        FileInfo file = files[i];
                        if (_adLengths.Contains(file.Length))
                        {
                            long crc = _crc32.ComputeFinal(File.ReadAllBytes(file.FullName)).ToUInt32();
                            if (_adCrcs.Contains(crc))
                            {
                                file.CopyTo(Path.Combine(settings.ADBackupDir, crc.ToString() + file.Extension), true);
                                files.RemoveAt(i);
                                list.RemoveAt(i);
                            }
                        }
                    }
                }
                if (settings.PackRemoveNest && files.Count > 0)
                {
                    int index = 0;
                    while (true)
                    {
                        var t = new HashSet<string>();
                        bool broked = false;
                        foreach (var l in list)
                        {
                            if (l.Length > index + 1)
                            {
                                t.Add(l[index]);
                            }
                            else
                            {
                                broked = true;
                                break;
                            }
                        }
                        index++;
                        if (!broked)
                        {
                            if (t.Count == 1)
                            {
                                string nt = t.First();
                                if (nt.Length > title.Length)
                                {
                                    title = nt;
                                }
                                remove += 1 + nt.Length;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (PackWorkbench.Instance.Abort)
            {
                return new PackResult(false, "用户终止。");
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
            if (settings.PackConvertToWebP)
            {
                string tmpDir = Path.Combine(settings.PackDir, Path.GetRandomFileName());
                try
                {
                    Directory.CreateDirectory(tmpDir);
                }
                catch
                {
                    return new PackResult(false, "无法创建 WebP 暂存工作目录。“" + tmpDir + "”");
                }
                try
                {
                    using var archive = ZipArchive.Create();
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (PackWorkbench.Instance.Abort)
                        {
                            return new PackResult(false, "用户终止。");
                        }
                        var file = files[i];
                        string key = file.FullName[remove..];
                        if (settings.PackAddNest)
                        {
                            key = $"{title}{key}";
                        }
                        if (file.FullName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                            || file.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            || file.FullName.Contains(".jpg", StringComparison.OrdinalIgnoreCase)
                            || file.FullName.Contains(".png", StringComparison.OrdinalIgnoreCase)
                            || file.FullName.Contains(".tga", StringComparison.OrdinalIgnoreCase))
                        {
                            string tmpFile = Path.Combine(tmpDir, i.ToString().PadLeft(7, '0') + Path.GetRandomFileName());
                            using (var image = Image.Load(file.FullName))
                            {
                                image.SaveAsWebp(tmpFile, _webpEncoder);
                            }
                            key = Path.ChangeExtension(key, ".webp");
                            archive.AddEntry(key, tmpFile);
                        }
                        else
                        {
                            archive.AddEntry(key, file);
                        }
                    }
                    archive.SaveTo(zip, _writerOptions);
                }
                catch
                {
                    return new PackResult(false, "写入压缩包失败。“" + path + "”");
                }
                Thread.Sleep(2000);
                try
                {
                    Directory.Delete(tmpDir, true);
                }
                catch
                {
                    return new PackResult(false, "无法删除 WebP 暂存工作目录。“" + tmpDir + "”");
                }
            }
            else
            {
                try
                {
                    using var archive = ZipArchive.Create();
                    foreach (var file in files)
                    {
                        if (PackWorkbench.Instance.Abort)
                        {
                            return new PackResult(false, "用户终止。");
                        }
                        string key = file.FullName[remove..];
                        if (settings.PackAddNest)
                        {
                            key = $"{title}{key}";
                        }
                        archive.AddEntry(key, file);
                    }
                    archive.SaveTo(zip, _writerOptions);
                }
                catch
                {
                    return new PackResult(false, "写入压缩包失败。“" + path + "”");
                }
            }

            if (settings.PackDelSource)
            {
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            return new PackResult(true, "打包完成。“" + path + "”");
        }
    }
}