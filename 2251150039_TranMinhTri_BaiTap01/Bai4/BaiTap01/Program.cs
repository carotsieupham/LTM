// See https://aka.ms/new-console-template for more information

using System;

namespace HelloWorld
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Nhap so nguyen duong  n: ");
            if (int.TryParse(Console.ReadLine(), out int n) && n > 0)
            {
                int sum = SumN(n);
                Console.WriteLine("Tong cua day so S({0}) là: {1}", n, sum);
            }
            else
            {
                Console.WriteLine("Vui long nhap so nguyen duong.");
            }
        }
        static int SumN(int n)
        {
            if(n==1)
            {
                return 1;
            }
            return  n+SumN(n -1 );
        }
    }
}
