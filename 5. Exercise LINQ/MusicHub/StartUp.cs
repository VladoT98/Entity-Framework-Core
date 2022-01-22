using System.Globalization;
using System.Linq;
using System.Text;
using MusicHub.Data.Models;

namespace MusicHub
{
    using System;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            string result = ExportSongsAboveDuration(context, 4);

            Console.WriteLine(result);

            //Test your solutions here
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context
                .Albums
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs
                        .ToArray()
                        .Select(s => new
                        {
                            SongName = s.Name,
                            s.Price,
                            Writer = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.Writer)
                        .ToArray(),
                    TotalAlbumPrice = a.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in albums)
            {
                sb.AppendLine($"-AlbumName: {a.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 1;

                foreach (var s in a.Songs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price:F2}");
                    sb.AppendLine($"---Writer: {s.Writer}");
                }

                sb.AppendLine($"-AlbumPrice: {a.TotalAlbumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context
                .Songs
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    Performer = s.SongPerformers
                        .ToArray()
                        .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                        .FirstOrDefault(),
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c", CultureInfo.InvariantCulture)
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.Writer)
                .ThenBy(s => s.Performer)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            int counter = 1;
            foreach (var s in songs)
            {
                sb.AppendLine($"-Song #{counter++}");
                sb.AppendLine($"---SongName: {s.SongName}");
                sb.AppendLine($"---Writer: {s.Writer}");
                sb.AppendLine($"---Performer: {s.Performer}");
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
