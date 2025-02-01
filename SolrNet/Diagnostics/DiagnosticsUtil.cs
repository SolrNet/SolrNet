using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using HttpWebAdapters;

namespace SolrNet.Diagnostics
{
    internal static class DiagnosticsUtil
    {
        private static readonly string SolrNetVersion = typeof(DiagnosticsUtil).Assembly.GetName().Version?.ToString();

        private static readonly Lazy<ActivitySource> ActivitySource =
            new Lazy<ActivitySource>(() => new ActivitySource(DiagnosticHeaders.DefaultSourceName, SolrNetVersion));

        public static Activity StartSolrActivity(ISolrCommand cmd)
        {
            if (!ActivitySource.Value.HasListeners()) return null;
            var commandName = Regex.Replace(cmd.GetType().Name, "Command(`\\d*){0,1}", "").ToLowerInvariant();
            var activity = ActivitySource.Value.StartActivity(commandName, ActivityKind.Client);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbOperationName, commandName);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbSystem, "solr");
            return activity;
        }
        
        public static Activity StartSolrActivity(ISolrQuery _)
        {
            if (!ActivitySource.Value.HasListeners()) return null;
            var activity = ActivitySource.Value.StartActivity("query", ActivityKind.Client);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbOperationName, "query");
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbSystem, "solr");
            return activity;
        }
        
        public static Activity StartSolrActivity(SolrMLTQuery _)
        {
            if (!ActivitySource.Value.HasListeners()) return null;
            var activity = ActivitySource.Value.StartActivity("mlt", ActivityKind.Client);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbOperationName, "mlt");
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbSystem, "solr");
            return activity;
        }
        
        public static Activity StartSolrActivity(string activityName)
        {
            if (!ActivitySource.Value.HasListeners()) return null;
            var activity = ActivitySource.Value.StartActivity(activityName, ActivityKind.Client);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbOperationName, activityName);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbSystem, "solr");
            return activity;
        }

        public static Activity StartSolrActivity(IEnumerable<KeyValuePair<string, string>> solrParams)
        {
            if (!ActivitySource.Value.HasListeners()) return null;
            var actionName = solrParams.Where(_ => _.Key == "action").Select(_ => _.Value).FirstOrDefault() ?? "send";
            var activity = ActivitySource.Value.StartActivity(actionName, ActivityKind.Client);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbOperationName, actionName);
            activity?.SetTag(DiagnosticHeaders.SemanticConventions.DbSystem, "solr");
            return activity;
        }
        
        internal static void EnrichCurrentActivity(IHttpWebRequest request) =>
            EnrichCurrentActivity(request.Method.ToString(), request.RequestUri);
            
        internal static void EnrichCurrentActivity(string httpMethod, Uri requestUri)
        {
            if (Activity.Current == null) return;
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.DbCollectionName, ExtractCore(requestUri.ToString()));
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.ServerAddress, requestUri.Host);
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.ServerPort, requestUri.Port);
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.UrlFull, requestUri.ToString());
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.HttpRequestMethod, httpMethod);
        }

        internal static void EnrichCurrentActivity(IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            if (Activity.Current == null) return;
            var parameters = queryParameters
                .Where(_ => _.Key != "wt" && _.Key != "version")
                .Select(_ => new Dictionary<string, string> { { _.Key, _.Value } });
            if (!parameters.Any()) return;
            var queryParamsAsJson = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.DbQueryText, queryParamsAsJson);
        }
        
        internal static void EnrichCurrentActivity(ResponseHeader header)
        {
            if (Activity.Current == null || header == null) return;
            Activity.Current.SetTag(DiagnosticHeaders.SolrNetConventions.Status, header.Status);
            Activity.Current.SetTag(DiagnosticHeaders.SolrNetConventions.QTime, header.QTime);
        }

        internal static void EnrichCurrentActivityWithError(string errorMessage)
        {
            if (Activity.Current == null) return;
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.ErrorType, errorMessage);
            Activity.Current.SetStatus(ActivityStatusCode.Error);
        }
        
        internal static void EnrichCurrentActivityWithError(Exception e)
        {
            if (Activity.Current == null) return;
            Activity.Current.SetTag(DiagnosticHeaders.SemanticConventions.ErrorType, e.Message);
            Activity.Current.SetStatus(ActivityStatusCode.Error);
        }
        
        private static string ExtractCore(string url)
        {
            var match = Regex.Match(url, @"\/solr\/([^\/]+)\/", RegexOptions.IgnoreCase);
            if (!match.Success) return null;
            var core = match.Groups[1].Value;
            
            if (core.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                core.Equals("config", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return core;
        }
    }
}
