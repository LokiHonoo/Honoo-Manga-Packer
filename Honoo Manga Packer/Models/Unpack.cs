using Honoo.MangaPacker.ViewModels;
using Microsoft.VisualBasic.FileIO;
using PdfiumViewer;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace Honoo.MangaPacker.Models
{
    internal static class Unpack
    {
        private static readonly ExtractionOptions _extractionOptions = new()
        {
            Overwrite = true,
            ExtractFullPath = true
        };

        internal static UnpackResult Do(string path, UnpackSettings settings)
        {
            if (File.Exists(path))
            {
                string ext = Path.GetExtension(path).ToUpperInvariant()!;
                return ext switch
                {
                    ".ZIP" or ".RAR" or ".7Z" => TryDoZip(path, settings),
                    ".PDF" => TryDoPdf(path, settings),
                    _ => new UnpackResult(false, "不支持的文件格式。“" + path + "”", string.Empty),
                };
            }
            else
            {
                return new UnpackResult(false, "文件不存在。“" + path + "”", string.Empty);
            }
        }

        private static UnpackResult TryDoPdf(string path, UnpackSettings settings)
        {
            string title = Path.GetFileNameWithoutExtension(path);
            string dir = Path.Combine(settings.UnpackDir, title);
            int n = 1;
            while (Directory.Exists(dir))
            {
                dir = Path.Combine(settings.UnpackDir, $"{title} ({n})");
                n++;
            }
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                return new UnpackResult(false, "无法创建解包文件夹。“" + path + "”", string.Empty);
            }
            try
            {
                var pdf = PdfDocument.Load(path);
                IList<SizeF> pageSizes = pdf.PageSizes;
                for (int i = 0; i < pdf.PageCount; i++)
                {
                    if (UnpackWorkbench.Instance.Abort)
                    {
                        return new UnpackResult(false, "用户终止。", string.Empty);
                    }
                    string fileName = i.ToString(CultureInfo.InvariantCulture).PadLeft(6, '0') + ".jpg";
                    SizeF pageSize = pageSizes[i];
                    Image image = pdf.Render(i, (int)pageSize.Width, (int)pageSize.Height, 150, 150, PdfRenderFlags.Annotations);
                    image.Save(Path.Combine(dir, fileName), ImageFormat.Jpeg);
                }
            }
            catch
            {
                return new UnpackResult(false, "导出图像失败。“" + path + "”", string.Empty);
            }
            if (settings.UnpackDelSource)
            {
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            return new UnpackResult(true, "解包完成。“" + path + "”", dir);
        }

        private static UnpackResult TryDoZip(string path, UnpackSettings settings)
        {
            string title = Path.GetFileNameWithoutExtension(path);
            string dir = Path.Combine(settings.UnpackDir, title);
            int n = 1;
            while (Directory.Exists(dir))
            {
                dir = Path.Combine(settings.UnpackDir, $"{title} ({n})");
                n++;
            }
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                return new UnpackResult(false, "无法创建解包文件夹。“" + path + "”", string.Empty);
            }
            Encoding encoding = Encoding.GetEncoding(settings.UnpackEncoding);
            var readerOptions = new ReaderOptions()
            {
                ArchiveEncoding = new ArchiveEncoding(encoding, encoding),
                LookForHeader = true,
                Password = null
            };
            bool success = TryDoZip(path, dir, readerOptions);
            if (!success && settings.UnpackTryPassword)
            {
                foreach (var pwd in settings.UnpackPasswords)
                {
                    readerOptions.Password = pwd;
                    success = TryDoZip(path, dir, readerOptions);
                    if (success)
                    {
                        break;
                    }
                }
            }
            if (success)
            {
                if (settings.UnpackDelSource)
                {
                    FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                return new UnpackResult(true, "解包完成。“" + path + "”", dir);
            }
            else
            {
                return new UnpackResult(false, "解压缩失败。“" + path + "”", string.Empty);
            }
        }

        private static bool TryDoZip(string path, string destination, ReaderOptions options)
        {
            try
            {
                using IArchive archive = ArchiveFactory.Open(path, options);
                archive.WriteToDirectory(destination, _extractionOptions);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}