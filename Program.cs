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
        public Coordinaat Positie;
        public Plaatsbaar(char symbol = ' ') {
            this.Symbol = symbol;
        }
        public void ResetPositie() {
            Positie = new Coordinaat(0, 0);
        }
        public virtual void Teken() {
            Tekener.SchrijfOp(Positie, "" + Symbol);
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
        public Vijandje() : base('E') { }
    }
    class Veld : Tekenbaar
    {
        public int Size { get; set; } = 8;
        public void Teken()
        {
            Tekener.SchrijfOp(new Coordinaat(0, 0), "----------");
            for (int i = 1; i < Size; i++)
                Tekener.SchrijfOp(new Coordinaat(0, i), "|        |");
            Tekener.SchrijfOp(new Coordinaat(0, Size), "----------");
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
    class Level : Tekenbaar
    {
        private Veld veld = new Veld();
        public List<Vijandje> Vijandjes { get; set; }
        public string Naam { get; set; }
        public int? Moeilijkheid { get; set; }
        public void Teken()
        {
            veld.Teken();
            foreach (var vijandje in Vijandjes) {
                vijandje.Teken();
            }
            Tekener.SchrijfOp(new Coordinaat(0, veld.Size + 1), Naam ?? "Naamloos level");
            Console.Write(Vijandjes?.Count.AantalString() ?? "0");
            Console.WriteLine(" Vijandjes");
            if (Moeilijkheid != null)
                Console.WriteLine("Moeilijkheidsgraad: " + Moeilijkheid.Value.AantalString());
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            try {
                Console.WriteLine("Welkom bij mijn game!");
                var s = new Speler() { Punten = 10 };
                s.Naam = Console.ReadLine();
                if (s.Naam == "admin") {
                    // doe admin dingen ...
                }
                s.Positie.X = 4;
                s.Positie.Y = 1;
                var level = new Level() { Vijandjes = new List<Vijandje>() { 
                    new Vijandje() { Positie = new Coordinaat(1, 3) }, 
                    new Vijandje() { Positie = new Coordinaat(2, 2) } 
                }};
                level.Teken();
                s.Teken();
                var key = Console.ReadKey();
                while (key.KeyChar != 'q') {
                    switch (key.KeyChar) {
                        case 'a': s.Positie = s.Positie + new Coordinaat(-1, 0); break;
                        case 'w': s.Positie = s.Positie + new Coordinaat(0, -1); break;
                        case 's': s.Positie = s.Positie + new Coordinaat(0, 1); break;
                        case 'd': s.Positie = s.Positie + new Coordinaat(1, 0); break;
                    }
                    while (!Console.KeyAvailable) {
                        level.Teken();
                        s.Teken();
                    }
                    key = Console.ReadKey();
                }
            } catch (NegatiefTekenenException e) {
                Console.WriteLine("Ergens is geprobeerd te tekenen op het negatieve vlak!");
            }
        }
    }
}
