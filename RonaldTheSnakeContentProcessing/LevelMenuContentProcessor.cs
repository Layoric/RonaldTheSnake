using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml.Linq;
using RonandTheSnake.CustomDataTypes;

namespace RonaldTheSnakeContentProcessing
{
    [ContentProcessor(DisplayName="Ronald LevelMenu Content Processor")]
    public class LevelMenuContentProcessor : ContentProcessor<XDocument,LevelMenu[]>
    {
        public LevelMenuContentProcessor()
        {

        }
        public override LevelMenu[] Process(XDocument input, ContentProcessorContext context)
        {
            List<LevelMenu> result = new List<LevelMenu>();

            var allItems = input.Descendants().Where(x => x.Name == "Item");

            foreach (var item in allItems)
            {
                var mapFileNameNode = item.Descendants().Where(x => x.Name == "MapFileName").FirstOrDefault();
                if (mapFileNameNode == null)
                    throw new ArgumentNullException("MapFileName");

                var menuNameNode = item.Descendants().Where(x => x.Name == "MenuName").FirstOrDefault();
                if (menuNameNode == null)
                    throw new ArgumentNullException("MenuName");

                var menuImageFileNode = item.Descendants().Where(x => x.Name == "MenuImageFile").FirstOrDefault();
                if (menuImageFileNode == null)
                    throw new ArgumentNullException("MenuImageFile");

                result.Add(new LevelMenu() { MapFileName = mapFileNameNode.Value, MenuImageFile = menuImageFileNode.Value, MenuName = menuNameNode.Value });

            }

            return result.ToArray();
        }
    }
}
