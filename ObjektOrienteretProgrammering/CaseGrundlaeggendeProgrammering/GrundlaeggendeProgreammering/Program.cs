using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Plukliste;

class PluklisteProgram
{
    static void Main()
    {
        //Arrange
        char readKey = ' '; // input key
        List<string> files;
        var index = -1;
        var standardColor = Console.ForegroundColor;

        // check and create import dir
        Directory.CreateDirectory("import");

        // load files from dir
        files = LoadFiles("export");

        // ACT loop
        while (readKey != 'Q')
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
                break;
            }
            else
            {
                if (index == -1) index = 0;

                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"\nfile: {files[index]}");

                try
                {
                    using (FileStream file = File.OpenRead(files[index]))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Pluklist));
                        var pickList = (Pluklist?)xmlSerializer.Deserialize(file);

                        // print picklist
                        if (pickList != null && pickList.Lines != null)
                        {
                            Console.WriteLine("\n{0, -13}{1}", "Name:", pickList.Name);
                            Console.WriteLine("{0, -13}{1}", "Forsendelse:", pickList.Shipment);
                            //TODO: Add adresse to screen print

                            Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                            foreach (var item in pickList.Lines)
                            {
                                Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                            }
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"File not found: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Permission denied: {ex.Message}");
                }
                catch (XmlException ex)
                {
                    Console.WriteLine($"Error reading data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
            }

            // print options
            PrintOptions(index, files.Count, standardColor);

            readKey = Console.ReadKey().KeyChar;
            if (readKey >= 'a') readKey -= (char)('a' - 'A'); //HACK: To upper

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red; // status in red

            try
            {
                switch (readKey)
                {
                    case 'G':
                        files = LoadFiles("export");
                        index = -1;
                        Console.WriteLine("Pluklister genindlæst");
                        break;
                    case 'F':
                        if (index > 0) index--;
                        break;
                    case 'N':
                        if (index < files.Count - 1) index++;
                        break;
                    case 'A':
                        // move files to import dir
                        var fileWithoutPath = Path.GetFileName(files[index]);
                        var destinationPath = Path.Combine("import", fileWithoutPath);
                        File.Move(files[index], destinationPath);
                        Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                        files.RemoveAt(index);
                        if (index == files.Count) index--;
                        break;
                    case 'V':
                        //generate html file
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                continue; // skip after error
            }
            finally
            {
                Console.ForegroundColor = standardColor; // reset color
            }
        }
    }

    // load filed from dir
    static List<string> LoadFiles(string directory)
    {
        try
        {
            return Directory.EnumerateFiles(directory).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading files from {directory}: {ex.Message}");
            return new List<string>();
        }
    }

    //print options
    static void PrintOptions(int index, int fileCount, ConsoleColor standardColor)
    {
        Console.WriteLine("\n\nOptions:");
        PrintOptionsItem("Quit", standardColor);

        if (index >= 0)
        {
            PrintOptionsItem("Aflsut plukseddel", standardColor);
        }
        if (index > 0)
        {
            PrintOptionsItem("Forrige plukseddel", standardColor);
        }
        if (index < fileCount - 1)
        {
            PrintOptionsItem("Næste plukseddel", standardColor);
        }
        PrintOptionsItem("Genindlæs pluksedler", standardColor);
        PrintOptionsItem("Vejledning", standardColor);
    }

    static void PrintOptionsItem(string word, ConsoleColor standardColor)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(word[0]);
        Console.ForegroundColor = standardColor;
        Console.WriteLine(word.Substring(1));
    }
}
