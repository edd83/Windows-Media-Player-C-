using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace MyWindowsMediaPlayer
{
    public class Playlist
    {
        private int index;

        public Playlist(string type)
        {
            items = new List<Media>();
            this.type = type;
            index = 0;
        }

        public Playlist()
        {
            index = 0;
            items = new List<Media>();
        }

        public List<Media> items { get; set; }
        public string type { get; set; }

        public static void save(string path, Playlist list)
        {
            try
            {
                var xs = new XmlSerializer(typeof (Playlist));
                var wr = new StreamWriter(path);
                xs.Serialize(wr, list);
                wr.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Playlist load(string path)
        {
            var xs = new XmlSerializer(typeof (Playlist));
            try
            {
                var rd = new StreamReader(path);
                var p = xs.Deserialize(rd) as Playlist;
                rd.Close();
                return (p);
            }
            catch (Exception)
            {
                Playlist p = new Playlist();
                return (p);
            }
        }

        public int  getNbElement()
        {
            return (items.Count());
        }
        public Media getCurrentItem()
        {
            if (getNbElement() > 0)
                return (items[index]);
            return (null);
        }

        public bool AddSong(string path)
        {
            bool ret = File.Exists(path);

            if (ret)
            {
                items.Add(new Media(Path.GetFileNameWithoutExtension(path), path));
            }
            return (ret);
        }

        public void RemoveSong(string title)
        {
            Media s = FindByTitle(title);
            items.Remove(s);
        }

        public bool Next()
        {
            if (index + 1 < items.Count())
            {
                index += 1;
                return (true);
            }
            return (false);
        }

        public bool Prev()
        {
            if (index - 1 >= 0)
            {
                index -= 1;
                return (true);
            }
            return (false);
        }

        public string to_s()
        {
            string ret = "";
            if (items.Count > 0)
                ret += items.First().to_s();
            return (ret);
        }

        public bool setIndex(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                this.index = index;
                return (true);
            }
            return (false);
        }

        public Media FindByTitle(string title)
        {
            return (items.First(x => x.title == title));
        }

        public int getIndexByTitle(string title)
        {
            return (items.IndexOf(FindByTitle(title)));
        }
    }
}