using System;

class Program
{
    static void Main()
    {
        Console.Write("Nhap so thu nhat: ");
        if (double.TryParse(Console.ReadLine(), out double soThuNhat))
        {
            Console.Write("Nhap so thu hai: ");
            if (double.TryParse(Console.ReadLine(), out double soThuHai))
            {
                Console.Write("Nhap so thu ba: ");
                if (double.TryParse(Console.ReadLine(), out double soThuBa))
                {
                    double max = TimSoLonNhat(soThuNhat, soThuHai, soThuBa);
                    Console.WriteLine("So lon nhat la: " + max);
                }
                else
                {
                    Console.WriteLine("So thu ba khong hop le.");
                }
            }
            else
            {
                Console.WriteLine("So thu hai khong hop le.");
            }
        }
        else
        {
            Console.WriteLine("So thu nhat khong hop le.");
        }
    }

    static double TimSoLonNhat(double a, double b, double c)
    {
        double max = a;
        if (b > max)
        {
            max = b;
        }
        if (c > max)
        {
            max = c;
        }
        return max;
    }
}
