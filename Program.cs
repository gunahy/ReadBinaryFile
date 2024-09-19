using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBinaryFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string binaryFilePath = @"C:\testProgram\students.dat";
            string outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");

            List<Students> StudentsW = new List<Students>
            {
                new Students { Name = "Иван Иванов", Group = "Группа A", DateOfBirth = new DateTime(2000, 5, 21), AverageScore = 4.5m },
                new Students { Name = "Ольга Петрова", Group = "Группа B", DateOfBirth = new DateTime(1999, 11, 12), AverageScore = 3.8m },
                new Students { Name = "Алексей Сидоров", Group = "Группа A", DateOfBirth = new DateTime(2001, 7, 15), AverageScore = 4.2m },
                new Students { Name = "Екатерина Смирнова", Group = "Группа C", DateOfBirth = new DateTime(2002, 2, 5), AverageScore = 4.9m },
                new Students { Name = "Дмитрий Попов", Group = "Группа B", DateOfBirth = new DateTime(2000, 9, 29), AverageScore = 3.6m }
            };

            WriteStudentsToBinaryFile(StudentsW, binaryFilePath);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            List<Students> StudentsR = ReadStudentsFromBinaryFile(binaryFilePath);
            var studentsByGroup = StudentsR.GroupBy(s => s.Group);

            foreach (var group in studentsByGroup)
            {
                string groupFile = Path.Combine(outputDirectory, group.Key + ".csv");
                using (StreamWriter writer = new StreamWriter(groupFile))
                {
                    writer.WriteLine("Фамилия Имя, Дата рождения, Средний балл");
                    foreach (var student in group)
                    {
                        string line = $"{student.Name}, {student.DateOfBirth:yyyy-MM-dd}, {student.AverageScore:F2}";
                        writer.WriteLine(line);
                    }
                }
            }

            Console.WriteLine("Данные успешно сохранены по группам.");
            Console.ReadKey();

        }

        static void WriteStudentsToBinaryFile(List<Students> students, string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                foreach (var student in students)
                {

                    writer.Write(student.Name);
                    writer.Write(student.Group);
                    writer.Write(student.AverageScore);
                    writer.Write(student.DateOfBirth.ToBinary());
                }
            }
        }

        static List<Students> ReadStudentsFromBinaryFile(string binaryFilePath)
        {
            List<Students> students = new List<Students> ();

            using (BinaryReader reader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    Students student = new Students();
                    student.Name = reader.ReadString();
                    student.Group = reader.ReadString();
                    student.AverageScore = reader.ReadDecimal();

                    long dateBinary = reader.ReadInt64();
                    Console.WriteLine(dateBinary);
                    student.DateOfBirth = DateTime.FromBinary(dateBinary);

                    students.Add(student);
                }
            }
            return students;
        }

    }
}
