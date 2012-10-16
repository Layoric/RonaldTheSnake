using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml.Linq;
using System.IO;

namespace RonaldTheSnakeContentProcessing
{
    [ContentImporter(".xml", DisplayName="Ronald XML Level menu importer", DefaultProcessor="LevelMenuContentProcessor")]
    public class LevelMenuContentImporter : ContentImporter<XDocument>
    {

        public override XDocument Import(string filename, ContentImporterContext context)
        {
            XDocument doc = XDocument.Load(filename);
            doc.Document.Root.Add(new XElement("File",
                new XAttribute("name", Path.GetFileName(filename)),
                new XAttribute("path", Path.GetDirectoryName(filename))));

            return doc;
        }
    }
}
