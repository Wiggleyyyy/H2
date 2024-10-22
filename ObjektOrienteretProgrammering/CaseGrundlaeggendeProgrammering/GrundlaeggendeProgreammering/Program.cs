using Plukliste;
using System.Xml.Serialization;
using System.Xml;
using System.Net.Http.Headers;

class PluklisteProgram
{
    static void Main()
    {
        //Arrange
        var index = -1;
        var standardColor = Console.ForegroundColor;
        Pluklist _pickList = new Pluklist();

        // check and create import dir
        Directory.CreateDirectory("import");

        // load files from dir
        List<string> files = LoadFiles("export");

        // ACT loop
        char readKey = ' '; // input key
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
                    var currentFile = files[index];
                    if (currentFile.ToLower().EndsWith(".xml"))
                    {
                        try
                        {
                            using (FileStream file = File.OpenRead(currentFile))
                            {
                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Pluklist));
                                var pickList = (Pluklist?)xmlSerializer.Deserialize(file);
                                _pickList = pickList; // set new var for accessibility later

                                // Call the method to print picklist
                                PrintPicklist(_pickList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing XML file: {ex.Message}");
                        }
                    }
                    else if (currentFile.ToLower().EndsWith(".csv"))
                    {
                        try
                        {
                            using (var reader = new StreamReader(currentFile))
                            {
                                List<Item> csvItems = new List<Item>();
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    var values = line.Split(','); // Assuming comma-separated values

                                    if (values.Length >= 4)
                                    {
                                        // Create Item from CSV row
                                        Item item = new Item
                                        {
                                            ProductID = values[0],
                                            Title = values[1],
                                            Type = (ItemType)Enum.Parse(typeof(ItemType), values[2], true), // Ensure proper parsing of enum
                                            Amount = int.Parse(values[3])
                                        };

                                        csvItems.Add(item);
                                    }
                                }

                                // Set _pickList and print
                                _pickList = new Pluklist
                                {
                                    Name = "CSV Picklist",  // Assign default name or handle CSV file metadata separately
                                    Shipment = "Unknown",   // You can extend CSV structure to include these
                                    Address = "Unknown",    // You can extend CSV structure to include these
                                    Lines = csvItems
                                };

                                // Call the method to print picklist
                                PrintPicklist(_pickList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing CSV file: {ex.Message}");
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
            Console.WriteLine("\n\nOptions:");
            PrintOption("Quit", standardColor);

            if (index >= 0)
            {
                PrintOption("Aflsut plukseddel", standardColor);
            }
            if (index > 0)
            {
                PrintOption("Forrige plukseddel", standardColor);
            }
            if (index < files.Count - 1)
            {
                PrintOption("Næste plukseddel", standardColor);
            }
            PrintOption("Genindlæs pluksedler", standardColor);
            if (_pickList.Lines.Any(x => x.Type == ItemType.Print)) // only display option if printable
            {
                PrintOption("Vejledning", standardColor);
            }

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
                        var fileWithoutPath = Path.GetFileName(files[index]);
                        var destinationPath = Path.Combine("import", fileWithoutPath);
                        File.Move(files[index], destinationPath);
                        Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                        files.RemoveAt(index);
                        if (index == files.Count) index--;
                        break;
                    case 'V':
                        var printItem = _pickList.Lines.FirstOrDefault(x => x.ProductID.StartsWith("PRINT-"));
                        if (printItem != null)
                        {
                            string outputFilePath = string.Empty;
                            string templateFilePath = string.Empty;
                            switch (printItem.ProductID)
                            {
                                case "PRINT-OPGRADE":
                                    templateFilePath = $@"Templates\PRINT-OPGRADE.html";
                                    outputFilePath = $@"Print\Print_Opgrade_{DateTime.Now:yyyyMMdd_HHmmSS}.html";
                                    break;
                                case "PRINT-OPSIGELSE":
                                    templateFilePath = $@"Templates\PRINT-OPSIGELSE.html";
                                    outputFilePath = $@"Print\Print_Opsigelse_{DateTime.Now:yyyyMMdd_HHmmSS}.html";
                                    break;
                                case "PRINT-WELCOME":
                                    templateFilePath = $@"Templates\PRINT-WELCOME.html";
                                    outputFilePath = $@"Print\Print_Welcome_{DateTime.Now:yyyyMMdd_HHmmSS}.html";
                                    break;
                            }

                            if (!String.IsNullOrEmpty(outputFilePath) && !String.IsNullOrEmpty(templateFilePath))
                            {
                                PrintPluklisteToHtml(_pickList, templateFilePath, outputFilePath);
                            }

                            Console.Clear();
                            Console.ForegroundColor = standardColor;
                            Console.WriteLine("Vejledning oprettet.\n\n");
                        }

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

    // Refactored method to print the picklist
    static void PrintPicklist(Pluklist pickList)
    {
        if (pickList != null && pickList.Lines != null)
        {
            Console.WriteLine("\n{0, -13}{1}", "Name:", pickList.Name);
            Console.WriteLine("{0, -13}{1}", "Forsendelse:", pickList.Shipment);

            Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
            foreach (var item in pickList.Lines)
            {
                Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
            }
        }
    }

    static void PrintPluklisteToHtml(Pluklist pickList, string templatePath, string outputFilePath)
    {
        try
        {
            string htmlContent = File.ReadAllText(templatePath);

            htmlContent = htmlContent.Replace("[Adresse]", pickList.Address ?? "[Ingen Adresse]");
            htmlContent = htmlContent.Replace("[Name]", pickList.Name ?? "[Intet navn]");
            if (htmlContent.Contains("[Plukliste]"))
            {
                System.Text.StringBuilder pickListStringBuilder = new System.Text.StringBuilder();
                pickListStringBuilder.AppendLine("<table>");
                pickListStringBuilder.AppendLine("<tr><th>Antal</th><th>Type</th><th>Produktnr.</th><th>Navn</th></tr>");
                foreach (var item in pickList.Lines)
                {
                    pickListStringBuilder.AppendLine($"<tr><td>{item.Amount}</td><td>{item.Type}</td><td>{item.ProductID}</td><td>{item.Title}</td></tr>");
                }
                pickListStringBuilder.AppendLine("</table>");
                htmlContent = htmlContent.Replace("[Plukliste]", pickListStringBuilder.ToString());
            }

            File.WriteAllText(outputFilePath, htmlContent);
            Console.WriteLine($"HTML file successfully generated: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red; //status red
            Console.WriteLine($"Der skete en fejl ved indlæsning af template: {ex.Message}");
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

    static void PrintOption(string word, ConsoleColor standardColor)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(word[0]);
        Console.ForegroundColor = standardColor;
        Console.WriteLine(word.Substring(1));
    }
}
