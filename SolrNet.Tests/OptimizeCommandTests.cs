using System;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands;

namespace SolrNet.Tests
{
    [TestFixture]
    public class OptimizeCommandTests
    {
        public delegate string Writer(string ignored, string s);

        [Test]
        public void ExecuteBasic()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            With.Mocks(mocks).Expecting(() =>
            {
                conn.Post("/update","<optimize />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new OptimizeCommand();
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
                conn.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new OptimizeCommand();
                cmd.WaitFlush = true;
                cmd.WaitSearcher = true;
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
                conn.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
            }).Verify(() =>
            {
                var cmd = new OptimizeCommand();
                cmd.MaxSegments = 2;
                cmd.ExpungeDeletes = true;
                cmd.WaitFlush = true;
                cmd.WaitSearcher = true;
                cmd.Execute(conn);
            });
        }
    }
}