using System;
using System.Collections.Generic;
using System.IO;

// Klass för Artist
// Klassen har egenskaperna Name och PopularSong
class Artist
{
    public string Name { get; set; } // Få artistens namn
    public string PopularSong { get; set; } // Få artistens populäraste låt 
}

// Klass för Stage
// Klassen har egenskaperna Name och Capacity (kapaciteten för scenen)
class Stage
{
    public string Name { get; set; } // Få scenens namn
    public int Capacity { get; set; } // Få scenens kapacitet 
}

// Klass för Schema
// Klassen hanterar schemaläggningen av artister på festivalen
// Lista scheduledArtists för att se vilka artister som är schemalagda.
// Metoden AddArtistToSchedule lägger till en artist i schemat och triggar ett evenemang (ScheduleChanged) som meddelar att schemat har ändrats.
// Metoden DisplaySchedule visar festivalens schema.
// Metod för att ladda och spara till en fil.
class Schedule
{
    private List<ScheduledArtist> scheduledArtists = new List<ScheduledArtist>(); // Lista för att lagra schemalagda artister

    public event Action<string, string, DateTime> ScheduleChanged; // Händelse när schemat ändras

    // Lägg till artist i schemat
    public void AddArtistToSchedule(Artist artist, Stage stage, DateTime time)
    {
        // Skapar en instans av ScheduledArtist och lägg till i listan 
        ScheduledArtist scheduledArtist = new ScheduledArtist(artist.Name, stage.Name, time);
        scheduledArtists.Add(scheduledArtist);

        // Informera om ändring i schemat
        ScheduleChanged?.Invoke(artist.Name, stage.Name, time);
    }

    // Visa festivalens schema
    public void DisplaySchedule()
    {
        Console.WriteLine("Festivalens schema:");

        // Kontrollera om det finns några schemalagda artister 
        if (scheduledArtists.Count == 0) // Om det ej finns = Visa meddelande 
        {
            Console.WriteLine("Inga schemalagda artister för närvarande.");
            return;
        }

        // Loopa igenom alla schemalagda artister och visa informationen
        foreach (var scheduledArtist in scheduledArtists)
        {
            Console.WriteLine($"Artist: {scheduledArtist.ArtistName}, Scen: {scheduledArtist.StageName}, Tidpunkt: {scheduledArtist.Time}");
        }
    }

    //  Klass för schemalagd artist.
    //  Information om artistens namn, scenens namn och tidpunkten för framträdandet.
    private class ScheduledArtist
    {
        public string ArtistName { get; } // Artistens namn
        public string StageName { get; } // Scenens namn 
        public DateTime Time { get; } // Tiden för framträdandet 

        // schemalägg en artist
        public ScheduledArtist(string artistName, string stageName, DateTime time)
        {
            ArtistName = artistName;
            StageName = stageName;
            Time = time;
        }
    }

    // Ladda schema från fil
    public void LoadFromFile(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    // Kontrollera om raden har rätt format och konvertera tiden
                    if (parts.Length == 3 &&
                        DateTime.TryParse(parts[2], out DateTime time))
                    {
                        AddArtistToSchedule(new Artist { Name = parts[0] }, new Stage { Name = parts[1] }, time);
                    }
                }
            }

            Console.WriteLine("Festivalplanen har laddats från filen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid inläsning från fil: {ex.Message}");
        }
    }

    // Spara schema till fil
    public void SaveToFile(string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var scheduledArtist in scheduledArtists) // Skriv varje schemalagd artist till filen
                {
                    writer.WriteLine($"{scheduledArtist.ArtistName},{scheduledArtist.StageName},{scheduledArtist.Time}");
                }
            }

            Console.WriteLine("Festivalplanen har sparats till filen.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid sparande till fil: {ex.Message}");
        }
    }
}

// Huvudapplikationsklass
// Denna klass innehåller Main-metoden, där programmet startar.
// Den innehåller huvudmenyn för användaren med olika alternativ som att lägga till artister, lägga till scener, schemalägga artister, visa schemat för festivalen, visa alla artister och deras populära låtar, visa alla scener och deras kapacitet, ladda festivalplan från fil, spara festivalplan till fil och avsluta programmet.
// Varje alternativ har en motsvarande metod som utför den önskade åtgärden.
class Program
{
    static List<Artist> artists = new List<Artist>(); // Lista för att lagra artister
    static List<Stage> stages = new List<Stage>(); // Lista för att lagra scener
    static Schedule festivalSchedule = new Schedule(); // Schemat för festivalen

    static void Main()
    {
        // Visa huvudmenyn för användaren
        while (true)
        {
            Console.WriteLine("1. Lägg till artist");
            Console.WriteLine("2. Lägg till scen");
            Console.WriteLine("3. Schemalägg artist");
            Console.WriteLine("4. Visa schemat för festivalen");
            Console.WriteLine("5. Visa alla artister och deras populära låtar");
            Console.WriteLine("6. Visa alla scener och deras kapacitet");
            Console.WriteLine("7. Ladda festivalplan från fil");
            Console.WriteLine("8. Spara festivalplan till fil");
            Console.WriteLine("9. Avsluta");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Ogiltigt val. Ange en siffra.");
                continue;
            }

            // Hantera beroende på användarens val
            switch (choice)
            {
                case 1:
                    AddArtist();
                    break;
                case 2:
                    AddStage();
                    break;
                case 3:
                    ScheduleArtist();
                    break;
                case 4:
                    festivalSchedule.DisplaySchedule();
                    break;
                case 5:
                    DisplayArtists();
                    break;
                case 6:
                    DisplayStages();
                    break;
                case 7:
                    LoadFromFile();
                    break;
                case 8:
                    SaveToFile();
                    break;
                case 9:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }
    }

    // Metod för att lägga till artist
    static void AddArtist()
    {
        Console.WriteLine("Ange artistens namn:");
        string name = Console.ReadLine();

        Console.WriteLine("Ange artistens populära låt:");
        string popularSong = Console.ReadLine();

        // Skapa en ny artist och lägg till i listan
        Artist newArtist = new Artist { Name = name, PopularSong = popularSong };
        artists.Add(newArtist);
        Console.WriteLine("Artist tillagd!");
    }

    // Metod för att lägga till scen
    static void AddStage()
    {
        Console.WriteLine("Ange scenens namn:");
        string name = Console.ReadLine();

        Console.WriteLine("Ange scenens kapacitet:");
        if (!int.TryParse(Console.ReadLine(), out int capacity))
        {
            Console.WriteLine("Ogiltig kapacitet. Ange en siffra.");
            return;
        }

        // Skapa en ny scen och lägg till i listan
        Stage newStage = new Stage { Name = name, Capacity = capacity };
        stages.Add(newStage);
        Console.WriteLine("Scen tillagd!");
    }

    // Metod för att schemalägga artist
    static void ScheduleArtist()
    {
        Console.WriteLine("Välj artist att schemalägga:");
        DisplayArtists();
        if (!int.TryParse(Console.ReadLine(), out int artistIndex) || artistIndex < 1 || artistIndex > artists.Count)
        {
            Console.WriteLine("Ogiltigt val. Försök igen.");
            return;
        }

        Console.WriteLine("Välj scen att schemalägga på:");
        DisplayStages();
        if (!int.TryParse(Console.ReadLine(), out int stageIndex) || stageIndex < 1 || stageIndex > stages.Count)
        {
            Console.WriteLine("Ogiltigt val. Försök igen.");
            return;
        }

        Console.WriteLine("Ange tidpunkt för schemaläggning (YYYY-MM-DD HH:mm):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime time))
        {
            Console.WriteLine("Ogiltigt datum och tid. Ange i formatet YYYY-MM-DD HH:mm.");
            return;
        }

        // Schemalägg artisten
        festivalSchedule.AddArtistToSchedule(artists[artistIndex - 1], stages[stageIndex - 1], time);
        Console.WriteLine("Artist schemalagd!");
    }

    // Metod för att visa artister + deras populära låtar
    static void DisplayArtists()
    {
        Console.WriteLine("Alla artister och deras populära låtar:");
        for (int i = 0; i < artists.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {artists[i].Name} - {artists[i].PopularSong}");
        }
    }

    // Metod för att visa alla scener och deras kapacitet
    static void DisplayStages()
    {
        Console.WriteLine("Alla scener och deras kapacitet:");
        for (int i = 0; i < stages.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {stages[i].Name} - Kapacitet: {stages[i].Capacity}");
        }
    }

    // Metod för att ladda festivalplan från fil
    static void LoadFromFile()
    {
        Console.WriteLine("Ange filens sökväg för inläsning:");
        string filePath = Console.ReadLine();

        // Anropa metoden för att ladda från fil
        festivalSchedule.LoadFromFile(filePath);
    }

    // Metod för att spara festivalplan till fil
    static void SaveToFile()
    {
        Console.WriteLine("Ange filens sökväg för sparande:");
        string filePath = Console.ReadLine();

        // Anropa metoden för att spara till fil
        festivalSchedule.SaveToFile(filePath);
    }
}
