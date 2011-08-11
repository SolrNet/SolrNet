using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration {
  public class SolrNetContainerConfiguration {
    public IUnityContainer ConfigureContainer(SolrServers solrServers, IUnityContainer container) {
      container.RegisterType<IReadOnlyMappingManager, MemoizingMappingManager>(new InjectionConstructor(new ResolvedParameter(typeof (AttributesMappingManager))));
      container.RegisterType(typeof (ISolrDocumentActivator<>), typeof (SolrDocumentActivator<>));
      container.RegisterType(typeof (ISolrQueryExecuter<>), typeof (SolrQueryExecuter<>));
      container.RegisterType<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
      container.RegisterType<IMappingValidator, MappingValidator>();

      RegisterParsers(container);
      RegisterValidationRules(container);
      RegisterSerializers(container);

      AddCoresFromConfig(solrServers, container);

      return container;
    }

    private void RegisterValidationRules(IUnityContainer container) {
      var validationRules = new[] {
        typeof (MappedPropertiesIsInSolrSchemaRule),
        typeof (RequiredFieldsAreMappedRule),
        typeof (UniqueKeyMatchesMappingRule),
      };

      foreach (var validationRule in validationRules) {
        container.RegisterType(typeof (IValidationRule), validationRule);
      }
    }

    private void RegisterSerializers(IUnityContainer container) {
      container.RegisterType(typeof (ISolrDocumentSerializer<>), typeof (SolrDocumentSerializer<>));
      container.RegisterType(typeof (ISolrDocumentSerializer<Dictionary<string, object>>), typeof (SolrDictionarySerializer));
      container.RegisterType<ISolrFieldSerializer, DefaultFieldSerializer>();
      container.RegisterType<ISolrQuerySerializer, DefaultQuerySerializer>();
      container.RegisterType<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
    }

    private void RegisterParsers(IUnityContainer container) {
      container.RegisterType(typeof (ISolrDocumentResponseParser<>), typeof (SolrDocumentResponseParser<>));
      container.RegisterType<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();

      var parsers = new[] {
        typeof (ResultsResponseParser<>),
        typeof (HeaderResponseParser<>),
        typeof (FacetsResponseParser<>),
        typeof (HighlightingResponseParser<>),
        typeof (MoreLikeThisResponseParser<>),
        typeof (SpellCheckResponseParser<>),
        typeof (StatsResponseParser<>),
        typeof (CollapseResponseParser<>),
        typeof(GroupingResponseParser<>),
        typeof(ClusterResponseParser<>),
        typeof(TermsResponseParser<>)
      };

      foreach (var parser in parsers) {
        container.RegisterType(typeof (ISolrResponseParser<>), parser, parser.ToString());
      }

      container.RegisterType<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
      container.RegisterType<ISolrExtractResponseParser, ExtractResponseParser>();
      container.RegisterType(typeof (ISolrQueryResultParser<>), typeof (SolrQueryResultParser<>));
      container.RegisterType<ISolrFieldParser, DefaultFieldParser>();
      container.RegisterType<ISolrSchemaParser, SolrSchemaParser>();
      container.RegisterType<ISolrDIHStatusParser, SolrDIHStatusParser>();
    }

    private void RegisterCore(SolrCore core, IUnityContainer container) {
      var coreConnectionId = core.Id + typeof (SolrConnection);

      container.RegisterType<ISolrConnection, SolrConnection>(coreConnectionId, new InjectionConstructor(core.Url));

      var ISolrQueryExecuter = typeof (ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
      var SolrQueryExecuter = typeof (SolrQueryExecuter<>).MakeGenericType(core.DocumentType);

      container.RegisterType(
        ISolrQueryExecuter, SolrQueryExecuter,
        new InjectionConstructor(
          new ResolvedParameter(typeof (ISolrQueryResultParser<>).MakeGenericType(core.DocumentType)),
          new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
          new ResolvedParameter(typeof (ISolrQuerySerializer)),
          new ResolvedParameter(typeof (ISolrFacetQuerySerializer))));

      var ISolrBasicOperations = typeof (ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
      var ISolrBasicReadOnlyOperations = typeof (ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
      var SolrBasicServer = typeof (SolrBasicServer<>).MakeGenericType(core.DocumentType);

      container.RegisterType(
        ISolrBasicOperations, SolrBasicServer,
        new InjectionConstructor(
          new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
          new ResolvedParameter(ISolrQueryExecuter),
          new ResolvedParameter(typeof (ISolrDocumentSerializer<>).MakeGenericType(core.DocumentType)),
          new ResolvedParameter(typeof (ISolrSchemaParser)),
          new ResolvedParameter(typeof (ISolrHeaderResponseParser)),
          new ResolvedParameter(typeof (ISolrQuerySerializer)),
          new ResolvedParameter(typeof (ISolrDIHStatusParser)),
          new ResolvedParameter(typeof (ISolrExtractResponseParser))));

      container.RegisterType(
        ISolrBasicReadOnlyOperations, SolrBasicServer,
        new InjectionConstructor(
          new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
          new ResolvedParameter(ISolrQueryExecuter),
          new ResolvedParameter(typeof (ISolrDocumentSerializer<>).MakeGenericType(core.DocumentType)),
          new ResolvedParameter(typeof (ISolrSchemaParser)),
          new ResolvedParameter(typeof (ISolrHeaderResponseParser)),
          new ResolvedParameter(typeof (ISolrQuerySerializer)),
          new ResolvedParameter(typeof (ISolrDIHStatusParser)),
          new ResolvedParameter(typeof (ISolrExtractResponseParser))));

      var ISolrOperations = typeof (ISolrOperations<>).MakeGenericType(core.DocumentType);
      var SolrServer = typeof (SolrServer<>).MakeGenericType(core.DocumentType);

      container.RegisterType(
        ISolrOperations, SolrServer,
        new InjectionConstructor(
          new ResolvedParameter(ISolrBasicOperations),
          new ResolvedParameter(typeof (IReadOnlyMappingManager)),
          new ResolvedParameter(typeof (IMappingValidator))));

      var ISolrReadOnlyOperations = typeof (ISolrReadOnlyOperations<>).MakeGenericType(core.DocumentType);

      container.RegisterType(
        ISolrReadOnlyOperations, SolrServer,
        new InjectionConstructor(
          new ResolvedParameter(ISolrBasicOperations),
          new ResolvedParameter(typeof (IReadOnlyMappingManager)),
          new ResolvedParameter(typeof (IMappingValidator))));
    }

    private void AddCoresFromConfig(SolrServers solrServers, IUnityContainer container) {
      if (solrServers == null) {
        return;
      }

      var cores =
        from server in solrServers.Cast<SolrServerElement>()
        select GetCoreFrom(server);

      foreach (var core in cores) {
        RegisterCore(core, container);
      }
    }

    private static SolrCore GetCoreFrom(SolrServerElement server) {
      var id = server.Id ?? Guid.NewGuid().ToString();
      var documentType = GetCoreDocumentType(server);
      var coreUrl = GetCoreUrl(server);
      ValidateUrl(coreUrl);
      return new SolrCore(id, documentType, coreUrl);
    }

    private static string GetCoreUrl(SolrServerElement server) {
      var url = server.Url;
      if (string.IsNullOrEmpty(url)) {
        throw new ConfigurationErrorsException("Core url missing in SolrNet core configuration");
      }
      return url;
    }

    private static Type GetCoreDocumentType(SolrServerElement server) {
      var documentType = server.DocumentType;

      if (string.IsNullOrEmpty(documentType)) {
        throw new ConfigurationErrorsException("Document type missing in SolrNet core configuration");
      }

      Type type;

      try {
        type = Type.GetType(documentType);
      } catch (Exception e) {
        throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType), e);
      }

      if (type == null)
        throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType));

      return type;
    }

    private static void ValidateUrl(string url) {
      try {
        var uri = new Uri(url);
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) {
          throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
        }
      } catch (ArgumentException e) {
        throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
      } catch (UriFormatException e) {
        throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
      }
    }
  }
}