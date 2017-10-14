using MyAdjacencyList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CS_B04
{
    class Program
    {
        /// <summary>
        /// Object Adjacency List is reference type so we need to deep copy it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T other)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        static AdjacencyList ReadAdjacencyList(string filepath)
        {
            using (var sr = new StreamReader(filepath))
            {
                // Allocations
                var list = new AdjacencyList
                {
                    Vertices = int.Parse(sr.ReadLine())
                };
                list.List = new LinkedList<int>[list.Vertices];

                // Read matrix
                for (int i = 0; i < list.Vertices; i++)
                {
                    var s = sr.ReadLine().Split(' ');

                    var tmp = new LinkedList<int>();
                    foreach (string e in s)
                    {
                        // catch the error if line content is not integers
                        if (int.TryParse(e, out int tmp2) == false)
                            if (tmp2 == 0)
                                continue;
                        tmp.AddLast(--tmp2);
                    }

                    list.List[i] = tmp;
                }

                return list;
            }
        }

        static LinkedList<int> SearchConnectedComponent(AdjacencyList list, int source, ref bool[] checkpoint)
        {
            var q = new Queue<int>();

            q.Enqueue(source);
            checkpoint[source] = true;
            var tmp = new LinkedList<int>();
            tmp.AddLast(source);
            while (q.Count != 0)
            {
                source = q.Dequeue();
                foreach (var item in list.List[source])
                {
                    if (checkpoint[item])
                        continue;
                    checkpoint[item] = true;
                    tmp.AddLast(item);
                    q.Enqueue(item);
                }
            }
            return tmp;
        }

        static List<LinkedList<int>> FindConnectedComponent(AdjacencyList list)
        {
            var q = new Queue<int>();
            var checkpoint = new bool[list.Vertices];
            var result = new List<LinkedList<int>>();

            for (int i = 0; i < checkpoint.Length; i++)
            {
                if (checkpoint[i])
                    continue;
                result.Add(SearchConnectedComponent(list, i, ref checkpoint));
            }

            return result;
        }

        /// <summary>
        /// Check bridges
        /// </summary>
        /// <param name="list"></param>
        static void Process1(AdjacencyList list)
        {
            int n = FindConnectedComponent(list).Count();
            
            for (int i = 0; i < list.List.Length; i++)
            {
                foreach (var item in list.List[i])
                {
                    var tmp = DeepCopy(list);

                    // 1. Remove the edge
                    tmp.List[i].Remove(item);
                    tmp.List[item].Remove(i);

                    // 2. Counting connected components
                    int m = FindConnectedComponent(tmp).Count();

                    // 3. Comparing and printing result
                    if (m > n)
                        WriteLine($"[{i}, {item}] is a brigde");
                    else
                        WriteLine($"[{i}, {item}] is not a brigde");
                }
            }

            WriteLine();
        }

        /// <summary>
        /// Check articulation points
        /// </summary>
        /// <param name="list"></param>
        static void Process2(AdjacencyList list)
        {
            int n = FindConnectedComponent(list).Count();

            for (int i = 0; i < list.List.Length; i++)
            {
                var tmp = DeepCopy(list);

                // 1. Remove the vertex
                tmp.List[i].Clear();

                // 2. Remove the edges that connect to the vertex
                for (int j = 0; j < tmp.Vertices; j++)
                {
                    tmp.List[j].Remove(i);
                }
                
                // 3. Decrease 1: not counting the connected-component of removed-vertex
                int m = FindConnectedComponent(tmp).Count() - 1;

                // Compare and print result
                if (m > n)
                    WriteLine($"Vertex[{i}] is a articulation point");
                else
                    WriteLine($"Vertex[{i}] is not a articulation point");
            }

            WriteLine();
        }
        



        static void CheckEdge(AdjacencyList list, Tuple<int, int, int> input, ref StringBuilder sb)
        {
            var n = FindConnectedComponent(list).Count;
            var x = input.Item1;
            var y = input.Item2;

            list.List[x].Remove(y);
            list.List[y].Remove(x);

            var m = FindConnectedComponent(list).Count;

            if(m > n)
            {
                WriteLine($"[{x + 1}, {y + 1}] is a bridge.");
                sb.Append("La canh cau.");
            }
            else
            {
                WriteLine($"[{x + 1}, {y + 1}] is not a bridge.");
                sb.Append("Khong la canh cau.");
            }
        }
        static void CheckVertex(AdjacencyList list, Tuple<int,int,int> input, ref StringBuilder sb)
        {
            var n = FindConnectedComponent(list).Count();
            var z = input.Item3;

            list.List[z].Clear();
            for (int i = 0; i < list.List.Length; i++)
            {
                list.List[i].Remove(z);
            }

            var m = FindConnectedComponent(list).Count;

            if (m > n)
            {
                WriteLine($"[{z}] is a articulation point.");
                sb.Append("La dinh khop.");
            }
            else
            {
                WriteLine($"[{z}] is not a articulation point.");
                sb.Append("Khong la dinh khop.");
            }
        }

        static Tuple<int,int, int> Input()
        {
            Write("Source x: "); var x = int.Parse(ReadLine()) - 1;
            Write("Target y: "); var y = int.Parse(ReadLine()) - 1;
            Write("Vertex z: "); var z = int.Parse(ReadLine()) - 1;

            return new Tuple<int, int, int>(x, y, z);
        }

        static void Problem(AdjacencyList list)
        {
            var input = Input();
            var sb = new StringBuilder();
            CheckEdge(list, input, ref sb);
            sb.AppendLine();
            CheckVertex(list, input, ref sb);

            WriteResult(sb);
        }

        static void WriteResult(StringBuilder sb)
        {
            using (var sw = new StreamWriter("..//..//lietkelienthongbfs.out"))
            {
                sw.Write(sb.ToString());
            }
        }

        static void Main(string[] args)
        {

            var list = new AdjacencyList();
            list = ReadAdjacencyList("..//..//dsk.txt");

            Problem(list);
            ReadLine();
        }
    }
}
