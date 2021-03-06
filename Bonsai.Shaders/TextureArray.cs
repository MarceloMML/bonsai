﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bonsai.Shaders
{
    public class TextureArray : IDisposable, IEnumerable<int>
    {
        readonly int[] textures;

        public TextureArray(int length)
        {
            textures = new int[length];
            GL.GenTextures(textures.Length, textures);
        }

        internal TextureArray(int[] textures)
        {
            this.textures = textures;
        }

        public int this[int index]
        {
            get { return textures[index]; }
        }

        public int Length
        {
            get { return textures.Length; }
        }

        public void Dispose()
        {
            GL.DeleteTextures(textures.Length, textures);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < textures.Length; i++)
            {
                yield return textures[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
