﻿namespace NServiceBus.AcceptanceTests.Sagas
{
    using System;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using EndpointTemplates;
    using Features;
    using NUnit.Framework;
    using Persistence.Sql;

    public class When_using_contain_saga_data : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_handle_timeouts_properly()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<EndpointThatHostsASaga>(
                    b => b.When(session => session.SendLocal(new StartSaga
                    {
                        DataId = Guid.NewGuid()
                    })))
                .Done(c => c.TimeoutReceived)
                .Run();

            Assert.True(context.TimeoutReceived);
        }

        public class Context : ScenarioContext
        {
            public bool TimeoutReceived { get; set; }
        }

        public class EndpointThatHostsASaga : EndpointConfigurationBuilder
        {
            public EndpointThatHostsASaga()
            {
                EndpointSetup<DefaultServer>(config => config.EnableFeature<TimeoutManager>());
            }

            [SqlSaga(CorrelationProperty = nameof(MySagaData.DataId))]
            public class MySaga : SqlSaga<MySaga.MySagaData>,
                IAmStartedByMessages<StartSaga>,
                IHandleTimeouts<MySaga.TimeHasPassed>
            {
                public Context TestContext { get; set; }

                public Task Handle(StartSaga message, IMessageHandlerContext context)
                {
                    Data.DataId = message.DataId;

                    return RequestTimeout(context, TimeSpan.FromMilliseconds(1), new TimeHasPassed());
                }

                public Task Timeout(TimeHasPassed state, IMessageHandlerContext context)
                {
                    MarkAsComplete();
                    TestContext.TimeoutReceived = true;
                    return Task.FromResult(0);
                }

                protected override void ConfigureMapping(MessagePropertyMapper<MySagaData> mapper)
                {
                    mapper.MapMessage<StartSaga>(m => m.DataId);
                }

                public class MySagaData : ContainSagaData
                {
                    public virtual Guid DataId { get; set; }
                }

                public class TimeHasPassed
                {
                }
            }
        }

        public class StartSaga : IMessage
        {
            public Guid DataId { get; set; }
        }
    }
}