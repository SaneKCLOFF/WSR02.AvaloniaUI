using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSR02.Kukhar.AvaloniaUI.Models.NotDbModels
{
    public class Plate
    {
        public string Title { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string ArticleNumber { get; set; } = null!;
        public decimal Cost { get; set; }
        public string Materials { get; set; } = null!;

        public Bitmap Image { get; set; } = null!;
        

        public Plate(string title, string? type,
            string articleNumber, decimal cost,
            string? materials, Bitmap image)
        {
            Title = title;
            Type = type;
            ArticleNumber = articleNumber;
            Cost = cost;
            Materials = materials;
            Image = image;
        }
        
    }
}
