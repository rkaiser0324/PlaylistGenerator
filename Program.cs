using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace PlaylistGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
                Console.WriteLine(@"Syntax: PlaylistGenerator C:\path\to\oldplaylist.txt C:\path\to\Music");
            else
            {
                process(args[0], args[1]);

                Console.WriteLine(@"Any key to continue...");
                Console.ReadKey();
            }

        }

        private static void process(string inputPlaylist, string musicDirectory)
        {
            // https://en.wikipedia.org/wiki/M3U
            string m3u = "#EXTM3U\n";

            string outputDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            string playlistName = Path.GetFileNameWithoutExtension(inputPlaylist);

            string outputFilename = outputDirectory + "\\" + playlistName + ".m3u";

            string[] lines = File.ReadAllLines(inputPlaylist);

            int found = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // Skip header
                if (i > 0)
                {
                    string line = lines[i];
                    string[] fields = line.Split('\t');
                    string artistAndSong = fields[0];
                    string song = fields[0];

                    Regex r = new Regex("( - )"); // Split on hyphens. 
                    string[] tokens = r.Split(artistAndSong);
                    if (tokens.Length == 3)
                        song = tokens[2];

                    string artist = fields[1];
                    string album = fields[4];

                    string path = search(artist, song, musicDirectory);
                    if (path == "")
                    {
                        // drop the [Acoustic] or [Unplugged] from the song
                        r = new Regex(@"( [\(\[](.*)(Acoustic|Unplugged)(.*)[\)\]])");
                        string trimmed = r.Replace(song, "");

                        if (trimmed != song)
                            path = search(artist, trimmed, musicDirectory);
                    }

                    if (path != "")
                    {
                        m3u += path + "\n";
                        found++;
                    }

                    else
                        Console.WriteLine(String.Format("Cannot find \"{0}\" / \"{1}\"", artist, song));
                }
            }

            Console.WriteLine(m3u);

            File.WriteAllText(outputFilename, m3u);

            Console.WriteLine(String.Format("Found {0} / {1} for {2}", found, lines.Length - 1, outputFilename));
        }

        static private string search(string ARTIST, string SONG, string musicDirectory)
        {
            string path = "";

            // https://stackoverflow.com/questions/34338465/how-to-use-windows-search-service-in-c-sharp
            // http://www.thejoyofcode.com/Using_Windows_Search_in_your_applications.aspx

            using (OleDbConnection connection = new OleDbConnection(@"Provider =Search.CollatorDSO;Extended Properties=""Application=Windows"""))
            {

                //// File name search (case insensitive), also searches sub directories
                //var query1 = @"SELECT System.ItemName FROM SystemIndex " +
                //            @"WHERE scope ='file:C:/' AND System.ItemName LIKE '%Test%'";

                //// File name search (case insensitive), does not search sub directories
                //var query2 = @"SELECT System.ItemName FROM SystemIndex " +
                //            @"WHERE directory = 'file:C:/' AND System.ItemName LIKE '%Test%' ";

                //// Folder name search (case insensitive)
                //var query3 = @"SELECT System.ItemName FROM SystemIndex " +
                //            @"WHERE scope = 'file:C:/' AND System.ItemType = 'Directory' AND System.Itemname LIKE '%Test%' ";

                //// Folder name search (case insensitive), does not search sub directories
                //var query4 = @"SELECT System.ItemName FROM SystemIndex " +
                //            @"WHERE directory = 'file:H:/music/O/Oasis' AND System.ItemType = 'Directory' AND System.Itemname LIKE '%Test%' ";


                // https://docs.microsoft.com/en-us/windows/desktop/properties/music-bumper
                var query = String.Format(@"SELECT System.ItemName, System.Music.Artist, System.Music.AlbumTitle, System.ItemType, System.ItemPathDisplay FROM SystemIndex " +
                            @"WHERE scope = 'file:{0}' AND System.Music.Artist LIKE '%{1}%' AND System.Itemname LIKE '%{2}%' " +
                            @"AND System.ItemTypeText = 'Groove Music' AND System.ItemType = '.mp3' ",
                            musicDirectory,
                            ARTIST.Replace("'", "''"),
                            SONG.Replace("'", "''")
                            );

                connection.Open();

                var command = new OleDbCommand(query, connection);

                using (var r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string name = r.GetValue(0).ToString();
                        string artist = "";
                        if (!(r[1] is System.DBNull))
                        {
                            string[] artists = (string[])r[1];
                            artist = artists[0];
                        }

                        string album = r[2].ToString();
                        string filetype = r[3].ToString();
                        path = r[4].ToString();

                        //Console.WriteLine(String.Format("{0} / {1} / {2} / {3}", name, artist, filetype, path));
                        break;
                    }
                    if (path == "")
                    {
                        //Console.WriteLine(String.Format("Cannot find \"{0}\" / \"{1}\"", ARTIST, SONG));
                    }

                }
            }

            return path;
        }
    }
}
