using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Scenes.Map2D
{
    public class Map2DBlock_Texture
    {
        public Map2DBlockType BType
        {
            get { return Map2DBlockType.texture; }
        }

        public string TextureName
        {
            get { return mTextureName; }
            set { mTextureName = value; }
        }

        string mTextureName;
    }
}
