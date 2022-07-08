using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInvaders
{
    public static class TextureManager
    {
        private static Dictionary<string, Bitmap> _registry = new Dictionary<string, Bitmap>();
        private static Bitmap ERROR = new Bitmap(64, 64);

        static TextureManager()
        {
            using(var g = Graphics.FromImage(ERROR))
            {
                g.FillRectangle(Brushes.Magenta, 0, 0, 32, 32);
                g.FillRectangle(Brushes.Magenta, 32, 32, 32, 32);

                g.FillRectangle(Brushes.DarkGray, 32, 0, 32, 32);
                g.FillRectangle(Brushes.DarkGray, 0, 32, 32, 32);
            }
        }

        public static Bitmap Get(string id)
        {
            if (_registry.TryGetValue(id, out var img))
                return img;

            var files = new[] { $"assets/textures/{id}.png", $"assets/textures/{id}.jpg", $"assets/textures/{id}.jpeg" };

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        img = (Bitmap)Image.FromFile($"assets/textures/{id}.png");

                        _registry.Add(id, img);

                        return img;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }

            return ERROR;
        }
    }
}
