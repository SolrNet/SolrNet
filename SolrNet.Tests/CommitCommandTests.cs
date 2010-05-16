using System;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands;

namespace SolrNet.Tests
{
    [TestFixture]
    public class CommitCommandTests
    {
        public delegate string Writer(string ignored, string s);

        [Test]
        public void ExecuteBasic()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            With.Mocks(mocks).Expecting(() =>
            {
                conn.Post("/update", "<commit />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new CommitCommand();
                cmd.Execute(conn);
            });
        }

        [Test]
        public void ExecuteWithBasicOptions()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            With.Mocks(mocks).Expecting(() =>
            {
                conn.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new CommitCommand { WaitFlush = true, WaitSearcher = true };
                cmd.Execute(conn);
            });
        }

        [Test]
        public void ExecuteWithAllOptions()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            With.Mocks(mocks).Expecting(() =>
            {
                conn.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new CommitCommand
                              {
                                  MaxSegments = 2,
                                  ExpungeDeletes = true,
                                  WaitFlush = true,
                                  WaitSearcher = true
                              };
                cmd.Execute(conn);
            });
        }    
    }
}