//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="Lifeprojects.de">
//     Class: MainWindow
//     Copyright © Lifeprojects.de yyyy
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>dd.MM.yyyy</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace XamlIconViewer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml;

    using XamlIconViewer.Converter;
    using XamlIconViewer.SVG;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            DrawingImage icon = (DrawingImage)FindResource("AppIcon2");
            WpfIconHelper.ApplyIcon(this, icon, 32);

            this.WindowTitel = "Xaml Icon Viewer";
            this.DataContext = this;
        }

        private string _WindowTitel;

        public string WindowTitel
        {
            get { return _WindowTitel; }
            set
            {
                if (this._WindowTitel != value)
                {
                    this._WindowTitel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<string> _ImageListSource;

        public IEnumerable<string> ImageListSource
        {
            get { return this._ImageListSource; }
            set
            {
                if (this._ImageListSource != value)
                {
                    this._ImageListSource = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<ListBox, SelectionChangedEventArgs>.AddHandler(this.lbFiles, "SelectionChanged", this.OnSelectionChanged);
            WeakEventManager<RadioButton, RoutedEventArgs>.AddHandler(this.rbCanvas, "Checked", this.OnCanvasChecked);
            WeakEventManager<RadioButton, RoutedEventArgs>.AddHandler(this.rbViewbox, "Checked", this.OnViewboxChecked);
            WeakEventManager<RadioButton, RoutedEventArgs>.AddHandler(this.rbDrawingImage, "Checked", this.OnDrawingImageChecked);
            WeakEventManager<RadioButton, RoutedEventArgs>.AddHandler(this.rbPathGeometry, "Checked", this.OnPathGeometryChecked);
            WeakEventManager<RadioButton, RoutedEventArgs>.AddHandler(this.rbSVG, "Checked", this.OnSVGChecked);
            this.rbCanvas.IsChecked = true;
        }

        private void OnCanvasChecked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
            string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\Canvas\\");
            this.ImageListSource = null;
            this.ImageListSource = Directory.EnumerateFiles(imagePath, "*.xaml");
            this.lbFiles.Focus();
            this.lbFiles.SelectedIndex = 0;
            this.chkKonvertieren.IsEnabled = false;
        }

        private void OnViewboxChecked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
            string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\Viewbox\\");
            this.ImageListSource = null;
            this.ImageListSource = Directory.EnumerateFiles(imagePath, "*.xaml");
            this.lbFiles.Focus();
            this.lbFiles.SelectedIndex = 0;
            this.chkKonvertieren.IsEnabled = true;
        }

        private void OnDrawingImageChecked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
            string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\DrawingImage\\");
            this.ImageListSource = null;
            this.ImageListSource = Directory.EnumerateFiles(imagePath, "*.xaml");
            this.lbFiles.Focus();
            this.lbFiles.SelectedIndex = 0;
            this.chkKonvertieren.IsEnabled = false;
        }

        private void OnPathGeometryChecked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
            string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\PathGeometry\\");
            this.ImageListSource = null;
            this.ImageListSource = Directory.EnumerateFiles(imagePath, "*.xaml");
            this.lbFiles.Focus();
            this.lbFiles.SelectedIndex = 0;
            this.chkKonvertieren.IsEnabled = false;
        }

        private void OnSVGChecked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
            string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\SVG\\");
            this.ImageListSource = null;
            this.ImageListSource = Directory.EnumerateFiles(imagePath, "*.svg");
            this.lbFiles.Focus();
            this.lbFiles.SelectedIndex = 0;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                this.ImagesView.Source = null;
                return;
            }

            string file = e.AddedItems[0].ToString();
            if (file.Contains("canvas", StringComparison.OrdinalIgnoreCase) == true)
            {
                var canvasDict = CreateCanvasDictionary(this.ImageListSource);
                Canvas canvasObject = canvasDict[Path.GetFileName(file)];
                this.ImagesView.Source = ConvertCanvasToBitmap(canvasObject);
            }
            else if (file.Contains("viewbox", StringComparison.OrdinalIgnoreCase) == true)
            {
                var viewboxDict = CreateViewboxDictionary(this.ImageListSource);
                if (this.chkKonvertieren.IsChecked == true)
                {
                    DirectoryInfo di = new DirectoryInfo(CurrentAssemblyPath());
                    string imagePath = Path.Combine(di.Parent.Parent.Parent.FullName, "XamlIcon\\DrawingImage\\", $"OutVB_{Path.GetFileNameWithoutExtension(file)}.xaml");
                    ViewboxToDrawingImageXamlConverter.CreateDrawingImageXamlFromViewboxXaml(file, imagePath);
                }

                Viewbox viewboxObject = viewboxDict[Path.GetFileName(file)];
                this.ImagesView.Source = ConvertViewboxToBitmap(viewboxObject);
            }
            else if (file.Contains("DrawingImage", StringComparison.OrdinalIgnoreCase) == true)
            {
                var drawingDict = CreateDrawingImageDictionary(this.ImageListSource);
                DrawingImage viewObject = drawingDict[Path.GetFileName(file)];
                GeometryDrawing drawingGemetry = viewObject.Drawing as GeometryDrawing;
                DrawingGroup drawingGroup = viewObject.Drawing as DrawingGroup;

                int iconSize = 64;
                string sizeText = Regex.Match(file, @"\d+").Value;
                if (string.IsNullOrEmpty(sizeText) == false)
                {
                    iconSize = Convert.ToInt32(Regex.Match(file, @"\d+").Value, CultureInfo.CurrentCulture);
                    if (iconSize <= 16)
                    {
                        iconSize = 64;
                    }
                }

                if (drawingGroup != null)
                {
                    this.ImagesView.Source = ConvertDrawingGroupToBitmap(drawingGroup, iconSize, iconSize);
                }
                else
                {
                    this.ImagesView.Source = ConvertDrawingGroupToBitmap(drawingGemetry, iconSize, iconSize);
                }
            }
            else if (file.Contains("PathGeometry", StringComparison.OrdinalIgnoreCase) == true)
            {
                var drawingDict = CreatePathGeometryDictionary(this.ImageListSource);
                PathGeometry viewObject = drawingDict[Path.GetFileName(file)];
                this.ImagesView.Source = ConvertPathGeometryToBitmap(viewObject, 64, 64);
            }
            else if (file.Contains("SVG", StringComparison.OrdinalIgnoreCase) == true)
            {
                var drawingDict = CreateSvgDictionary(this.ImageListSource);
                DrawingImage viewObject = drawingDict[Path.GetFileName(file)];
                DrawingGroup drawingGroup = viewObject.Drawing as DrawingGroup;
                this.ImagesView.Source = ConvertSvgToBitmap(drawingGroup);
            }
        }

        #region Lese und Konvertiere Canvas XAML Icon aus einem XAML File
        private static Dictionary<string, Canvas> CreateCanvasDictionary(IEnumerable<string> xamlIconList)
        {
            if (xamlIconList == null)
            {
                return null;
            }

            Dictionary<string, Canvas> resourceNamesDict = new Dictionary<string, Canvas>();

            try
            {
                foreach (string file in xamlIconList)
                {
                    Canvas content = ConvertXamlFileToCanvas(file);
                    resourceNamesDict.Add(Path.GetFileName(file), content);
                }

                return resourceNamesDict;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Canvas ConvertXamlFileToCanvas(string filePath)
        {
            Canvas result = null;
            using (TextReader sr = new StringReader(File.ReadAllText(filePath)))
            {
                XmlReader xmlReader = XmlReader.Create(sr);
                UIElement uiElement = (Canvas)XamlReader.Load(xmlReader) as UIElement;
                result = uiElement as Canvas;
            }

            return result;
        }

        private static RenderTargetBitmap ConvertCanvasToBitmap(Canvas canvas)
        {
            if (canvas == null)
            {
                return null;
            }

            int canvasWidth = 64; // (int)canvas.Width;
            int canvasHeight = 64; // (int)canvas.Height;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(canvasWidth, canvasHeight, 96d, 96d, PixelFormats.Pbgra32);

            // needed otherwise the image output is black
            canvas.Measure(new Size(canvasWidth, canvasHeight));
            canvas.Arrange(new Rect(new Size(canvasWidth, canvasHeight)));

            renderBitmap.Render(canvas);

            /*
            // for jpg bitmap
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            // for png bitmap
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream fs = File.Create(@"c:\temp\test.png"))
            {
                encoder.Save(fs);
            }
            */

            return renderBitmap;
        }
        #endregion Lese und Konvertiere Canvas XAML Icon aus einem XAML File

        #region Lese und Konvertiere Viewbox XAML Icon aus einem XAML File
        private static Dictionary<string, Viewbox> CreateViewboxDictionary(IEnumerable<string> xamlIconList)
        {
            if (xamlIconList == null)
            {
                return null;
            }

            Dictionary<string, Viewbox> resourceNamesDict = new Dictionary<string, Viewbox>();

            try
            {
                foreach (string file in xamlIconList)
                {
                    Viewbox content = ConvertXamlFileToViewbox(file);
                    resourceNamesDict.Add(Path.GetFileName(file), content);
                }

                return resourceNamesDict;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Viewbox ConvertXamlFileToViewbox(string filePath)
        {
            Viewbox result = null;
            using (TextReader sr = new StringReader(File.ReadAllText(filePath)))
            {
                XmlReader xmlReader = XmlReader.Create(sr);
                Viewbox uiElement = (Viewbox)XamlReader.Load(xmlReader) as Viewbox;
                result = uiElement as Viewbox;
            }

            return result;
        }

        private static RenderTargetBitmap ConvertViewboxToBitmap(FrameworkElement target)
        {
            if (target == null)
            {
                return null;
            }

            int canvasWidth = 64;
            int canvasHeight = 64;

            target.Measure(new Size(canvasWidth, canvasHeight));
            target.Arrange(new Rect(0, 0, canvasWidth, canvasHeight));
            target.UpdateLayout();

            var renderBitmap = new RenderTargetBitmap(canvasWidth, canvasHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(target);

            /*
            // for jpg bitmap
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            // for png bitmap
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream fs = File.Create(@"c:\temp\test.png"))
            {
                encoder.Save(fs);
            }
            */

            return renderBitmap;
        }
        #endregion Lese und Konvertiere Viewbox XAML Icon aus einem XAML File

        #region Lese und Konvertiere DrawingImage XAML Icon aus einem XAML File
        private static Dictionary<string, DrawingImage> CreateDrawingImageDictionary(IEnumerable<string> xamlIconList)
        {
            if (xamlIconList == null)
            {
                return null;
            }

            Dictionary<string, DrawingImage> resourceNamesDict = new Dictionary<string, DrawingImage>();

            try
            {
                foreach (string file in xamlIconList)
                {
                    DrawingImage content = ConvertXamlFileToDrawingImage(file);
                    resourceNamesDict.Add(Path.GetFileName(file), content);
                }

                return resourceNamesDict;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DrawingImage ConvertXamlFileToDrawingImage(string filePath)
        {
            DrawingImage result = null;
            using (TextReader sr = new StringReader(File.ReadAllText(filePath)))
            {
                XmlReader xmlReader = XmlReader.Create(sr);
                var xamlContent = XamlReader.Load(xmlReader);
                var drawingImageRes = ((System.Windows.ResourceDictionary)xamlContent).Values;
                result = drawingImageRes.Cast<DrawingImage>().First();
            }

            return result;
        }

        private static RenderTargetBitmap ConvertDrawingImageToBitmap(DrawingImage drawingImage)
        {
            if (drawingImage == null)
            {
                return null;
            }

            int width = 64;
            int height = 64;

            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                context.DrawDrawing(drawingImage.Drawing);
            }

            var rtb = new RenderTargetBitmap(
                width,       // Pixelbreite
                height,      // Pixelhöhe
                96,          // DPI X
                96,          // DPI Y
                PixelFormats.Pbgra32);

            rtb.Render(drawingVisual);

            /*
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var stream = new FileStream("output.png", FileMode.Create))
            {
                encoder.Save(stream);
            }
            */

            return rtb;
        }

        public static RenderTargetBitmap ConvertDrawingGroupToBitmap(GeometryDrawing drawingGroup, int pixelWidth, int pixelHeight)
        {
            // Optional: Bounding Box des Drawings bestimmen
            Rect contentBounds = drawingGroup.Bounds;

            // Skalierung, damit das DrawingGroup in die gewünschte Bitmapgröße passt
            double scaleX = pixelWidth / contentBounds.Width;
            double scaleY = pixelHeight / contentBounds.Height;
            double scale = Math.Min(scaleX, scaleY); // Uniform Scaling

            // Transform anwenden (Skalierung + Verschiebung zur Zentrierung)
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            transformGroup.Children.Add(new TranslateTransform(-contentBounds.X * scale, -contentBounds.Y * scale));

            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                context.PushTransform(transformGroup);
                context.DrawDrawing(drawingGroup);
            }

            var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            return rtb;
        }

        public static RenderTargetBitmap ConvertDrawingGroupToBitmap(DrawingGroup drawingGroup, int pixelWidth, int pixelHeight)
        {
            // Optional: Bounding Box des Drawings bestimmen
            Rect contentBounds = drawingGroup.Bounds;

            // Skalierung, damit das DrawingGroup in die gewünschte Bitmapgröße passt
            double scaleX = pixelWidth / contentBounds.Width;
            double scaleY = pixelHeight / contentBounds.Height;
            double scale = Math.Min(scaleX, scaleY); // Uniform Scaling

            // Transform anwenden (Skalierung + Verschiebung zur Zentrierung)
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            transformGroup.Children.Add(new TranslateTransform(-contentBounds.X * scale, -contentBounds.Y * scale));

            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                context.PushTransform(transformGroup);
                context.DrawDrawing(drawingGroup);
            }

            var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            return rtb;
        }

        #endregion Lese und Konvertiere DrawingImage XAML Icon aus einem XAML File

        #region Lese und Konvertiere PathGeometry XAML Icon aus einem XAML File
        private static Dictionary<string, PathGeometry> CreatePathGeometryDictionary(IEnumerable<string> xamlIconList)
        {
            if (xamlIconList == null)
            {
                return null;
            }

            Dictionary<string, PathGeometry> resourceNamesDict = new Dictionary<string, PathGeometry>();

            try
            {
                foreach (string file in xamlIconList)
                {
                    PathGeometry content = ConvertXamlFileToPathGeometry(file);
                    resourceNamesDict.Add(Path.GetFileName(file), content);
                }

                return resourceNamesDict;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static PathGeometry ConvertXamlFileToPathGeometry(string filePath)
        {
            PathGeometry result = null;
            using (TextReader sr = new StringReader(File.ReadAllText(filePath)))
            {
                XmlReader xmlReader = XmlReader.Create(sr);
                var xamlContent = XamlReader.Load(xmlReader);
                var imageRes = ((System.Windows.ResourceDictionary)xamlContent).Values;
                result = imageRes.Cast<PathGeometry>().First();
            }

            return result;
        }

        public static RenderTargetBitmap ConvertPathGeometryToBitmap(PathGeometry path, int width, int height, Brush stroke = null, Brush fill = null, double strokeThickness = 1.0)
        {
            stroke ??= Brushes.Green;
            fill ??= Brushes.Blue;

            // Geometrie-Grenzen ermitteln
            Rect bounds = path.Bounds;

            // Skalierung berechnen (uniform fit)
            double scaleX = width / bounds.Width;
            double scaleY = height / bounds.Height;
            double scale = Math.Min(scaleX, scaleY); // Für UniformFit

            // Offset zur Zentrierung nach Skalierung
            double offsetX = (width - bounds.Width * scale) / 2;
            double offsetY = (height - bounds.Height * scale) / 2;

            // Visual zeichnen mit Skalierung
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                // Transformationskette: Skalieren und Verschieben (Bounds korrigieren)
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform(scale, scale));
                transform.Children.Add(new TranslateTransform(-bounds.X * scale + offsetX, -bounds.Y * scale + offsetY));

                context.PushTransform(transform);
                context.DrawGeometry(fill, new Pen(stroke, strokeThickness / scale), path); // Pen skaliert mit
            }

            // Bitmap erzeugen
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);

            /*
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var stream = new FileStream("output.png", FileMode.Create))
            {
                encoder.Save(stream);
            }
            */

            return rtb;
        }

        #endregion Lese und Konvertiere PathGeometry XAML Icon aus einem XAML File

        #region Lese und Konvertiere PathGeometry XAML Icon aus einem Svg File
        private static Dictionary<string, DrawingImage> CreateSvgDictionary(IEnumerable<string> svgIconList)
        {
            if (svgIconList == null)
            {
                return null;
            }

            Dictionary<string, DrawingImage> resourceNamesDict = new Dictionary<string, DrawingImage>();

            try
            {
                foreach (string file in svgIconList)
                {
                    DrawingImage content = ConvertSvgFileToDrawingImage(file);
                    resourceNamesDict.Add(Path.GetFileName(file), content);
                }

                return resourceNamesDict;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DrawingImage ConvertSvgFileToDrawingImage(string filePath)
        {
            DrawingImage result = null;

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                result = SvgReader.Load(stream);
            }

            return result;
        }

        public static RenderTargetBitmap ConvertSvgToBitmap(DrawingGroup drawingGroup, int pixelWidth = 150, int pixelHeight = 150)
        {
            // Optional: Bounding Box des Drawings bestimmen
            Rect contentBounds = drawingGroup.Bounds;

            // Skalierung, damit das DrawingGroup in die gewünschte Bitmapgröße passt
            double scaleX = pixelWidth / contentBounds.Width;
            double scaleY = pixelHeight / contentBounds.Height;
            double scale = Math.Min(scaleX, scaleY); // Uniform Scaling

            // Transform anwenden (Skalierung + Verschiebung zur Zentrierung)
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            transformGroup.Children.Add(new TranslateTransform(-contentBounds.X * scale, -contentBounds.Y * scale));

            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                context.PushTransform(transformGroup);
                context.DrawDrawing(drawingGroup);
            }

            var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            return rtb;
        }
        #endregion Lese und Konvertiere PathGeometry XAML Icon aus einem Svg File

        private static string CurrentAssemblyPath()
        {
            string result = string.Empty;

            result = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            return result;
        }

        #region INotifyPropertyChanged implementierung
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
        #endregion INotifyPropertyChanged implementierung
    }

    public static class WpfIconHelper
    {
        public static ImageSource CreateIcon(DrawingImage drawingImage, int size = 64, double dpi = 96)
        {
            if (size.In(32,64) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Der Wert für die Icon Größe muß 32 oder 64 Pixel sein.");
            }

            DrawingVisual visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawImage(drawingImage, new Rect(0, 0, size, size));
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(size, size, dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            bitmap.Freeze(); // Performance + Thread-Safety

            return bitmap;
        }

        public static void ApplyIcon(Window window, DrawingImage drawingImage, int size = 64)
        {
            window.Icon = CreateIcon(drawingImage, size);
        }
    }

    public static class IntegerExtensions
    {
        // Die 'this'-Anweisung vor 'int value' definiert den Typ, der erweitert wird
        public static bool In(this int value, params int[] allowedValues)
        {
            // Prüft, ob 'value' in der Menge 'allowedValues' enthalten ist
            return allowedValues.Contains(value);
        }

        public static bool NotIn(this int value, params int[] allowedValues)
        {
            // Prüft, ob 'value' in der Menge 'allowedValues' nicht enthalten ist
            return !allowedValues.Contains(value);
        }
    }
}