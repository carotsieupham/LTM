using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Giai phuong trinh ax + b = 0");
        Console.Write("Nhap gia tri a: ");
        if (double.TryParse(Console.ReadLine(), out double a))
        {
            Console.Write("Nhap gia tri b: ");
            if (double.TryParse(Console.ReadLine(), out double b))
            {
                GiaiPhuongTrinhBacNhat(a, b);
            }
            else
            {
                Console.WriteLine("Gia tri b khong hop le.");
            }
        }
        else
        {
            Console.WriteLine("Gia tri a khong hop le.");
        }
    }

    static void GiaiPhuongTrinhBacNhat(double a, double b)
    {
        if (a == 0)
        {
            if (b == 0)
            {
                Console.WriteLine("Phuong trinh vo so nghiem.");
            }
            else
            {
                Console.WriteLine("Phuong trinh vo nghiem.");
            }
        }
        else
        {
            double x = -b / a;
            Console.WriteLine("Nghiem cua phuong trinh x = " + x);
        }
    }
}