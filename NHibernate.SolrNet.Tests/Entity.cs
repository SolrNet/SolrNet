using System.Collections.Generic;

namespace NHibernate.SolrNet.Tests {
    public class Entity {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<string> Tags { get; set; }
    }
}