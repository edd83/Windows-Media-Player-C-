using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer
{
    public class Media
    {
        public Media() { }
        public Media(string title, string path)
        {
            this.path = path;
            this.title = title;
        }

        public string to_s()
        {
            return (this.title);
        }

        public string title { get; set; }
        public string path { get; set; }
    }
}
