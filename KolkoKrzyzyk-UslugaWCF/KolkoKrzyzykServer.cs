using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KolkoKrzyzyk_UslugaWCF
{
    // UWAGA: możesz użyć polecenia „Zmień nazwę” w menu „Refaktoryzuj”, aby zmienić nazwę klasy „Service1” w kodzie i pliku konfiguracji.
    public class KolkoKrzyzykServer : IGraKolkoKrzyzyk
    {
        const int W = 3; // Wiersze
        const int K = 3; // Kolumny
        static int[,] ArrayPlansza = new int[W, K];
        static List<int> ListaRuchow = new List<int>();

        static int ZnakKolko = 1;
        static int ZnakKrzyzyk = 2;
        Random rnd = new Random();

        public int Start(int whoDat)
        {
            // Set a clean board
            ListaRuchow.Clear();
            for (int i = 0; i < W; i++) // po wierszach
            {
                for (int j = 0; j < K; j++) // po kolumnach
                {
                    ArrayPlansza[i, j] = 0;
                    ListaRuchow.Add(i * 3 + j);
                }
            }
            if (whoDat == 0)
            {
                return -1;
            } else
            {
                int movno = rnd.Next() % 9;
                int indeksPola = ListaRuchow[movno];
                ListaRuchow.RemoveAt(movno);

                // Oblicz indeksy i wstaw symbol
                int wierszServer = indeksPola / W;
                int kolumnaServer = indeksPola % K;
                ArrayPlansza[wierszServer, kolumnaServer] = ZnakKrzyzyk;
                return indeksPola;
            }
        }

        public bool WykonajRuch(int wiersz, int kolumna, out int wierszServer, out int kolumnaServer)
        {
            // Ruch klienta, zapisz i usun
            int movno = wiersz * 3 + kolumna;
            ListaRuchow.Remove(movno);
            ArrayPlansza[wiersz, kolumna] = ZnakKolko;

            // Wykryj zwyciestwo gracza
            int isWin = SprawdzWygrana(wiersz, kolumna);
            if(isWin > 0)
            {
                wierszServer = -1;
                kolumnaServer = ZnakKolko;
                return true;
            }

            // Ruch serwera - losuj i usun
            int newCount = ListaRuchow.Count();
            // Złap remis
            if (newCount < 1)
            {
                wierszServer = -1;
                kolumnaServer = 0;
                return true;
            }
            movno = rnd.Next() % newCount;
            int move = ListaRuchow[movno];
            ListaRuchow.RemoveAt(movno);

            // Oblicz indeksy i wstaw symbol
            kolumnaServer = move % K;
            wierszServer = (move - kolumnaServer) / 3;
            ArrayPlansza[wierszServer, kolumnaServer] = ZnakKrzyzyk;

            isWin = SprawdzWygrana(wierszServer, kolumnaServer);
            if (isWin > 0)
            {
                return false;
            }

            newCount = ListaRuchow.Count();
            // Złap remis po raz 2
            if (newCount < 1)
            {
                wierszServer = -1;
                kolumnaServer = 0;
                return true;
            }
            return true;
        }

        public int SprawdzWygrana(int wiersz, int kolumna)
        {
            // Wiersze i kolumny
            int player = ArrayPlansza[wiersz, kolumna];
            for (int i = 0; i < K; i++)
            {
                if ((ArrayPlansza[0, i] == ArrayPlansza[1, i] && ArrayPlansza[1, i] == ArrayPlansza[2, i]) && ArrayPlansza[1, i] == player)
                {
                    return 1;
                }
                if ((ArrayPlansza[i, 0] == ArrayPlansza[i, 1] && ArrayPlansza[i, 1] == ArrayPlansza[i, 2]) && ArrayPlansza[i, 1] == player)
                {
                    return 2;
                }
            }

            // Ukosy
            if ((ArrayPlansza[0, 0] == ArrayPlansza[1, 1] && ArrayPlansza[1, 1] == ArrayPlansza[2, 2]) && ArrayPlansza[1, 1] == player)
            {
                return 4;
            }
            if ((ArrayPlansza[0, 2] == ArrayPlansza[1, 1] && ArrayPlansza[1, 1] == ArrayPlansza[2, 0]) && ArrayPlansza[1, 1] == player)
            {
                return 8;
            }

            //int player = ArrayPlansza[wiersz, kolumna];
            // check rows
            //if (ArrayPlansza[0, 0] == player && ArrayPlansza[0, 1] == player && ArrayPlansza[0, 2] == player) { return 1; }
            //if (ArrayPlansza[1, 0] == player && ArrayPlansza[1, 1] == player && ArrayPlansza[1, 2] == player) { return 1; }
            //if (ArrayPlansza[2, 0] == player && ArrayPlansza[2, 1] == player && ArrayPlansza[2, 2] == player) { return 1; }

            // check columns
            //if (ArrayPlansza[0, 0] == player && ArrayPlansza[1, 0] == player && ArrayPlansza[2, 0] == player) { return 2; }
            //if (ArrayPlansza[0, 1] == player && ArrayPlansza[1, 1] == player && ArrayPlansza[2, 1] == player) { return 2; }
            //if (ArrayPlansza[0, 2] == player && ArrayPlansza[1, 2] == player && ArrayPlansza[2, 2] == player) { return 2; }

            // check diags
            //if (ArrayPlansza[0, 0] == player && ArrayPlansza[1, 1] == player && ArrayPlansza[2, 2] == player) { return 4; }
            //if (ArrayPlansza[0, 2] == player && ArrayPlansza[1, 1] == player && ArrayPlansza[2, 0] == player) { return 8; }

            return 0;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
    }
}
