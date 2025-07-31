using System.Drawing;

namespace Application.Services;

public static class FileConverterService
{
    public static string PlaceHolder = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw..."; // Puedes cambiar por otro base64

    public static string ConvertToBase64(Stream file, int w = 256)
    {
        if (file.Length > 0)
        {
            var ms = new MemoryStream();
            file.CopyTo(ms);
            ms = ResizeImage(ms, w);
            var fileBytes = ms.ToArray();
            return "data:image/png;base64," + Convert.ToBase64String(fileBytes);
        }
        else
        {
            throw new FileLoadException();
        }
    }

    public static MemoryStream ResizeImage(MemoryStream ms, int w)
    {
        Image img = Image.FromStream(ms);
        int h = Convert.ToInt32(w * img.Height / img.Width);
        Image imgN = img.GetThumbnailImage(w, h, null, IntPtr.Zero);
        MemoryStream res = new MemoryStream();
        imgN.Save(res, img.RawFormat);
        return res;
    }
}
