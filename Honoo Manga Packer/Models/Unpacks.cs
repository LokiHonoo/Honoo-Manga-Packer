using Microsoft.VisualBasic.FileIO;
using PdfiumViewer;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
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

        internal static bool Do(string path, Settings settings, out Tuple<bool, string, Exception?> log)
        {
            if (File.Exists(path))
            {
                string ext = Path.GetExtension(path).ToUpperInvariant()!;
                switch (ext)
                {
                    case ".ZIP": case ".RAR": case ".7Z": return TryDoZip(path, settings, out log);
                    case ".PDF": return TryDoPdf(path, settings, out log);
                    default:
                        log = new Tuple<bool, string, Exception?>(false, path, new IOException($"Unsupported file extension - \"{ext}\"."));
                        return false;
                }
            }
            log = new Tuple<bool, string, Exception?>(false, path, new FileNotFoundException("File not exists."));
            return false;
        }

        private static bool TryDoPdf(string path, Settings settings, out Tuple<bool, string, Exception?> log)
        {
            string title = Path.GetFileNameWithoutExtension(path);
            string dir = Path.Combine(settings.WorkDirectly, "Unpacks", title);
            int n = 1;
            while (Directory.Exists(dir))
            {
                dir = Path.Combine(settings.WorkDirectly, "Unpacks", $"{title} ({n})");
                n++;
            }
            try
            {
                Directory.CreateDirectory(dir);
                var pdf = PdfDocument.Load(path);
                IList<SizeF> pageSizes = pdf.PageSizes;
                for (int i = 0; i < pdf.PageCount; i++)
                {
                    string fileName = i.ToString(CultureInfo.InvariantCulture).PadLeft(6, '0') + ".jpg";
                    SizeF pageSize = pageSizes[i];
                    Image image = pdf.Render(i, (int)pageSize.Width, (int)pageSize.Height, 150, 150, PdfRenderFlags.Annotations);
                    image.Save(Path.Combine(dir, fileName), ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                log = new Tuple<bool, string, Exception?>(false, path, ex);
                return false;
            }
            if (settings.MoveToRecycleBin)
            {
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            log = new Tuple<bool, string, Exception?>(true, path, null);
            return true;
        }

        private static bool TryDoZip(string path, Settings settings, out Tuple<bool, string, Exception?> log)
        {
            string title = Path.GetFileNameWithoutExtension(path);
            string tmp = Path.Combine(settings.WorkDirectly, "Unpacks", Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tmp);
            }
            catch (Exception ex)
            {
                log = new Tuple<bool, string, Exception?>(false, path, ex);
                return false;
            }
            Encoding encoding = Encoding.GetEncoding(settings.UnpackEncoding);
            var readerOptions = new ReaderOptions()
            {
                ArchiveEncoding = new ArchiveEncoding(encoding, encoding),
                LookForHeader = true,
                Password = null
            };
            if (!TryDoZip(path, tmp, readerOptions, out Exception? exception))
            {
                bool extracted = false;
                foreach (var password in settings.Passwords.Keys)
                {
                    readerOptions.Password = password;
                    if (TryDoZip(path, tmp, readerOptions, out exception))
                    {
                        settings.Passwords[password]++;
                        extracted = true;
                        break;
                    }
                }
                if (!extracted)
                {
                    log = new Tuple<bool, string, Exception?>(false, path, exception);
                    return false;
                }
            }
            string deepDir = tmp;
            if (settings.ResetName)
            {
                string[] d = Directory.GetDirectories(deepDir);
                string[] f = Directory.GetFiles(deepDir);
                while (d.Length == 1 && f.Length == 0)
                {
                    deepDir = d[0];
                    string t = Path.GetFileName(deepDir);
                    if (t.Length > title.Length)
                    {
                        title = t;
                    }
                    d = Directory.GetDirectories(deepDir);
                    f = Directory.GetFiles(deepDir);
                }
            }
            string dir = Path.Combine(settings.WorkDirectly, "Unpacks", title);
            int n = 1;
            while (Directory.Exists(dir))
            {
                dir = Path.Combine(settings.WorkDirectly, "Unpacks", $"{title} ({n})");
                n++;
            }
            Directory.Move(deepDir, dir);
            if (Directory.Exists(tmp))
            {
                Directory.Delete(tmp, true);
            }
            string[] dels = Directory.GetFiles(dir, "Thumbs.db", System.IO.SearchOption.AllDirectories);
            if (dels.Length > 0)
            {
                foreach (string del in dels)
                {
                    File.Delete(del);
                }
            }
            dels = Directory.GetFiles(dir, "desktop.ini", System.IO.SearchOption.AllDirectories);
            if (dels.Length > 0)
            {
                foreach (string del in dels)
                {
                    File.Delete(del);
                }
            }
            if (settings.MoveToRecycleBin)
            {
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            log = new Tuple<bool, string, Exception?>(true, path, null);
            return true;
        }

        private static bool TryDoZip(string path, string dir, ReaderOptions readerOptions, out Exception? exception)
        {
            try
            {
                using (IArchive archive = ArchiveFactory.Open(path, readerOptions))
                {
                    archive.WriteToDirectory(dir, _extractionOptions);
                }
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}