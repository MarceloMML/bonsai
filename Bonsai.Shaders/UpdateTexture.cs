﻿using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Shaders
{
    public class UpdateTexture : Sink<IplImage>
    {
        [Editor("Bonsai.Shaders.Design.ShaderConfigurationEditor, Bonsai.Shaders.Design", typeof(UITypeEditor))]
        public string ShaderName { get; set; }

        public override IObservable<IplImage> Process(IObservable<IplImage> source)
        {
            return Observable.Create<IplImage>(observer =>
            {
                var resource = ShaderManager.ReserveShader(ShaderName);
                resource.Shader.Subscribe(observer);
                return source.Do(input =>
                {
                    resource.Shader.Update(() =>
                    {
                        TextureHelper.UpdateTexture(resource.Shader.Texture, input);
                    });
                })
                .Finally(resource.Dispose)
                .SubscribeSafe(observer);
            });
        }
    }
}