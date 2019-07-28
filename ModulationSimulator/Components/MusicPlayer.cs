using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModulationSimulator.Components
{
    /// <summary>
    /// Mp3播放器
    /// </summary>
    public class MusicPlayer
    {
        private MediaPlayer _MediaPlayer;

        public MusicPlayer()
        {
            _MediaPlayer = new MediaPlayer();
        }

        public void Play(string Path)
        {
            if (Path == null) return;
            Uri uri = new Uri(Path, UriKind.Relative);
            _MediaPlayer.Open(uri);
            _MediaPlayer.Play();

            //循环播放
            _MediaPlayer.MediaEnded +=
                (sender, e) => {
                    _MediaPlayer.Position = new TimeSpan(0);
                };
        }

        public void Pause()
        {
            _MediaPlayer.Pause();
        }

        public void Dispose()
        {
            _MediaPlayer.Close();
            _MediaPlayer = null;
        }
    }
}
