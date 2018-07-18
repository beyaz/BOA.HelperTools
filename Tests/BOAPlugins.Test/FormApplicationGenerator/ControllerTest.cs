﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.FormApplicationGenerator
{
    [TestClass]
    public class ControllerTest
    {
        #region Public Methods
        [TestMethod]
        public void CreateModel()
        {
            var solutionPath = @"D:\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\BOA.CardGeneral.DebitCard.sln";

            var controller = new Controller();

            var model = controller.CreateModel(solutionPath, "BOAReverseQueue");

            model.FormDataClassFields.Add(new FieldInfo
            {
                TypeName = DotNetTypeName.Decimal,
                Name     = "BusinessKey"
            });

            model.FormDataClassFields.Add(new FieldInfo
            {
                TypeName = DotNetTypeName.Decimal,
                Name     = "LastBusinessKey"
            });

            model.FormDataClassFields.Add(new FieldInfo
            {
                TypeName = DotNetTypeName.String,
                Name     = "ErrorMessage"
            });

            Assert.AreEqual("CardGeneral.DebitCard", model.NamespaceName);

            controller.ExportFiles();
        }
        #endregion
    }
}