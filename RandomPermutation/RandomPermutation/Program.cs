using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomPermutation
{
    class Program
    {
        const int n = 100;
        static int[] array = new int[n];
        static int[] array2 = new int[n];
        static Dictionary<int,int> histogram = new Dictionary<int,int>(n); 
        static int[,] matrix = new int[n, n];

        static void printArray(int[] a)
        {
            foreach(var e in a)
            {
                Console.Write("{0}\t",e); 
            }
        }

        static void initArray(int[] a)
        {
            for(int i=0; i<n;i++)
            {
                a[i] = i; 
            }
        }

        static void initArray(int[,] a)
        {
            for (int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                a[i,j] = i+j;
            }
        }

        static void permutation(int[] a)
        {
            Random random = new Random();

            int x, z; 

            for(int i=1; i<n; i++)
            {
                x = random.Next(i);
                z = a[x];
                a[x] = a[i];
                a[i] = z; 
            }
        }

        static void parallelPermutation(int[] a)
        {
            var localRandom = new ThreadLocal<Random>(() => new Random());

            Parallel.For(1, n, i =>
            {
                int x = localRandom.Value.Next(i);
                int z = a[x];
                a[x] = a[i];
                a[i] = z; 
            }); 

        }

        static bool exists(int x, int[] a)
        {
            bool result = false; 

            Parallel.For(0, n, (i, state) =>
            {
                if (a[i] == x)
                {
                    result = true;
                    state.Break(); 
                }
            });

            return result; 
        }

        static bool exists(int x, int[,] a)
        {
            bool result = false; 

            Parallel.For(0, n, (i, state) =>
            {                
                for(int j=0; j<n; j++)
                {
                    if(a[i,j]==x)
                    {
                        result = true;
                        break; 
                    }
                }

                if (result == true) state.Break(); 
            });

            return result;
        }

        static void makeHistogram(int[] a)
        {
            Parallel.For(0, n, i =>
            {
                histogram[a[i]] = 0;
            });

            Parallel.For(0, n, i =>
            {
                histogram[a[i]]++;
            }); 
        }

        static void makeHistogram(int[,] a)
        {
            Parallel.For(0, n, i =>
            {
                for (int j = 0; j < n; j++)
                {
                    histogram[a[i, j]] = 0;
                }
            });

            Parallel.For(0, n, i =>
            {
                for(int j=0; j<n; j++)
                {
                    histogram[a[i, j]]++;
                }
            });
        }

        static bool checkArray(int[] a)
        {
            makeHistogram(a); 

            for (int i = 0; i < n; i++)
            {
                if (histogram[i] != 1) return false; 
            }

            return true; 
        }

        static int scalarProduct(int[] a, int[] b)
        {
            int result = 0;

            Parallel.For(0, n, i =>
            {
                result += a[i] * b[i]; 
            }); 

            return result; 
        }

        static void Main(string[] args)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();

            initArray(array);
            initArray(array2);
            initArray(matrix);

            //zad 2.4-----------------------------------------------------
            Console.WriteLine("Histogram of array [value,quantity]: "); 
            makeHistogram(array);
            foreach (var e in histogram) { Console.WriteLine(e); }
            Console.WriteLine("\nHistogram of matrix [value,quantity]: ");
            makeHistogram(matrix);
            foreach (var e in histogram) { Console.WriteLine(e); }
            //zad 2.5-----------------------------------------------------
            Console.WriteLine("\nExists in array (1): ");
            Console.WriteLine(exists(1, array));
            Console.WriteLine("Exists in array (-1): ");
            Console.WriteLine(exists(-1, array));
            Console.WriteLine("Exists in matrix (1): ");
            Console.WriteLine(exists(2, matrix));
            Console.WriteLine("Exists in matrix (-1): ");
            Console.WriteLine(exists(-1, matrix));
            //zad 2.6-----------------------------------------------------
            Console.WriteLine("\nScalar Product: "); 
            Console.WriteLine(scalarProduct(array, array2));
            //zad 2.7-----------------------------------------------------
            stopwatch.Start();  //pomiar czasu 
            parallelPermutation(array);
            stopwatch.Stop();
            Console.WriteLine("\nParallel permutation execution time [ms]: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Restart(); 
            permutation(array2);
            stopwatch.Stop();
            Console.WriteLine("\nSequential permutation execution time [ms]: ");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
