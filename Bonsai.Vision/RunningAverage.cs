﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Bonsai.Vision
{
    public class RunningAverage : Transform<IplImage, IplImage>
    {
        [Range(0, 1)]
        [Precision(2, .01)]
        [Editor(DesignTypes.TrackbarEditor, typeof(UITypeEditor))]
        public double Alpha { get; set; }

        public override IObservable<IplImage> Process(IObservable<IplImage> source)
        {
            return Observable.Defer(() =>
            {
                IplImage accumulator = null;
                return source.Select(input =>
                {
                    if (accumulator == null)
                    {
                        accumulator = new IplImage(input.Size, 32, input.NumChannels);
                        Core.cvConvertScale(input, accumulator, 1, 0);
                        return input;
                    }
                    else
                    {
                        var output = new IplImage(input.Size, input.Depth, input.NumChannels);
                        ImgProc.cvRunningAvg(input, accumulator, Alpha, CvArr.Null);
                        Core.cvConvertScale(accumulator, output, 1, 0);
                        return output;
                    }
                });
            });
        }
    }
}
