﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.ComponentModel.Composition;

namespace Codecs.Codecs
{
    /// <summary>
    /// DSP Group TrueSpeech codec, using ACM
    /// n.b. Windows XP came with a TrueSpeech codec built in
    /// - looks like Windows 7 doesn't
    /// </summary>
    [Export(typeof(INetworkChatCodec))]
    public class TrueSpeechChatCodec : AcmChatCodec
    {
        public TrueSpeechChatCodec()
            : base(new WaveFormat(8000, 16, 1), new TrueSpeechWaveFormat())
        {
        }


        public override string Name
        {
            get { return "DSP Group TrueSpeech"; }
        }

    }
}