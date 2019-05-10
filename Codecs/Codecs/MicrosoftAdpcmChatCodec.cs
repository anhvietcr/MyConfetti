using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.ComponentModel.Composition;

namespace Codecs.Codecs
{
    [Export(typeof(INetworkChatCodec))]
    public class MicrosoftAdpcmChatCodec : AcmChatCodec
    {
        public MicrosoftAdpcmChatCodec()
            : base(new WaveFormat(8000, 16, 1), new AdpcmWaveFormat(8000, 1))
        {
        }

        public override string Name { get { return "Microsoft ADPCM"; } }
    }
}