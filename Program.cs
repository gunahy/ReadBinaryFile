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
                new Students { Name = "Иван Иванов", Group = "Группа A", DateOfBirth = new DateTime(2000, 5, 21), AverageScore = 4.5M },
                new Students { Name = "Ольга Петрова", Group = "Группа B", DateOfBirth = new DateTime(1999, 11, 12), AverageScore = 3.8M },
                new Students { Name = "Алексей Сидоров", Group = "Группа A", DateOfBirth = new DateTime(2001, 7, 15), AverageScore = 4.2M },
                new Students { Name = "Екатерина Смирнова", Group = "Группа C", DateOfBirth = new DateTime(2002, 2, 5), AverageScore = 4.9M },
                new Students { Name = "Дмитрий Попов", Group = "Группа B", DateOfBirth = new DateTime(2000, 9, 29), AverageScore = 3.6M }
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
                string groupFile = Path.Combine(outputDirectory, group.Key + ".txt");
                using (StreamWriter writer = new StreamWriter(groupFile))
                {
                    foreach (var student in group)
                    {
                        string line = $"{student.Name}, {student.DateOfBirth:yyy-MM-dd},"; //{student.AverageScore:F2}";
                        writer.WriteLine(line);
                    }
                }
            }

            Console.WriteLine("Данные успешно сохранены по группам.");

        }

        static void WriteStudentsToBinaryFile(List<Students> students, string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                foreach (var student in students)
                {
                    WriteString(writer, student.Name);
                    WriteString(writer, student.Group);          
                    writer.Write(student.DateOfBirth.ToBinary()); 
                    writer.Write(student.AverageScore);          
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
                    student.Name = ReadString(reader);
                    student.Group = ReadString(reader);

                    long dateBinary = reader.ReadInt64();
                    student.DateOfBirth = ReadDateTime(reader);

                    //student.AverageScore = reader.ReadDecimal();

                    students.Add(student);
                }
            }
            return students;
        }

        static void WriteString(BinaryWriter writer, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes.Length);  
            writer.Write(bytes);         
        }

        static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        static DateTime ReadDateTime(BinaryReader reader)
        {
            long dateBinary = reader.ReadInt64();

            // Проверка, что значение находится в допустимом диапазоне Ticks
            if (dateBinary >= DateTime.MinValue.Ticks && dateBinary <= DateTime.MaxValue.Ticks)
            {
                return DateTime.FromBinary(dateBinary);
            }
            else
            {
                throw new InvalidDataException($"Некорректное значение даты: {dateBinary}. Оно должно быть между {DateTime.MinValue.Ticks} и {DateTime.MaxValue.Ticks}.");
            }
        }

    }
}
