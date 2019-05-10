﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.ComponentModel.Composition;

namespace Codecs.Codecs
{
    [Export(typeof(INetworkChatCodec))]
    public class Gsm610ChatCodec : AcmChatCodec
    {
        public Gsm610ChatCodec()
            : base(new WaveFormat(8000, 16, 1), new Gsm610WaveFormat())
        {
        }

        public override string Name { get { return "GSM 6.10"; } }
    }
}