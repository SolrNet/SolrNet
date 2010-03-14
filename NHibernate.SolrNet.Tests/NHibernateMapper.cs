#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.SolrNet.Tests {
    /// <summary>
    /// From http://code.google.com/p/unhaddins/source/browse/HunabKu/src/MappingSource/MappingSource/
    /// </summary>
    public class Configuration : Cfg.Configuration {
        public void Register(System.Type entity) {
            var dialect = Dialect.Dialect.GetDialect(Properties);
            var mappings = CreateMappings(dialect);
            SetDefaultMappingsProperties(mappings);
            new EntityMapper(mappings, dialect).Bind(entity);
        }

        private static void SetDefaultMappingsProperties(Mappings mappings) {
            mappings.SchemaName = null;
            mappings.DefaultCascade = "none";
            mappings.DefaultAccess = "property";
            mappings.DefaultLazy = true;
            mappings.IsAutoImport = true;
            mappings.DefaultNamespace = null;
            mappings.DefaultAssembly = null;
        }
    }

    public struct DefaultValues {
        public const string Accessor = "property";
        public const string PoIDBaseName = "id";
        public const string PoIDGenerator = "native";
    }

    /// <summary>
    /// Only Root classes
    /// </summary>
    public class EntityMapper {
        private readonly Dialect.Dialect dialect;
        private readonly Mappings mappings;

        public EntityMapper(Mappings mappings, Dialect.Dialect dialect) {
            this.mappings = mappings;
            this.dialect = dialect;
        }

        public Mappings Mappings {
            get { return mappings; }
        }

        public Dialect.Dialect Dialect {
            get { return dialect; }
        }

        public void Bind(System.Type entity) {
            var rootClass = new RootClass();
            BindClass(entity, rootClass);
        }

        private void BindClass(System.Type entity, PersistentClass pclass) {
            pclass.IsLazy = true;
            pclass.EntityName = entity.FullName;
            pclass.ClassName = entity.AssemblyQualifiedName;
            pclass.ProxyInterfaceName = entity.AssemblyQualifiedName;
            string tableName = GetClassTableName(pclass);
            Table table = mappings.AddTable(null, null, tableName, null, pclass.IsAbstract.GetValueOrDefault(), null);
            ((ITableOwner) pclass).Table = table;
            pclass.IsMutable = true;
            PropertyInfo[] propInfos = entity.GetProperties();

            PropertyInfo toExclude = new IdBinder(this, propInfos).Bind(pclass, table);

            pclass.CreatePrimaryKey(dialect);
            BindProperties(pclass, propInfos.Where(x => x != toExclude));
            mappings.AddClass(pclass);

            string qualifiedName = pclass.MappedClass == null ? pclass.EntityName : pclass.MappedClass.AssemblyQualifiedName;
            mappings.AddImport(qualifiedName, pclass.EntityName);
            if (mappings.IsAutoImport && pclass.EntityName.IndexOf('.') > 0) {
                mappings.AddImport(qualifiedName, StringHelper.Unqualify(pclass.EntityName));
            }
        }

        private void BindProperties(PersistentClass pclass, IEnumerable<PropertyInfo> infos) {
            var table = pclass.Table;

            foreach (var propertyInfo in infos) {
                if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string))
                    continue;
                // Bind a simple value property
                IValue value = new SimpleValue(table);
                BindColumn((SimpleValue) value, true, propertyInfo.Name);

                Property property = CreateProperty(value, propertyInfo, pclass.ClassName);
                //	property.IsUpdateable = false;
                pclass.AddProperty(property);
            }
        }

        private static Property CreateProperty(IValue value, PropertyInfo info, string className) {
            if (!string.IsNullOrEmpty(className) && value.IsSimpleValue) {
                value.SetTypeUsingReflection(className, info.Name, DefaultValues.Accessor);
            }
            return new Property {Name = info.Name, Value = value};
        }

        private void BindColumn(SimpleValue value, bool nullable, string propName) {
            var col = new Column {Value = value, IsNullable = nullable, Name = mappings.NamingStrategy.ColumnName(propName)};
            value.Table.AddColumn(col);
            value.AddColumn(col);

            value.AddColumn(col);
        }

        private string GetClassTableName(PersistentClass pclass) {
            return mappings.NamingStrategy.ClassToTableName(pclass.EntityName);
        }
    }

    public class IdBinder {
        private readonly EntityMapper mapper;
        private readonly PropertyInfo[] propInfos;

        public IdBinder(EntityMapper mapper, PropertyInfo[] propInfos) {
            this.mapper = mapper;
            this.propInfos = propInfos;
        }

        public PropertyInfo Bind(PersistentClass pclass, Table table) {
            var identifier = new SimpleValue(table);
            pclass.Identifier = identifier;
            PropertyInfo result = GetIdProperty();
            string propName = result.Name;
            AddColumn(identifier, propName);
            CreateIdentifierProperty(pclass, identifier, propName);
            BindGenerator(identifier);
            identifier.Table.SetIdentifierValue(identifier);
            return result;
        }

        private static void BindGenerator(SimpleValue identifier) {
            identifier.IdentifierGeneratorStrategy = DefaultValues.PoIDGenerator;
        }

        private void CreateIdentifierProperty(PersistentClass pclass, IValue identifier, string propName) {
            identifier.SetTypeUsingReflection(pclass.MappedClass == null ? null : pclass.MappedClass.AssemblyQualifiedName,
                                              propName, DefaultValues.Accessor);

            var property = new Property(identifier) {
                Name = propName,
                PropertyAccessorName = DefaultValues.Accessor,
                Cascade = mapper.Mappings.DefaultCascade,
                IsUpdateable = true,
                IsInsertable = true,
                IsOptimisticLocked = true,
                Generation = PropertyGeneration.Never
            };

            pclass.IdentifierProperty = property;
        }

        public PropertyInfo GetIdProperty() {
            PropertyInfo result =
                propInfos.FirstOrDefault(x => DefaultValues.PoIDBaseName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
            if (result != null) {
                return result;
            }
            throw new Exception("Invalid name for POID property.");
        }

        private void AddColumn(SimpleValue id, string propName) {
            Column column = CreateColumn();
            column.Value = id;
            column.Name = mapper.Mappings.NamingStrategy.ColumnName(propName);

            if (id.Table != null) {
                id.Table.AddColumn(column);
            }

            id.AddColumn(column);
        }

        private static Column CreateColumn() {
            return new Column {IsNullable = false, IsUnique = false, CheckConstraint = string.Empty, SqlType = null};
        }
    }
}