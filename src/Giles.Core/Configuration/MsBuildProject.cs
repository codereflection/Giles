using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Giles.Core.Configuration
{
    public class MsBuildProject
    {
        private readonly XmlDocument document;

        public MsBuildProject(XmlDocument document)
        {
            this.document = document;
        }

        public static MsBuildProject Load(Stream projectFile)
        {
            var document = new XmlDocument();
            document.Load(projectFile);

            return new MsBuildProject(document);
        }

        public IEnumerable<string> GetLocalAssemblyRefs()
        {
            return (from XmlNode itemGroup in ProjectNode.ChildNodes
                    where itemGroup.Name == "ItemGroup"
                    from XmlNode refNode in itemGroup.ChildNodes
                    where refNode.Name == "Reference"
                    from XmlNode hintNode in refNode.ChildNodes
                    where hintNode.Name == "HintPath"
                    select hintNode.InnerText).ToArray();
        }

        public string GetAssemblyName()
        {
            var platformConfig = GetDefaultPlatformConfig();
            return GetPropertyValue(platformConfig, "AssemblyName");
        }

        /// <summary>
        /// Gets the output assembly file path relative to the
        /// project files directory.  Uses the project default
        /// plaftorm configuration.
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyFilePath(string projectFilePath)
        {
            var platformConfig = GetDefaultPlatformConfig();
            var dir = GetPropertyValue(platformConfig, "OutputPath");
            var outtype = GetPropertyValue(platformConfig, "OutputType");

            // FIXME: DLL or EXE
            var assemblyName = GetPropertyValue(platformConfig, "AssemblyName");
            switch(outtype.ToUpper())
            {
                case "LIBRARY":
                    assemblyName += ".dll";
                    break;
                case "EXE":
                    assemblyName += ".exe";
                    break;
            }

            var projectPath = Path.GetDirectoryName(projectFilePath);
            if (projectPath == null)
                throw new ArgumentException("Invalid project file path", "projectFilePath");

            return Path.Combine (projectPath, Path.Combine(dir, assemblyName));
        }

        public string GetPropertyValue(string platformConfig, string property)
        {
            var condition = "'" + platformConfig + "'";
            var specific = (
                from XmlNode propertyGroup in ProjectNode.ChildNodes
                where propertyGroup.Name == "PropertyGroup" &&
                      propertyGroup.Attributes != null &&
                      propertyGroup.Attributes["Condition"] != null &&
                      propertyGroup.Attributes["Condition"].InnerText.Contains(condition)

                from XmlNode outputNode in propertyGroup.ChildNodes
                where outputNode.Name == property
                select outputNode.InnerText).ToArray();

            var defaultConfig = (
                from XmlNode propertyGroup in ProjectNode.ChildNodes
                where propertyGroup.Name == "PropertyGroup" && (
                      propertyGroup.Attributes == null ||
                      propertyGroup.Attributes["Condition"] == null)

                from XmlNode outputNode in propertyGroup.ChildNodes
                where outputNode.Name == property
                select outputNode.InnerText).ToArray();

            switch (specific.Length)
            {
                case 1:
                    return specific[0];
                case 0:
                    switch (defaultConfig.Length)
                    {
                        case 1:
                            return defaultConfig[0];
                        case 0:
                            throw new InvalidOperationException(
                                string.Format("No value found for property '{0}' using "
                                    + "platform configuration '{1}'", property, platformConfig));
                    }
                    break;
            }
            throw new InvalidOperationException(
                string.Format("The property '{0}' had multiple values "
                    + "applicable for the same platform configuration '{1}'",
                    property, platformConfig));
        }

        public string GetDefaultPlatformConfig()
        {
            var config = GetDefaultPropertyValue("Configuration");
            var platform = GetDefaultPropertyValue("Platform");
            return config + "|" + platform;
        }

        private string GetDefaultPropertyValue(string property)
        {
            return (from XmlNode propertyGroup in ProjectNode.ChildNodes
                    where propertyGroup.Name == "PropertyGroup" &&
                        (propertyGroup.Attributes == null ||
                         propertyGroup.Attributes["Condition"] == null)
                    from XmlNode configNode in propertyGroup.ChildNodes
                    where configNode.Name == property &&
                        configNode.Attributes != null &&
                        configNode.Attributes["Condition"] != null &&
                        configNode.Attributes["Condition"].InnerText.Contains("'$(" + property + ")' == ''")
                    select configNode.InnerText).Single();
        }

        private XmlNode ProjectNode
        {
            get { return document.FirstChild.NextSibling; }
        }
    }
}