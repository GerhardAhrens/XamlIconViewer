namespace XamlIconViewer.Converter
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Xml;

    using XamlIconViewer.Extension;

    public static class ViewboxToDrawingImageXamlConverter
    {
        public static void CreateDrawingImageXamlFromViewboxXaml(string xamlFileIn, string xamlFileOut)
        {
            string keyName = System.IO.Path.GetFileNameWithoutExtension(xamlFileIn);
            string inContent = System.IO.File.ReadAllText(xamlFileIn);

            IEnumerable<string> pathSource = inContent.ExtractFromString("<Path"," />");
            if (pathSource == null)
            {
                return;
            }

            if (inContent.Contains("<Path.Fill>") == true)
            {
                MessageBox.Show("ViewBox mit '<Path.Fill>' können nicht konvertiert werden!", "VieBox konvertieren",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            StringBuilder outXaml = new StringBuilder();
            outXaml.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
            outXaml.AppendLine(CultureInfo.CurrentCulture,$"\t<DrawingImage x:Key=\"{keyName}\">");
            outXaml.AppendLine("\t\t<DrawingImage.Drawing>");
            outXaml.AppendLine("\t\t\t<DrawingGroup>");
            outXaml.AppendLine("\t\t\t\t<DrawingGroup.Children>");
            foreach (string pathText in pathSource)
            {
                string geometryDrawing = pathText.Replace("Fill", "Brush").Replace("Data", "Geometry");
                outXaml.AppendLine(CultureInfo.CurrentCulture,$"<GeometryDrawing\r\n{geometryDrawing}\r\nPen=\"{{x:Null}}\" />");
            }

            outXaml.AppendLine("\t\t\t\t</DrawingGroup.Children>");
            outXaml.AppendLine("\t\t\t</DrawingGroup>");
            outXaml.AppendLine("\t\t</DrawingImage.Drawing>");
            outXaml.AppendLine("\t</DrawingImage>");
            outXaml.AppendLine("</ResourceDictionary>");

            System.IO.File.WriteAllText(xamlFileOut, outXaml.ToString());
        }

        public static string ConvertViewboxXamlToDrawingImageXaml(string viewboxXaml)
        {
            // Schritt 1: Viewbox laden
            var stringReader = new System.IO.StringReader(viewboxXaml);
            var xmlReader = XmlReader.Create(stringReader);
            var root = (FrameworkElement)XamlReader.Load(xmlReader);

            // Schritt 2: Path-Elemente finden und zu GeometryDrawing umwandeln
            DrawingGroup drawingGroup = new DrawingGroup();
            foreach (var path in FindPathsRecursively(root))
            {
                GeometryDrawing drawing = new GeometryDrawing
                {
                    Geometry = path.Data,
                    Brush = path.Fill,
                    Pen = CreatePen(path)
                };

                drawingGroup.Children.Add(drawing);
            }

            // Schritt 3: DrawingImage erstellen
            DrawingImage drawingImage = new DrawingImage(drawingGroup);

            // Schritt 4: DrawingImage als XAML serialisieren
            return XamlWriter.Save(drawingImage);
        }

        private static Pen CreatePen(Path path)
        {
            if (path.Stroke != null && path.StrokeThickness > 0)
            {
                return new Pen(path.Stroke, path.StrokeThickness);
            }

            return null;
        }

        private static IEnumerable<Path> FindPathsRecursively(DependencyObject parent)
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current is Path path)
                    yield return path;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    queue.Enqueue(child);
                }

                // Für logische Kinder, die nicht in der VisualTree sind
                if (current is System.Windows.Controls.Panel panel)
                {
                    foreach (var child in panel.Children)
                        if (child is DependencyObject depChild)
                            queue.Enqueue(depChild);
                }
                else if (current is System.Windows.Controls.Decorator decorator && decorator.Child is DependencyObject decChild)
                {
                    queue.Enqueue(decChild);
                }
                else if (current is System.Windows.Controls.ContentControl contentControl && contentControl.Content is DependencyObject contChild)
                {
                    queue.Enqueue(contChild);
                }
            }
        }
    }
}
