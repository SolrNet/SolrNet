# Change Log

## 1.1.1

- Fix typo in "router.field" parameter #595 (@Jure-BB)

## 1.1.0
 - Add LightInject support (@jevgenigeurtsen)
 - Support basic auth in non-async operations (@jevgenigeurtsen)
 - Fix commitWithin for atomic updates (@SirMrDexter)
 - Fix reuse of HttpClients in MS DI (@hoerup)
 - Fix issue with large requests (@deefco)
 - Fix getting error message with POST requests (@pmuessig)
 - Fix edge case with POST requests (@pmuessig)
 - Fix parsing of dynamic fields from schema (@yokuno)
 - Fix parsing of extract response (@ancailliau)
 - Improve MS DI integration (@bernarden)
 - Drop SolrNet.DSL project
 - Drop NHibernate.SolrNet project
 - Revamped integration tests and build scripts

## 1.0.19 (2019-11-21)
- Support overriding the default wt=xml parameter in order to support JSON response. #509 (@LCHarold)

## 1.0.18 (2019-10-06)
- Update SolrPostConnection to use IHttpWebRequestFactory #502 (@YevhenLukomskyi)
- Autofac Integration for SolrNet.Cloud #498 (@femiadebayo)

## 1.0.17 (2019-06-25)
- Provide support for adding request body (eg. JSON) in Solr query #481 (@mattflax)

## 1.0.16 (2019-06-09)
- Provide support for date and string fields in stats result. #484 (@Jaap-van-Hengstum)
- Bug fix: SolrCloud still sends traffic to dead nodes #479 (@karlomedallo)
- Bug fix: Atomic Update Command Name Bugfix #482 (@1dot44mb)
- Documentation Autofac multicore example #477 (@meanin)

## 1.0.15 (2019-03-19)
- Update Castle Windsor integration fixes issues #472 (@hesamkashefi)

## 1.0.14 (2019-01-27)
- Updated ZooKeeperNetEx version to 3.4.12.1 #467 (@karlomedallo)
- Fix SimpleInjector registration improvement fix issue #461 (@kaink4)

## 1.0.13 (2018-10-29)
- Perf Fix: have Microsoft Dependency Injection and SimpleInjector use MemoizingMappingManager. #452 
- Various AutoSolrConnection improvements.

## 1.0.12 (2018-10-11)
- MS Dependency Injection: Allow setting Http client basic authentication header for typed instances #449 (@amitsingh5290 )
- Fix json serialization issues in AtomicUpdateCommand #448 (@Panopto)

## 1.0.11 (2018-09-22)
- Added SimpleInjector support #430 (@jrmartins)
- Support multiple cores when using Microsoft Dependency Injection #432 (@tspayne87)
- Added support for percentiles in stats queries #442 (@PulasthiSeneviratne)
- Fix: Send date in UTC time zone to Solr Server #439 (@mr-KVA)

## 1.0.10 (2018-07-26)
- QueryInListSerializer should not always auto quote
- Overriding the default mapper, built-in container example doesn't work #423

## 1.0.9 (2018-05-14)
- Added missing properties to SolrFacetFieldQuery

## 1.0.8 (2018-04-02)
- Microsoft Dependency Injection update: use AutoSolrConnection, and allow tweaking HttpClient on service configuration. #409
- Added ability to force the usage of the reflected type when adding a property mapping #407 (@davewut)

## 1.0.7 (2018-03-05)
- Fix: Parsing error when spellcheck.extendedResults=true #398 (@ariasjose)
- Tweaked the AutoSolrConnection `MaxUriLength`.
- Fix: AtomicUpdate async doesn't work due to stream being closed prematurely #401
- Use AutoSolrConnection instead of SolrConnection in StructureMap.SolrNetIntegration #400 (@chyczewski-maciej)

## 1.0.6 (2018-02-26)
- New: `AutoSolrConnection`, automatically uses `GET` or `POST` depending on uri length. Improved performance when using `async` methods. 
- Add `netstandard 2.0` support to `Unity.SolrNetIntegration`

## 1.0.5 (2018-02-13)
- SolrNet Cloud: add checks if Zookeeper connection is valid

## 1.0.4 (2018-02-04)
- Add `netstandard 2.0` support to `Ninject.Integration.SolrNet`
- Add `netstandard 2.0` support to `AutofacContrib.SolrNet`

## 1.0.3 (2018-02-05)
- Add `netstandard 2.0` support to `StructureMap.SolrNetIntegration` #376 (@ciprianmo)
- Fix: Attempting to add duplicate SolrField should give a more helpful error #380
 
## 1.0.2 (2018-01-24)
- Fix: Error when collations contain duplicates #373 (@ariasjose)
- House keeping: move to new csproj format. Merge nuspec files into csproj. #374

## 1.0.1 (2018-01-15)
- Upgrade `Unity` from 4.0 to 5.0  #364 (@JeroenvdBurg)
- Change Contributing guide to follow the GitHub Fork and Pull model.
- Support for PostSolrConnections in SolrNet.Windsor  (@adegroff)

## 1.0.0 (2018-01-01)
- **SolrNet.Cloud**: First class SolrCloud support
- Move to semantic versioning
- Added support for custom request handlers (RequestHandlerParameters)
- Breaking change: SolrQueryExecuter Handler property changed to DefaultHandler
- Add support for `group.facet` parameter #215  (@drakeh)
- Adding `collations` support for Solr 4+ #348 (@ariasjose)
- Added support for `Atomic Updates` #341 (@alanh-ti)
- Added Support for managed-schema in `solr.EnumerateValidationResults()` #357 (@bkrbkr)

## 0.9.1 (2017-11-02)
* Add support for `ConstantScore` query
* Fixed support for Solr 7.0 due to breaking change, in which JSON  is the default response formatter instead of XML
* More DI libraries are now `netstandard 2.0` compliant
* Decoupled shared unit test code.

## 0.8.1 (2017-09-27)
* Added Range Faceting support
* Added Interval Faceting support
* Marked Date Faceting as obsolete.

## 0.7.1 (2017-08-28)
- Added `netstandard 2.0` support
- Added `Microsoft.Extensions.DependencyInjection` support
- Added `Async` support
- Use `xUnit` for unit tests instead of MBUnit
- Upgraded `Unity` to 4.0.1
- Upgraded `StructureMap` to 4.5.2
- Added `AppVeyor.yml` for default build scripts
- Cleaned up solution

## 0.6.1 (2017-08-15)
- Upgraded all projects to .NET 4.6
- Upgraded Ninject to 3.2.2
- Upgraded CommonServiceLocator to 1.3 
- Upgraded Autofac to 4.6.1.0

## 0.5.1
- Fix handling of NaN in stats
- Handle multi-value field containing one or more nulls
- Spellcheck and Collation Parser Improvement

## 0.5.0a1 (2015-05-04)
- Breaking change: queries that quote values now also quote slash ('/') due to changes in Solr/Lucene 4 (https://issues.apache.org/jira/browse/LUCENE-2604)
- Upgraded to Ninject 3
- Added commitWithin option to Delete
- Added support for TermVectorComponent
- Added SolrRequiredQuery
- Fixed milliseconds in DateTime serialization
- Added multicore to StructureMap integration
- Added multicore to Unity integration
- Fixed parens in query by field expressions with spaces in value
- Added core admin commands
- Added multicore to Ninject module
- Fixed commit without parameters should not send waitFlush (issue #172)
- Fixed MappingManager with class hierarchy (issue #37)

## 0.4.0b2 (2011-12-29)


- Upgraded to Windsor 3.0
- Fixed cache injection in Structuremap integration
- Fixed response parser registrations in built-in container

## 0.4.0b1 (2011-12-04)

- Upgraded to NHibernate 3.2.0
- Added support for MoreLikeThis handler
- Fixed intermitent bug in NHibernate integration
- Breaking change: removed query result interfaces (ISolrQueryResults)
- Friendlier highlighting results (issue #96)
- Added Terms component support (issue #122)
- Added Clustering component support (issue #121)
- Fixed issue #159 : ignore LocalParams for facet date parameters
- Added option to disable quoting in SolrQueryByField
- Added ngroups for grouping
- Fixed bug with fastVectorHighlighter
- Fixed bug when using SolrNet in Application_Start in IIS 7+
- Upgraded to Autofac 2.5.2
- Added support for multicore in Autofac module
- Obsoleted Add(IEnumerable<T>). Use AddRange() instead.

## 0.4.0a1 (2011-06-19)

- Upgraded to StructureMap 2.6.2
- Added mixed exclusive/inclusive range queries (for Solr 4.0) (issue #142)
- Breaking change for IReadOnlyMappingManager implementors: it now indexes fields by name to speed up lookup
- Added Solr 4.0 grouping (issue #127)
- Added Solr 4.0 pivot faceting (issue #128)
- Fixed support for nullable enum properties
- Added Unity integration
- Breaking change: SolrQueryByField now quotes '*' and '?'
- Upgraded to Windsor 2.5.3
- Upgraded to Ninject 2.2.1.0
- Fluent: added index-time document boosting
- Fluent: added easier way to set Solr URL and timeout
- Added SolrQueryByDistance
- Upgraded to NHibernate 3.1.0
- Added support for ExtractingRequestHandler (issue #79)
- Added Rollback (missing in ISolrOperations)
- Added CommitWithin and Overwrite parameters for document add (issue #85)
- Upgraded to .NET 3.5
- Minor breaking change: removed SolrConnection constructor with IHttpWebRequestFactory parameter. Made IHttpWebRequestFactory a property.
- Added Autofac integration module (issue #85)

## 0.3.1 (2011-03-31)

- Fixed issue #139 : fixed parsing of decimals with exponential notation
- Fixed SolrQueryInList with empty strings
- Fixed facet.missing=true
- Added support for nullable Guid properties
- Fixed date faceting for Solr 3.x by ignoring 'start' element
- Fixed issue #135 : NRE with facet.missing=true
- Fixed issue #130 : null in range queries translated to *
- Fixed issue #133 : ignore LocalParams for facet field parameters

## 0.3.0 (2010-12-05)

- NuGet packages
- Upgraded to Ninject 2.1.0.76
- Upgraded to Windsor 2.5.2
- Signed assemblies
- Fixed support for readonly and writeonly properties in document type
- Fixed issue #113: duplicate add in NHibernate integration
- Fixed bug with attributes not being picked up by a class higher in the class hierarchy
- Improved HTTP performance by setting KeepAlive and HTTP/1.1 for POSTS
- Fixed NHibernate integration overriding existing event listeners
- Improved response parsing performance
- Fixed issue #93: nullable DateTime range queries
- Added support for multi-core in StructureMap registry
- Upgraded to StructureMap 2.6.1

## 0.3.0 beta1 (2010-06-08)

- Breaking change: field collapsing changed completely.
- Breaking change: removed ServerURL and Version properties from ISolrConnection.
- Breaking change: changed Highlighting and MoreLikeThis result classes. Indices are now string instead of T.
- Breaking change: all chainable methods on ISolrOperations et al now return ResponseHeader instead of 'this'.
- Breaking change: removed NoUniqueKeyException. Now IReadOnlyMappingManager.GetUniqueKey() returns null if there is no unique key.
- Added mapping validation
- Upgraded to Windsor 2.1.1
- Added StructureMap integration
- Fixed culture-related bug in highlighting parameters
- Fixed culture-related bug in range query
- Added MaxSegments and ExpungeDeletes parameters to commit/optimize
- Breaking change: renamed WaitOptions to CommitOptions
- Breaking change: fixed field boosting, was of type int, now is float
- Added field index-time boosting (issue #98)
- Breaking change: removed obsolete exceptions: BadMappingException, CollectionTypeNotSupportedException, FieldNotFoundException
- Added support for delete by id+query in the same request (issue #50)
- Fixed issue #95 : Highlights didn't support several snippets in results
- Fixed performance issue with SolrMultipleCriteriaQuery (issue #94)
- Breaking change: removed ISolrDocument interface
- Added support for loose mapping (issue #53)
- Improved multi-core configuration in Windsor facility (issue #70)
- Added Rollback command (issue #51)
- Added HTTP-level caching (issue #75)
- Added operator - for queries
- Added support for LocalParams (issue #62)

## 0.2.3 (2009-12-29)

- Upgraded to NHibernate 2.1.2
- Upgraded Solr in sample app to 1.4.0

## 0.2.3 beta1 (2009-09-13)

- Fixed minor date parsing bug
- Added support for field collapsing
- Added support for date-faceting (issue #7)
- Upgraded to Ninject trunk
- Upgraded sample app's Solr to nightly
- Added StatsComponent support (issue #67)
- Added index-time document boosting (issue #65)
- Added query-time document boosting (issue #57)
- Bugfix: response parsing was not fully culture-independent (issue #61)
- All exceptions are now serializable
- Fixed potential timeout issue
- NHibernate integration
- Fixed Not() query operator returning wrong type

## 0.2.2 (2009-05-07)

- Bugfix: semicolons are now correctly escaped in queries.
- Bugfix: invalid xml characters (control chars) are now correctly filtered.
- Deleting a list (IEnumerable) of documents now uses a single request (requires unique key and Solr 1.3+) 
- Added support for arbitrary parameters, using the QueryOptions.ExtraParams dictionary.
- Added per-field facet parameters.
- Breaking change: as a consequence of the previous change, facet queries and other facet parameters were moved to FacetParameters.
- Added a couple of fluenty QueryOptions building methods.
- Added dictionary mapping support.
- Upgraded Windsor facility to use Windsor 2.0
- Merged all SolrNet assemblies (SolrNet, SolrNet.DSL, the Castle facility, the Ninject module and the internal HttpWebAdapters).
- Windsor and Ninject are not packaged anymore.

## 0.2.1 (2009-02-25)

- Added support for Spell checking
- Added support for More like this
- Added explicit support for random sorting
- Added "has any value" query
- Fluent interface for query building

## 0.2.0 (2009-02-18)

- Major rewrite
- Deprecated ISolrDocument interface
- Dropped query by example 
- Dropped random sorting
- Added several ways to map solr fields to properties
- Added highlighting 
- Added filter queries 
- Added ping 
- Added sample application 
- Added Windsor facility 
- Added Ninject module 
- Added operator overloading for queries 
- Added MSDN-style docs 
- Added more code samples, better organized wiki
- Changed initialization and instantiation of the service

## 0.1 (2008-09-15)

- Initial release
