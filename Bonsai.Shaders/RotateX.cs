﻿using OpenTK;
using System;
using System.ComponentModel;

namespace Bonsai.Shaders
{
    [Description("Applies a rotation around the x-axis.")]
    public class RotateX : MatrixTransform
    {
        [Range(-Math.PI, Math.PI)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The angle describing the magnitude of the rotation about the x-axis.")]
        public float Angle { get; set; }

        protected override void CreateTransform(out Matrix4 result)
        {
            Matrix4.CreateRotationX(Angle, out result);
        }
    }
}
