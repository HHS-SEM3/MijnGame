using System;
using System.Collections.Generic;

namespace MijnGame
{
    interface Tekenbaar {
        void Teken();
    }
    struct Coordinaat {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinaat(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }
        public static Coordinaat operator +(Coordinaat c1, Coordinaat c2) {
            return new Coordinaat(c1.X + c2.X, c1.Y + c2.Y);
        }
        public static int Afstand(Coordinaat c1, Coordinaat c2) {
            return Math.Abs(c1.X - c2.X) + Math.Abs(c1.Y - c2.Y);
        }
        public static Coordinaat Willekeurig(int size)
        {
            return new Coordinaat(Program.random.Next(size), Program.random.Next(size));
        }
        public static bool operator ==(Coordinaat c1, Coordinaat c2) {
            return c1.X == c2.X && c1.Y == c2.Y;
        }
        public static bool operator !=(Coordinaat c1, Coordinaat c2) {
            return c1.X != c2.X || c1.Y != c2.Y;
        }
    }
    class NegatiefTekenenException : Exception { }
    static class Tekener {
        public static void SchrijfOp(Coordinaat Positie, string Text) {
            if (Positie.X < 0 || Positie.Y < 0)
                throw new NegatiefTekenenException();
            Console.SetCursorPosition(Positie.X, Positie.Y);
            Console.WriteLine(Text);
        }
    }
    abstract class Plaatsbaar : Tekenbaar {
        private Coordinaat positie;
        public Coordinaat Positie {
            get {
                return positie;
            }
            set { // gebruik tenminste 1 keer zinvol een niet auto-implemented property
                positie = value + new Coordinaat(Veld.Size, Veld.Size);
                positie.X %= Veld.Size;
                positie.Y %= Veld.Size;
            }
        }
        public Plaatsbaar(char symbol = ' ') {
            this.Symbol = symbol;
        }
        public void ResetPositie() {
            Positie = new Coordinaat(0, 0);
        }
        public virtual void Teken() {
            Tekener.SchrijfOp(Positie + new Coordinaat(1, 1), "" + Symbol);
        }
        public virtual char Symbol { get; }
    }
    class Muntje : Plaatsbaar {
        private bool knipper;
        public override char Symbol
        {
             get {
                if (knipper)
                    return 'O';
                else
                    return ' ';
             }
        }
        public override void Teken() {
            base.Teken();
            knipper = !knipper;
        }
    }
    class Speler : Plaatsbaar {
        public string Naam { get; set; }
        public int Punten { get; set; }
        public Speler() : base('*') { }
        public static bool operator >(Speler sp1, Speler sp2) {
            return sp1.Punten > sp2.Punten;
        }
        public static bool operator <(Speler sp1, Speler sp2) {
            return sp1.Punten < sp2.Punten;
        }
    }
    class Vijandje : Plaatsbaar {
        public Coordinaat Snelheid { get; set; }
        public Vijandje() : base('E') { }
        public void Beweeg() {
            Positie += Snelheid;
        }
    }
    class Veld : Tekenbaar
    {
        public const int Size = 18;
        public void Teken()
        {
            Tekener.SchrijfOp(new Coordinaat(0, 0), new String('-', Size + 2));
            for (int i = 1; i < Size + 1; i++)
                Tekener.SchrijfOp(new Coordinaat(0, i), "|" + new String(' ', Size) + "|");
            Tekener.SchrijfOp(new Coordinaat(0, Size + 1), new String('-', Size + 2));
        }
    }
    static class AantalExtensie
    {
        public static String AantalString(this int num) {
            if (num >= 1000000000) { return (num / 1000000000).ToString() + "B"; }
            if (num >= 1000000) { return (num / 1000000).ToString() + "M"; }
            if (num >= 1000) { return (num / 1000).ToString() + "k"; }
            return num.ToString();  
        }
    }
    class MoeilijkheidsStand // maak tenminste 1 zinvolle nieuwe klasse aan
    {
        public int VijandInterval { get; set; }
        public int VijandSnelheid { get; set; }
        // null indien er geen muntjes zijn
        public int? MuntjeInterval { get; set; } // maak tenminste 1 keer zinvol gebruik van een nullable 
    }
    class Level : Tekenbaar
    {
        private Veld veld = new Veld();
        public int Score { get; set; } = 0;
        public List<Muntje> Muntjes { get; set; } // maak een zinvolle, nieuwe lijst aan, en gebruik deze
        public List<Vijandje> Vijandjes { get; set; }
        public string Naam { get; set; }
        public int? Moeilijkheid { get; set; }
        public void Teken()
        {
            veld.Teken();
            foreach (var muntje in Muntjes) // gebruik tenminste 1 keer het keyword var
                muntje.Teken();
            foreach (var vijandje in Vijandjes)
                vijandje.Teken();
            Tekener.SchrijfOp(new Coordinaat(0, Veld.Size + 2), Naam ?? "Naamloos level");
            Console.Write(Vijandjes?.Count.AantalString() ?? "0");
            Console.WriteLine(" Vijandjes");
            Console.WriteLine(Score + " Punten!");
            if (Moeilijkheid != null)
                Console.WriteLine("Moeilijkheidsgraad: " + Moeilijkheid.Value.AantalString());
        }
    }
    class Program
    {
        public static readonly Random random = new Random();
        static void Main(string[] args)
        {
            var moeilijkheidsStanden = new List<MoeilijkheidsStand>() { // maak tenminste 1 keer gebruik van een list initializer
                new MoeilijkheidsStand { VijandSnelheid = 10, VijandInterval = 30}, // <- MuntjeInterval = null
                new MoeilijkheidsStand { MuntjeInterval = 30, VijandSnelheid = 10, VijandInterval = 30}, 
                new MoeilijkheidsStand { MuntjeInterval = 15, VijandSnelheid = 5, VijandInterval = 20}
            };
            Console.CursorVisible = false;
            try {
                Console.WriteLine("Welkom bij mijn game!");
                var s = new Speler() { Punten = 10 };
                s.Naam = Console.ReadLine();
                if (s.Naam == "admin") {
                    Console.WriteLine("Als admin ben je onsterfelijk! Klik enter...");
                    Console.ReadLine();
                }
                Console.WriteLine("Geef de moeilijkheids stand op: (0 - 2)");
                var moeilijkheid = Int16.Parse(Console.ReadLine());
                // Dit is niet meer mogelijk, omdat het een property is: 
                // s.Positie.X = 4;
                // s.Positie.Y = 1;
                s.Positie = new Coordinaat(Veld.Size / 2, Veld.Size / 2); 
                var level = new Level() { Vijandjes = new List<Vijandje>(), Muntjes = new List<Muntje>() };
                level.Teken();
                s.Teken();
                var key = Console.ReadKey();
                var time = 0;
                var dood = false;
                while (key.KeyChar != 'q' && !dood) {
                    switch (key.KeyChar) {
                        case 'a': s.Positie = s.Positie + new Coordinaat(-1, 0); break;
                        case 'w': s.Positie = s.Positie + new Coordinaat(0, -1); break;
                        case 's': s.Positie = s.Positie + new Coordinaat(0, 1); break;
                        case 'd': s.Positie = s.Positie + new Coordinaat(1, 0); break;
                    }
                    List<Muntje> teVerwijderen = new List<Muntje>();
                    foreach (var muntje in level.Muntjes)
                        if (muntje.Positie == s.Positie)
                            teVerwijderen.Add(muntje);
                    level.Score += teVerwijderen.Count;
                    foreach (var muntje in teVerwijderen)
                        level.Muntjes.Remove(muntje);
                    while (!Console.KeyAvailable && !dood) {
                        level.Teken();
                        s.Teken();
                        foreach (var vijandje in level.Vijandjes)
                            if (vijandje.Positie == s.Positie && s.Naam != "admin")
                            {
                                Console.WriteLine("Je bent gestorven!"); 
                                Console.ReadLine();
                                dood = true;
                                continue;
                            }
                        if (time % moeilijkheidsStanden[moeilijkheid].VijandSnelheid == 0)
                        {
                            foreach (var vijandje in level.Vijandjes)
                                vijandje.Beweeg();
                        }
                        if (time % moeilijkheidsStanden[moeilijkheid].VijandInterval == 0)
                        {
                            Coordinaat positie;
                            bool botsing;
                            do {
                                positie = Coordinaat.Willekeurig(Veld.Size);
                                botsing = false;
                                foreach (var vijandje in level.Vijandjes)
                                    if (vijandje.Positie == positie) // let op: hiervoor is de operator == geoverload
                                        botsing = true;
                                // met LINQ volgende week kan het ook zo: 
                                // botsing = level.Vijandjes.Any((vijandje) => vijandje.Positie == positie); 
                            } while (botsing || Coordinaat.Afstand(s.Positie, positie) <= 3);
                            var snelheid = random.Next(4);
                            level.Vijandjes.Add(new Vijandje { Positie = positie, Snelheid = new Coordinaat((1 - (snelheid % 2)) * (snelheid - 1), (snelheid % 2) * (snelheid - 2))}); // maak tenminste 1 keer gebruik van een object initializer
                        }
                        if (moeilijkheidsStanden[moeilijkheid].MuntjeInterval != null && (time + 5) % moeilijkheidsStanden[moeilijkheid].MuntjeInterval == 0)
                        {
                            level.Muntjes.Add(new Muntje {Positie = Coordinaat.Willekeurig(Veld.Size)});
                        }
                        System.Threading.Thread.Sleep(100);
                        time++;
                    }
                    key = Console.ReadKey();
                }
            } catch (NegatiefTekenenException e) {
                Console.WriteLine("Ergens is geprobeerd te tekenen op het negatieve vlak!");
            }
        }
    }
}
