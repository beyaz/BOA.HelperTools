using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace BOAPlugins.ViewClassDependency.Test
{
    [TestClass]
    public class GraphCreatorTest
    {

        [TestMethod]
        public void T()
        {
            var dir = @"D:\workde\BOA.BusinessModules\Dev\BOA.Card.CreditCardOperation\UI\BOA.UI.Card.CreditCardOperation\bin\Debug\";
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(dir);


            var dllPath = dir+ "BOA.UI.Card.CreditCardOperation.dll";

            var api = new GraphCreator();

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(dllPath,new ReaderParameters {AssemblyResolver = resolver });

            var typeDefinition = assemblyDefinition.FindType(type => type.FullName == "BOA.UI.Card.CreditCardOperation.LimitChangeForm.Model");

            api.CreateGraph(typeDefinition);


        }
    }
}
