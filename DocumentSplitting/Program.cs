
using SplittingComponents;


string configFile = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json";

var state = new StateController(configFile);

//await state.RetrieveMeta();

await state.RetrievePdfs();

//state.WriteMetaDataReport();

state.WritePdfDataReport();

//state.ConvertPdfsToHtml();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");