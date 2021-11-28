
using SplittingComponents;


string configFile = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json";

var state = new StateController(configFile);

await state.RetrieveMeta();

await state.RetrievePdfs();

await state.WriteMetaDataReport();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");