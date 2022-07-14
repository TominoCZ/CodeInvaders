using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace CodeInvaders
{
	public class SoundPlayer : IDisposable
	{
		private readonly Dictionary<string, string> _sounds = new Dictionary<string, string>();

		public SoundPlayer()
		{
			BassManager.Reload();

			var folder = "assets/sounds";
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			var files = Directory.GetFiles(folder);
			foreach (var file in files)
            {
				var id = Path.GetFileNameWithoutExtension(file);
				var ext = Path.GetExtension(file).ToLower();

				if (ext != ".mp3" && ext != ".wav" && ext != ".ogg")
					continue;

				_sounds.Add(id, file);
			}
		}

		public void Play(string id, float volume = 0.5f, float pitch = 1)
		{
			if (!_sounds.TryGetValue(id, out var file))
				return;

			var s = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);//sound, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);

			s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_STREAM_AUTOFREE);

			Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, volume);
			Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (pitch - 1) * 60);

			Bass.BASS_ChannelPlay(s, false);
		}

		public void Dispose()
		{
			Bass.BASS_Free();
		}
	}
}
