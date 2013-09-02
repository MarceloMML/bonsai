﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Audio.OpenAL;
using System.ComponentModel;
using System.Threading;
using OpenTK;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive;
using System.Runtime.InteropServices;
using OpenCV.Net;

namespace Bonsai.Audio
{
    [Description("Produces a sequence of buffered samples acquired from the specified audio capture device.")]
    public class AudioCapture : Source<Mat>
    {
        IObservable<Mat> source;

        public AudioCapture()
        {
            BufferLength = 10;
            Frequency = 44100;

            var bufferSize = 0;
            source = Observable.Create<Mat>(observer =>
            {
                var frequency = Frequency;
                bufferSize = (int)Math.Ceiling(frequency * 0.01);
                var captureBufferSize = (int)(BufferLength * frequency * 0.001 / BlittableValueType.StrideOf(short.MinValue));
                var capture = new OpenTK.Audio.AudioCapture(DeviceName, frequency, ALFormat.Mono16, captureBufferSize);

                capture.Start();
                return new CompositeDisposable(
                    HighResolutionScheduler.Default.Schedule<int>((int)(BufferLength / 2 + 0.5), TimeSpan.Zero, (interval, self) =>
                    {
                        while (capture.AvailableSamples > bufferSize)
                        {
                            var buffer = new Mat(1, bufferSize, Depth.S16, 1);
                            capture.ReadSamples(buffer.Data, bufferSize);
                            observer.OnNext(buffer);
                        }
                        self(interval, TimeSpan.FromMilliseconds(interval));
                    }),
                    Disposable.Create(capture.Stop),
                    Disposable.Create(capture.Dispose));
            })
            .PublishReconnectable()
            .RefCount();
        }

        [Description("The name of the capture device from which to acquire samples.")]
        [TypeConverter(typeof(CaptureDeviceNameConverter))]
        public string DeviceName { get; set; }

        [Description("The sampling frequency (Hz) used by the audio capture device.")]
        public int Frequency { get; set; }

        [Description("The length of the sample buffer (ms).")]
        public double BufferLength { get; set; }

        public override IObservable<Mat> Generate()
        {
            return source;
        }
    }
}
