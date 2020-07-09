using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.IO; // we call the library to read/write to the files

namespace PIWorks
{
    public partial class DesiredOutputForm : Form
    {
        #region Global Variables
        List<string> PlayId = new List<string>();
        List<string> SongId = new List<string>();
        List<string> ClientId = new List<string>();
        List<string> PlayTs = new List<string>();

        List<string> IndexesForPlayTs = new List<string>();
        List<string> IndexesForClientId = new List<string>();
        List<string> IndexesForSongId = new List<string>();

        List<string> NumberOfSongsByClientId = new List<string>();
        List<string> NumberOfTheSongsBySameClientIds = new List<string>();

        int NumberOfIndexesOfTens = 0;

        string[] PlayTs_TheTens_Array = { };

        string[] IndexesForPlayTs_Array = { };
        string[] IndexesForClientId_Array = { };
        string[] IndexesForSongId_Array = { };

        string[] NumberOfSongsForEachClientId_Array = { };
        string[] NumberOfDistinctSongs_Array = { };
        #endregion

        #region File Path Configs-Getting current directory to read and save the files
        string CurrentDirectPath = System.IO.Directory.GetCurrentDirectory();
        #endregion


        public DesiredOutputForm()
        {
            InitializeComponent();

            LoadAndParseCSV(); // We call the template to be parsed

        }

        private void LoadAndParseCSV()
        {
            try
            {
                var reader = new StreamReader(File.OpenRead(CurrentDirectPath + "/exhibitA-input.csv"));

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split('\t');

                    PlayId.Add(line[0]);
                    SongId.Add(line[1]);
                    ClientId.Add(line[2]);
                    PlayTs.Add(line[3]);

                }

                FindingIndexesForPlayTs(); // we go to the function to find the indexes that includes dates that start with 10/..
            }
            catch (Exception e) // if there is no such a path, it confirms the user
            {
                MessageBox.Show(e.Message);
            }

        }

        private void FindingIndexesForPlayTs()
        {
            PlayTs_TheTens_Array = PlayTs.ToArray();

            for (int counter = 0; counter < PlayTs_TheTens_Array.Length; ++counter) 
            {
                if (PlayTs_TheTens_Array[counter].Substring(0,2)=="10")
                {
                    IndexesForPlayTs.Add(counter.ToString());
                }
            }

            NumberOfIndexesOfTens = IndexesForPlayTs.Count;

            FindingClientIds(); // after finding the indexes that includes the dates that begin with 10/.., we go to the function to seperate client ids
        }

        private void FindingClientIds()
        {
            IndexesForPlayTs_Array = IndexesForPlayTs.ToArray();
            IndexesForClientId_Array = ClientId.ToArray();

            for (int counter = 0; counter < NumberOfIndexesOfTens; ++counter)
            {
                IndexesForClientId.Add(IndexesForClientId_Array[Convert.ToInt32(IndexesForPlayTs_Array[counter])]);
            }
            IndexesForClientId_Array = IndexesForClientId.ToArray();

            FindingSongIds(); // we go to the function to do same thinsg we do while finding client ids
        }

        private void FindingSongIds()
        {
            IndexesForPlayTs_Array = IndexesForPlayTs.ToArray();
            IndexesForSongId_Array = SongId.ToArray();

            for (int counter = 0; counter < NumberOfIndexesOfTens; ++counter)
            {
                IndexesForSongId.Add(IndexesForSongId_Array[Convert.ToInt32(IndexesForPlayTs_Array[counter])]);
            }
            IndexesForSongId_Array = IndexesForSongId.ToArray();

            FindingNumberOfSongsForEachClientId(); // after finding song ids as well, we run the function to find number of songs that each client listens

        }

        private void FindingNumberOfSongsForEachClientId()
        {

            for (int counter = 0; counter < NumberOfIndexesOfTens; ++counter)
            {
                int inc = 0;
                NumberOfTheSongsBySameClientIds.Clear();

                for (int counter1 = 0; counter1 < NumberOfIndexesOfTens; ++counter1)
                {
                    if (IndexesForClientId_Array[counter1] == IndexesForClientId_Array[counter])
                    {
                        NumberOfTheSongsBySameClientIds.Add(IndexesForSongId_Array[counter1]);
                    }
                }

                string[] NumberOfTheSongsBySameClientIds_Array = NumberOfTheSongsBySameClientIds.ToArray();

                for (int i = 0; i < NumberOfTheSongsBySameClientIds_Array.Length; ++i)
                {
                    for (int j = i + 1; j < NumberOfTheSongsBySameClientIds_Array.Length; ++j)
                    {
                        if (NumberOfTheSongsBySameClientIds_Array[i] == NumberOfTheSongsBySameClientIds_Array[j])
                        {
                            ++inc;
                        }
                    }

                }
                NumberOfSongsByClientId.Add((NumberOfTheSongsBySameClientIds_Array.Length - inc).ToString());
            }
            FindingNumberOfDistinctSongsForEachClientId(); // after finding number of songs by each client id, we go to the function to find out how may of these are distinct
        }

        private void FindingNumberOfDistinctSongsForEachClientId()
        {
            NumberOfSongsForEachClientId_Array = NumberOfSongsByClientId.ToArray();
            List<string> NumberOfDistinctSongs_List = new List<string>();
            var songs = NumberOfSongsByClientId.Distinct();
            foreach (string song in songs)
            {
                NumberOfDistinctSongs_List.Add(song);
            }
            NumberOfDistinctSongs_Array = NumberOfDistinctSongs_List.ToArray();

            FindingDesiredOutput(); // now we are ready to run desired output algorithm
        }

        private void FindingDesiredOutput()
        {
            List<string> AllClientsWhoListenTheSameSongs = new List<string>();
            var PathToBeSaved = CurrentDirectPath + "/DesiredOutput.txt";
            var File = new StreamWriter(PathToBeSaved);
            File.Write("DISTINCT_PLAY_COUNT" + "\t" + "CLIENT_COUNT");
            File.Write(Environment.NewLine);
            int counterForDistinctSongs = 0;

            for (int k = 0; k < NumberOfDistinctSongs_Array.Length; ++k)
            {
                for (int i = 0; i < NumberOfSongsForEachClientId_Array.Length; ++i)
                {
                    if (NumberOfSongsForEachClientId_Array[i] == NumberOfSongsForEachClientId_Array[k])
                    {
                        AllClientsWhoListenTheSameSongs.Add(IndexesForClientId_Array[i]);
                    }
                }
                var clients = AllClientsWhoListenTheSameSongs.Distinct();
                foreach (string client in clients)
                {
                    ++counterForDistinctSongs;
                }
                File.WriteLine(NumberOfDistinctSongs_Array[k] + "\t" + counterForDistinctSongs.ToString());
                counterForDistinctSongs = 0;
                AllClientsWhoListenTheSameSongs.Clear();
            }
            File.Close();
        }
    }
}
