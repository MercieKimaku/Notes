using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.Storage;

namespace Notes.Data_Model
{
    // class that holds the properties of a note
    public class Note
    {
        public string Title { get; set; }
        public string  Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
   public class DataSource
    {
        private ObservableCollection<Note> _myNotes;
        const string filename = "mapnotes.json";
        public DataSource()
        {
            _myNotes = new ObservableCollection<Note>();
        }

        public async Task <ObservableCollection<Note>> GetMapNotes()
        {
            await ensureDataLoaded();
            return _myNotes;
        }

        private async Task ensureDataLoaded()
        {
            if(_myNotes.Count == 0)
            {
                await getMapNoteDataAsync();
            }
        }

        private async Task getMapNoteDataAsync()
        {
            if(_myNotes.Count != 0)
                return;
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Note>));
            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename))
                {
                    _myNotes = (ObservableCollection<Note>)jsonSerializer.ReadObject(stream);

                }
            }
            catch 
            {
                _myNotes = new ObservableCollection<Note>();
            }
            
        }

        public async void AddNote(Note myNote)
        {
            _myNotes.Add(myNote);
            await saveNoteDataAsync();
        }

        private async Task saveNoteDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Note>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(filename,
                CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _myNotes);
            }

        }

        public async void DeleteMapNote(Note myNote)
        {
            _myNotes.Remove(myNote);
            await saveNoteDataAsync();
        }

    }
}
