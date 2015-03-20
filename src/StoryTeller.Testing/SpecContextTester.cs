﻿using System;
using System.Linq;
using System.Threading;
using FubuCore;
using NUnit.Framework;
using Shouldly;
using StoryTeller.Model;
using StoryTeller.Results;
using StoryTeller.Testing.Results;

namespace StoryTeller.Testing
{
    [TestFixture]
    public class SpecContextTester
    {
        private SpecContext theContext;

        [SetUp]
        public void SetUp()
        {
            theContext = new SpecContext(new Specification(), null, new NulloResultObserver(), new StopConditions(), new InMemoryServiceLocator());

        }

        [Test]
        public void log_exception_does_the_counts()
        {
            theContext.LogException("1", new NotImplementedException());

            theContext.Counts.ShouldEqual(0, 0, 1, 0);
        }

        [Test]
        public void log_exception_will_unwrap_the_exception_message()
        {
            theContext.LogException("1", new StorytellerAssertionException("It is wrong"));
            theContext.Results.OfType<StepResult>().Single().error
                .ShouldBe("It is wrong");
        }

        [Test]
        public void log_exception_logs_a_result()
        {
            var exception = new NotImplementedException();
            theContext.LogException("1", exception, position: Stage.setup);

            var result = theContext.Results.Single().ShouldBeOfType<StepResult>();
            result.Status.ShouldBe(ResultStatus.error);
            result.error.ShouldBe(exception.ToString());
            result.position.ShouldBe(Stage.setup.ToString());
        }

        [Test]
        public void wait_is_true_if_the_condition_is_already_true()
        {
            ShouldBeTestExtensions.ShouldBe(theContext.Wait(() => true, new TimeSpan(0, 0, 0, 0, 500)), true);
        }

        [Test]
        public void wait_is_false_if_the_condition_is_never_met()
        {
            ShouldBeTestExtensions.ShouldBe(theContext.Wait(() => false, new TimeSpan(0, 0, 0, 0, 100), 25), false);
        }

        [Test]
        public void automatically_false_if_the_context_is_cancelled()
        {
            theContext.RequestCancellation();
            ShouldBeTestExtensions.ShouldBe(theContext.Wait(() => true, new TimeSpan(0, 0, 0, 500)), false);
        }

        [Test]
        public void puts_the_spec_id_on_result_messages()
        {
            theContext.LogException("1", new NotImplementedException());

            theContext.Results.Last().spec.ShouldBe(theContext.Specification.id);
        }

        [Test]
        public void writes_contextual_logging_into_results()
        {
            theContext.Reporting.ReporterFor<DivReport>().Add("1");
            theContext.Reporting.ReporterFor<ListReport>().Add("2");

            var results = theContext.FinalizeResults(3);

            results.Reporting.Count().ShouldBe(2);

            
        }

        [Test]
        public void stores_the_attempt_number_on_the_finalized_results()
        {
            var results = theContext.FinalizeResults(3);
            results.Attempts.ShouldBe(3);
        }

        [Test]
        public void stores_whether_there_was_a_critical_exception()
        {
            ShouldBeTestExtensions.ShouldBe(theContext.HadCriticalException, false);
            ShouldBeTestExtensions.ShouldBe(theContext.FinalizeResults(3)
                    .HadCriticalException, false);

            theContext.LogException("1", new StorytellerCriticalException("Boo!"));
            ShouldBeTestExtensions.ShouldBe(theContext.HadCriticalException, true);

            ShouldBeTestExtensions.ShouldBe(theContext.FinalizeResults(3)
                    .HadCriticalException, true);
        }
    }

    [TestFixture]
    public class when_testing_for_should_continue
    {
        private SpecContext theContext;
        private CancellationTokenSource theCancellation;

        [SetUp]
        public void SetUp()
        {
            theContext = new SpecContext(new Specification(), null, new NulloResultObserver(), new StopConditions(), new InMemoryServiceLocator());
        }

        [Test]
        public void just_starting_with_default_options()
        {
            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), true);
        }

        [Test]
        public void stops_with_a_critical_exception_even_if_there_is_no_other_failure()
        {
            theContext.StopConditions.BreakOnExceptions = false;

            theContext.LogException("1", new StorytellerCriticalException());

            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), false);
        }

        [Test]
        public void stops_with_a_catastropic_exception_even_if_there_is_no_other_failure()
        {
            theContext.StopConditions.BreakOnExceptions = false;

            theContext.LogException("1", new StorytellerCatastrophicException());

            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), false);
        }

        [Test]
        public void stops_when_the_cancellation_token_has_been_cancelled()
        {
            theContext.RequestCancellation();

            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), false);
        }

        [Test]
        public void do_not_stop_on_normal_exceptions_if_the_type_is_not_critical()
        {
            theContext.LogException("1", new NotImplementedException());

            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), true);
        }

        [Test]
        public void stop_on_the_first_exception_if_condition_is_to_do_that()
        {
            theContext.StopConditions.BreakOnExceptions = true;

            theContext.LogException("1", new NotImplementedException());

            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), false);
        }

        [Test]
        public void stop_on_the_first_wrong_when_stop_conditions_say_so()
        {
            theContext.StopConditions.BreakOnWrongs = true;

            theContext.LogResult(new StepResult("1", ResultStatus.success));
            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), true);

            theContext.LogResult(new StepResult("1", ResultStatus.failed));
            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), false);
        }

        [Test]
        public void will_not_stop_on_wrong_if_stop_conditions_are_the_defaults()
        {

            theContext.LogResult(new StepResult("1", ResultStatus.failed));
            ShouldBeTestExtensions.ShouldBe(theContext.CanContinue(), true);
        }
    }
}