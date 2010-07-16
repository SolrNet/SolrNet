namespace SolrNet.Impl
{
    #region Using Directives

    using System.Xml;
    using SolrNet.Commands;

    #endregion

    public class SolrCoreAdmin : ISolrCoreAdmin
    {
        private readonly ISolrConnection connection;

        public SolrCoreAdmin(ISolrConnection connection)
        {
            this.connection = connection;
        }

        public void Alias(string coreName, string otherName)
        {
            var response = this.Send(new AliasCommand(coreName, otherName));
        }

        public void Create(string coreName, string instanceDir, string configFile, string schemaFile, string dataDir)
        {
            var response = this.Send(new CreateCommand(coreName, instanceDir, configFile, schemaFile, dataDir));
        }

        public void Reload(string coreName)
        {
            var response = this.Send(new ReloadCommand(coreName));
        }

        public void Rename(string coreName, string otherName)
        {
            var response = this.Send(new RenameCommand(coreName, otherName));
        }

        public void Status()
        {
            var response = this.Send(new StatusCommand());
            var xml = new XmlDocument();
            xml.LoadXml(response);
        }

        public void Status(string coreName)
        {
            var response = this.Send(new StatusCommand(coreName));
            var xml = new XmlDocument();
            xml.LoadXml(response);
        }

        public void Swap(string coreName, string otherName)
        {
            var response = this.Send(new SwapCommand(coreName, otherName));
        }

        public void Unload(string coreName)
        {
            var response = this.Send(new UnloadCommand(coreName));
        }

        private string Send(ISolrCommand cmd)
        {
            return cmd.Execute(this.connection);
        }
    }
}