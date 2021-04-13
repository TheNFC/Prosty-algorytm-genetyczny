using System;
using System.IO;

namespace ESI
{
    class Program
    {
        private static Random rand = new Random();

        private static int a = 4;
        private static int b = 7;
        private static int c = 2;
        private static int ile_wyn = 40;
        private static int lb_pop = 6;
        private static int ile_os = (150 / lb_pop); //lb_pop*ile_os <= 150
        private static int dlugosckrzyz;

        private static double pr_krzyz = 0.9; //90% szans na krzyżowanie
        private static double pr_mut = 0.05; //5% szans na mutację

        private static int lp = 1; //liczenie wierszy do pliku
        

        static void Main(string[] args)
        {
            byte[] osobniki = new byte[ile_os];
            byte[] dzieci = new byte[ile_os];

            for(int gpetla = 0; gpetla < ile_wyn; gpetla++)
            {
                for (int i = 0; i < ile_os; i++) //generowanie osobników
                {
                    osobniki[i] = Convert.ToByte(rand.Next(255)); //zakres 0-255
                }
                
 
                for (int j = 0; j < lb_pop; j++) //pętla populacji
                {
                    Shuffle(osobniki); //wywoływanie funkcji
                    Krzyzowanie(osobniki, dzieci);
                    Mutacje(dzieci);
                    Ruletka(dzieci, osobniki);
                    if(j == lb_pop-1) //w ostatniej populacji szuka najwyższej wartości
                    {
                        Najwyzsze(osobniki);
                    }
                }
                
            }
        }
        public static void Shuffle<T>(T[] osobniki) //funkcja do ustawienia osobników na losowych pozycjach w tablicy
        {
            for (int i = 0; i < osobniki.Length - 1; i++)
            {
                int j = rand.Next(i, osobniki.Length);
                T temp = osobniki[i];
                osobniki[i] = osobniki[j];
                osobniki[j] = temp;
            }
        }

        public static void Krzyzowanie(byte[] osobniki, byte[] dzieci) //funkcja do krzyzowania par i jak jest nieparzysta liczba osobnikow to przypisanie
        {                                                              // nieparzystego do tablicy dzieci
            if (osobniki.Length % 2 != 0) //sprawdzanie parzystosci
            {
                dlugosckrzyz = (osobniki.Length) - 1;
                dzieci[dlugosckrzyz] = osobniki[dlugosckrzyz]; //przypisanie nieparzystego osobnika
            }
            else
            {
                dlugosckrzyz = (osobniki.Length);
            }

            for (int i = 0; i < dlugosckrzyz; i += 2)
            {
                if (rand.NextDouble() <= pr_krzyz) //losowanie czy bedzie krzyzowanie
                {
                    int przeciecie = rand.Next(1, 7);

                    string tempjeden = Convert.ToString(osobniki[i], 2).PadLeft(8, '0');
                    string tempdwa = Convert.ToString(osobniki[i + 1], 2).PadLeft(8, '0');

                    string tempdjeden = (tempjeden.Substring(0, przeciecie) + tempdwa.Substring(przeciecie));
                    string tempddwa = (tempdwa.Substring(0, przeciecie) + tempjeden.Substring(przeciecie));

                    dzieci[i] = Convert.ToByte(tempdjeden, 2);
                    dzieci[i + 1] = Convert.ToByte(tempddwa, 2);
                }
                else //brak krzyzowania, przypisanie osobnikow do dzieci
                {
                    dzieci[i] = osobniki[i];
                    dzieci[i + 1] = osobniki[i + 1];
                }
            }
        }
        public static void Mutacje(byte[] dzieci) //funkcja do mutowania osobnikow
        {
            for(int i = 0; i < dzieci.Length; i++)
            {
                for(int n = 0;n <= 7; n++)
                {
                    if (rand.NextDouble() <= pr_mut) //losowanie czy dany bit zostanie mutowany
                    {
                        string temp = Convert.ToString(dzieci[i], 2).PadLeft(8, '0');
                        string znak = "";
                        string tempkoncowy = "";

                        if(n == 0)
                        {
                            znak = temp.Substring(0, 1);
                            if (znak == "0")
                            {
                                tempkoncowy = "1" + temp.Substring(1);
                            }
                            else
                            {
                                tempkoncowy = "0" + temp.Substring(1);
                            }
                        }
                        else if(n == 7)
                        {
                            znak = temp.Substring(7, 1);
                            if (znak == "0")
                            {
                                tempkoncowy = temp.Substring(0, 7) + "1";
                            }
                            else
                            {
                                tempkoncowy = temp.Substring(0, 7) + "0";
                            }
                        }
                        else
                        {
                            znak = temp.Substring(n,1);
                            if (znak == "0")
                            {
                                tempkoncowy = temp.Substring(0, n) + "1" + temp.Substring(n+1);
                            }
                            else
                            {
                                tempkoncowy = temp.Substring(0, n) + "0" + temp.Substring(n+1);
                            }
                        }
                        dzieci[i] = Convert.ToByte(tempkoncowy, 2);
                    }
                }
            }
        }
        public static void Ruletka(byte[] dzieci, byte[] osobniki) //losowanie czy dany osobnik przejdzie dalej
        {
            double[] przedzialy = new double[ile_os+1]; //tworzenie tablicy dla przedziału
            double[] losruletka = new double[ile_os]; //tworzenie tablicy dla wylosowanych liczb z ruletki
            double suma = 0;

            for (int i = 0; i < dzieci.Length; i++)
            {
                suma += (a*(dzieci[i]*dzieci[i]) + b*dzieci[i] + c); //f(suma)=f(x1)+...+f(xn)
                losruletka[i] = rand.NextDouble(); //zapisywanie koła ruletki do tablicy
                
            }
            przedzialy[0] = 0.0;
            przedzialy[ile_os] = 1.0;
            for(int j = 1; j < dzieci.Length; j++)
            {
                przedzialy[j] = przedzialy[j-1]+((a * (dzieci[j] * dzieci[j]) + b * dzieci[j] + c) / suma); //p(xi)=f(xi)/f(suma) - tworzenie przedziałów
            }
            for(int n = 0; n < losruletka.Length; n++)
            { 
                for(int k = 0; k < przedzialy.Length - 1; k++)
                {
                    if(losruletka[n] == przedzialy[k]  || ((losruletka[n] < przedzialy[k]) && (losruletka[n] > przedzialy[k+1]))) //sprawdzanie czy liczba wygenerowana w ruletce znajduję się w przedziale
                    {
                        osobniki[k] = dzieci[k]; //przejście dziecka do następnej populacji
                    }
                }
            }
        }
        public static void Najwyzsze(byte[] osobniki) //Obliczanie największej wartości funkcji i osobnika
        {
            int funkcjanajwieksza = 0;
            int funkcja;
            int wartoscnajwieksza = 0;

            for(int i = 0; i < osobniki.Length; i++)
            {
                funkcja = (a * (osobniki[i] * osobniki[i]) + b * osobniki[i] + c);
                if(funkcja > funkcjanajwieksza)
                {
                    funkcjanajwieksza = funkcja;
                }
                if(osobniki[i] > wartoscnajwieksza)
                {
                    wartoscnajwieksza = osobniki[i];
                }
            }
            DateTime dzis = DateTime.Now;

            using (StreamWriter sw = File.AppendText("wyniki_"+ (dzis.ToString("yyyy-dd-M--HH-mm-ss")) + ".txt")) //zapisywania do pliku wyników
            {
                sw.WriteLine(lp+": f(" + funkcjanajwieksza + ")   " + wartoscnajwieksza);
                lp++;
            }

            System.Console.WriteLine("f(" + funkcjanajwieksza + ")      "+ wartoscnajwieksza); //wyświetlanie w konsoli wyników
        }
    }
}
