using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using RonandTheSnake.CustomDataTypes;

namespace RonaldTheSnakeContentProcessing
{
    [ContentTypeWriter]
    public class LevelMenuContentWriter : ContentTypeWriter<LevelMenu[]>
    {
        protected override void Write(ContentWriter output, LevelMenu[] value)
        {
            output.Write(value.Length);
            foreach (var level in value)
            {
                output.Write(level.MapFileName);
                output.Write(level.MenuImageFile);
                output.Write(level.MenuName);
            }
        }

        public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return typeof(LevelMenuReader).AssemblyQualifiedName;
        }

        public override string GetRuntimeType(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return typeof(LevelMenu[]).AssemblyQualifiedName;
        }
    }
}
